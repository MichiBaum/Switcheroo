using System;
using System.Threading.Tasks;

namespace Switcheroo {
    public class WindowCloser : IDisposable {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMilliseconds(125);
        private bool _isDisposed;

        public void Dispose() {
            _isDisposed = true;
        }

        public async Task<bool> TryCloseAsync(AppWindowViewModel window) {
            window.IsBeingClosed = true;
            window.AppWindow.Close();

            while (!_isDisposed && !window.AppWindow.IsClosedOrHidden)
                await Task.Delay(CheckInterval).ConfigureAwait(false);

            return window.AppWindow.IsClosedOrHidden;
        }
    }
}