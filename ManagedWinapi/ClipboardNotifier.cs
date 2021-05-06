// Copyright by Switcheroo

#region

using ManagedWinapi.Windows;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace ManagedWinapi {
    /// <summary>
    ///     Specifies a component that monitors the system clipboard for changes.
    /// </summary>
    [DefaultEvent("ClipboardChanged")]
    public class ClipboardNotifier : Component {
        private static Boolean instantiated;

        private static readonly int
            WM_DRAWCLIPBOARD = 0x0308,
            WM_CHANGECBCHAIN = 0x030D;

        private readonly EventDispatchingNativeWindow ednw;

        private readonly IntPtr hWnd;
        private IntPtr nextHWnd;

        /// <summary>
        ///     Creates a new clipboard notifier.
        /// </summary>
        /// <param name="container">The container.</param>
        public ClipboardNotifier(IContainer container)
            : this() {
            container.Add(this);
        }

        /// <summary>
        ///     Creates a new clipboard notifier.
        /// </summary>
        public ClipboardNotifier() {
            if (instantiated) {
                // use new windows if more than one instance is used.
                Debug.WriteLine("WARNING: More than one ClipboardNotifier used!");
                ednw = new EventDispatchingNativeWindow();
            } else {
                ednw = EventDispatchingNativeWindow.Instance;
                instantiated = true;
            }

            ednw.EventHandler += clipboardEventHandler;
            hWnd = ednw.Handle;
            nextHWnd = SetClipboardViewer(hWnd);
        }

        /// <summary>
        ///     Occurs when the clipboard contents have changed.
        /// </summary>
        public event EventHandler ClipboardChanged;

        /// <summary>
        ///     Frees resources.
        /// </summary>
        protected override void Dispose(bool disposing) {
            ChangeClipboardChain(hWnd, nextHWnd);
            ednw.EventHandler -= clipboardEventHandler;
            base.Dispose(disposing);
        }

        private void clipboardEventHandler(ref Message m, ref bool handled) {
            if (handled)
                return;
            if (m.Msg == WM_DRAWCLIPBOARD) {
                // notify me
                if (ClipboardChanged != null)
                    ClipboardChanged(this, EventArgs.Empty);
                // pass on message
                SendMessage(nextHWnd, m.Msg, m.WParam, m.LParam);
                handled = true;
            } else if (m.Msg == WM_CHANGECBCHAIN) {
                if (m.WParam == nextHWnd)
                    nextHWnd = m.LParam;
                else
                    SendMessage(nextHWnd, m.Msg, m.WParam, m.LParam);
            }
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll")]
        private static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
    }
}