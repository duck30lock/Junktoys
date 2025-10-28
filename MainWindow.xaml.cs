using System;
using System.Windows;
using System.Windows.Controls;
using WindowsDebloater.Pages;
using System.Diagnostics;
using System.Linq;

namespace WindowsDebloater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame.Navigate(new BackgroundAppsPage());
        }

        private void Navigation_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && ContentFrame != null)
            {
                Page page = rb.Tag?.ToString() switch
                {
                    "BackgroundApps" => new BackgroundAppsPage(),
                    "Bloatware" => new BloatwareDetectorPage(),
                    "Startup" => new StartupManagerPage(),
                    "Services" => new ServicesPage(),
                    "Advanced" => new AdvancedPage(),
                    "Settings" => new SettingsPage(),
                    _ => null
                };

                if (page != null)
                {
                    ContentFrame.Navigate(page);
                }
            }
        }

        private void QuickClean_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Quick Clean will:\n\n" +
                "• Kill apps using >500MB RAM\n" +
                "• Clear temp files\n" +
                "• Optimize memory\n\n" +
                "Continue?",
                "Quick Clean",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Kill high memory processes
                    var processes = Process.GetProcesses()
                        .Where(p => p.WorkingSet64 > 500 * 1024 * 1024)
                        .Where(p => !IsCriticalProcess(p.ProcessName))
                        .ToList();

                    int killed = 0;
                    foreach (var proc in processes)
                    {
                        try { proc.Kill(); killed++; } catch { }
                    }

                    // Clear temp
                    Process.Start("cmd.exe", "/c del /q /f /s %temp%\\* 2>nul");

                    MessageBox.Show($"Quick Clean complete!\n\n• Killed {killed} heavy apps\n• Cleared temp files", 
                        "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsCriticalProcess(string name)
        {
            string[] critical = { "System", "csrss", "smss", "services", "lsass", "winlogon", "explorer", "dwm", "svchost" };
            return critical.Any(p => name.Equals(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
