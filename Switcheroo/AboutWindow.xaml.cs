using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

namespace Switcheroo {
    public partial class AboutWindow : Window {
        public AboutWindow() {
            InitializeComponent();
            VersionNumber.Inlines.Add(Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        private void HandleRequestNavigate(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is not Hyperlink hyperlink)
                return;

            string navigateUri = hyperlink.NavigateUri.ToString();
            Process.Start(new ProcessStartInfo(navigateUri));
            e.Handled = true;
        }

        private void Ok_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}