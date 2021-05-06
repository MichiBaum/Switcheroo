﻿using Switcheroo.Properties;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace Switcheroo {
    internal class Program {
        private const string mutex_id = "DBDE24E4-91F6-11DF-B495-C536DFD72085-switcheroo";

        [STAThread]
        private static void Main() {
            RunAsAdministratorIfConfigured();

            using Mutex mutex = new(false, mutex_id);
            bool hasHandle = false;
            try {
                try {
                    hasHandle = mutex.WaitOne(5000, false);
                    if (!hasHandle)
                        return; //another instance exist
                } catch (AbandonedMutexException amex) {
                    // Log the fact the mutex was abandoned in another process, it will still get aquired
                }

#if PORTABLE
                        MakePortable(Settings.Default);
#endif

                MigrateUserSettings();

                App app = new() {MainWindow = new MainWindow()};
                app.Run();
            } finally {
                if (hasHandle)
                    mutex.ReleaseMutex();
            }
        }

        private static void RunAsAdministratorIfConfigured() {
            if (RunAsAdminRequested() && !IsRunAsAdmin()) {
                ProcessStartInfo proc = new() {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Assembly.GetEntryAssembly().CodeBase,
                    Verb = "runas"
                };

                Process.Start(proc);
                Environment.Exit(0);
            }
        }

        private static bool RunAsAdminRequested() {
            return Settings.Default.RunAsAdmin;
        }

        // TODO unused method?
        private static void MakePortable(ApplicationSettingsBase settings) {
            PortableSettingsProvider portableSettingsProvider = new();
            settings.Providers.Add(portableSettingsProvider);
            foreach (SettingsProperty prop in settings.Properties) prop.Provider = portableSettingsProvider;
            settings.Reload();
        }

        private static void MigrateUserSettings() {
            if (!Settings.Default.FirstRun)
                return;

            Settings.Default.Upgrade();
            Settings.Default.FirstRun = false;
            Settings.Default.Save();
        }

        private static bool IsRunAsAdmin() {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(id);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}