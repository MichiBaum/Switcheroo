// Copyright by Switcheroo

#region

using System;
using System.Threading.Tasks;

#endregion

namespace Switcheroo {
    public class WindowCloser : IDisposable {
        private readonly TimeSpan CHECK_INTERVAL = TimeSpan.FromMilliseconds(125);
        private bool _isDisposed;

        public void Dispose() {
            _isDisposed = true;
        }

        public async Task<bool> TryCloseAsync(AppWindowViewModel window) {
            window.IsBeingClosed = true;
            window.AppWindow.Close();

            while (!_isDisposed && !window.AppWindow.IsClosedOrHidden)
                await Task.Delay(CHECK_INTERVAL).ConfigureAwait(false);

            return window.AppWindow.IsClosedOrHidden;
        }
    }
}