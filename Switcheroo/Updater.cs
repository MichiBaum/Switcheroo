// Copyright by Switcheroo

using Switcheroo.Properties;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Switcheroo {
    public interface IUpdater
    {
        void CheckForUpdates();
    }

    public class Updater : IUpdater {
        
        public void CheckForUpdates() {
            Version currentVersion = Assembly.GetEntryAssembly().GetName().Version;
            if (currentVersion == new Version(0, 0, 0, 0)) return;

            DispatcherTimer timer = new();

            timer.Tick += async (sender, args) => {
                timer.Stop();
                Version? latestVersion = await GetLatestVersion();
                if (latestVersion != null && latestVersion > currentVersion) {
                    MessageBoxResult result = MessageBox.Show(
                        string.Format(
                            language_en.MainWindow_CheckForUpdates_,
                            latestVersion, currentVersion),
                        "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result ==
                        MessageBoxResult
                            .Yes) // TODO can throw exception like https://stackoverflow.com/a/53245993/10258204
                        Process.Start("https://github.com/MichiBaum/Switcheroo/releases/latest");
                } else {
                    timer.Interval = new TimeSpan(24, 0, 0);
                    timer.Start();
                }
            };

            timer.Interval = new TimeSpan(0, 0, 0);
            timer.Start();
        }

        private async Task<Version?> GetLatestVersion() {
            try {
                // TODO add own versioning
                string versionAsString = await new WebClient()
                    .DownloadStringTaskAsync("https://raw.github.com/MichiBaum/Switcheroo/update/version.txt")
                    .ConfigureAwait(false);
                if (Version.TryParse(versionAsString, out Version? newVersion)) return newVersion;
            } catch (WebException) {
            }

            return null;
        }
    }
}