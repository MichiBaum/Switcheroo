using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Switcheroo {
    // Create a shortcut file in the current users start up folder
    // Based on this answer on Stackoverflow:
    // http://stackoverflow.com/a/19914018/198065
    public class AutoStart {
        public bool IsEnabled {
            get => HasShortcut();

            set {
                string appLink = GetAppLinkPath();

                if (value)
                    CreateShortcut(appLink);
                else if (IsEnabled) DeleteShortcut(appLink);
            }
        }

        private static bool HasShortcut() {
            try {
                return File.Exists(GetAppLinkPath());
            } catch {
                return false;
            }
        }

        private static string GetAppLinkPath() {
            string appDataStart =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Start Menu\Programs\Startup");
            return Path.Combine(appDataStart, "Switcheroo.lnk");
        }

        private static void DeleteShortcut(string appLink) {
            try {
                File.Delete(appLink);
            } catch {
                throw new AutoStartException(
                    "It was not possible to delete the shortcut to Switcheroo in the startup folder");
            }
        }

        private static void CreateShortcut(string appLink) {
            try {
                string exeLocation = Assembly.GetEntryAssembly().Location;

                //Windows Script Host Shell Object
                Type? t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
                if (t == null) return;
                dynamic? shell = Activator.CreateInstance(t);
                try {
                    dynamic? lnk = shell?.CreateShortcut(appLink);
                    try {
                        if (lnk == null) return;
                        lnk.TargetPath = exeLocation;
                        lnk.Save();
                    } finally {
                        Marshal.FinalReleaseComObject(lnk);
                    }
                } finally {
                    Marshal.FinalReleaseComObject(shell);
                }
            } catch {
                throw new AutoStartException(
                    "It was not possible to create a shortcut to Switcheroo in the startup folder");
            }
        }
    }
}