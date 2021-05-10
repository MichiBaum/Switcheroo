using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ManagedWinapi {
    /// <summary>
    ///     This component displays a crosshair icon that can be dragged to any point
    ///     on screen. This is useful to select other programs by dragging the crosshair
    ///     to a program window.
    /// </summary>
    [DefaultEvent("CrosshairDragged")]
    public partial class Crosshair : UserControl {
        private readonly Cursor myCursor;
        private readonly Image myImage;

        /// <summary>
        ///     Creates a new crosshair control.
        /// </summary>
        public Crosshair() {
            InitializeComponent();
            myImage = new Bitmap(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ManagedWinapi.crosshair.ico") ?? throw new InvalidOperationException());
            myCursor = new Cursor(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ManagedWinapi.crosshair.ico") ?? throw new InvalidOperationException());
            dragger.Image = myImage;
        }

        /// <summary>
        ///     Occurs when the user finished dragging the crosshair. Use
        ///     <see cref="Cursor.Position" /> to detect the cursor position.
        /// </summary>
        public event EventHandler CrosshairDragged;

        /// <summary>
        ///     Occurs while the user drags the crosshair. Use
        ///     <see cref="Cursor.Position" /> to detect the cursor position.
        /// </summary>
        public event EventHandler CrosshairDragging;

        private void dragger_MouseDown(object sender, MouseEventArgs e) {
            dragger.Image = null;
            dragger.Cursor = myCursor;
        }

        private void dragger_MouseUp(object sender, MouseEventArgs e) {
            dragger.Image = myImage;
            dragger.Cursor = Cursors.Cross;
            if (CrosshairDragged != null) CrosshairDragged(this, new EventArgs());
        }

        private void dragger_MouseMove(object sender, MouseEventArgs e) {
            if (dragger.Cursor == myCursor)
                if (CrosshairDragging != null)
                    CrosshairDragging(this, new EventArgs());
        }

        /// <summary>
        ///     When a window is hidden, the .NET framework releases mouse capture.
        ///     If you hide your window while the crosshair is dragged, invoke
        ///     this method afterwards to restore mouse capture.
        /// </summary>
        public void RestoreMouseCapture() {
            dragger.Capture = true;
        }
    }
}