// Copyright by Switcheroo

#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Switcheroo.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

#endregion

namespace Switcheroo {
    public class Program {
        private const string mutex_id = "DBDE24E4-91F6-11DF-B495-C536DFD72085-switcheroo";

        public static void Main(string[] args) {
            RunAsAdministratorIfConfigured();
            using Mutex mutex = new(false, mutex_id);
            bool hasHandle = false;
            try {
                hasHandle = mutex.WaitOne(5000, false);
                if (!hasHandle)
                    return; //another instance exist

                MigrateUserSettings();
                CreateHostBuilder(args).Build().Run();
            } catch (AbandonedMutexException) {
                // Log the fact the mutex was abandoned in another process, it will still get aquired
            } finally {
                if (hasHandle)
                    mutex.ReleaseMutex();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); });
        }


        private static void RunAsAdministratorIfConfigured() {
            if (!RunAsAdminRequested() || IsRunAsAdmin()) return;
            ProcessStartInfo proc = new() {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Assembly.GetEntryAssembly()?.CodeBase ?? throw new InvalidOperationException(),
                Verb = "runas"
            };

            Process.Start(proc);
            Environment.Exit(0);
        }

        private static bool RunAsAdminRequested() {
            return Settings.Default.RunAsAdmin;
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