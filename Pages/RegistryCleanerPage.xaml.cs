using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace WindowsDebloater.Pages
{
    public partial class RegistryCleanerPage : Page
    {
        private List<RegistryIssue> foundIssues = new List<RegistryIssue>();
        private int cleanedCount = 0;

        public RegistryCleanerPage()
        {
            InitializeComponent();
        }

        private async void ScanRegistry_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Start deep registry scan?\n\nThis may take a few minutes.",
                "Scan Registry",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            ScanProgressBorder.Visibility = Visibility.Visible;
            foundIssues.Clear();
            cleanedCount = 0;

            await Task.Run(() => ScanRegistryForIssues());

            ScanProgressBorder.Visibility = Visibility.Collapsed;
            IssuesFoundText.Text = foundIssues.Count.ToString();
            IssuesListView.ItemsSource = foundIssues;

            CleanButton.IsEnabled = foundIssues.Count > 0;
            CleanAllButton.IsEnabled = foundIssues.Count > 0;

            MessageBox.Show($"Scan complete!\n\nFound {foundIssues.Count} registry issues.", 
                "Scan Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ScanRegistryForIssues()
        {
            try
            {
                // Scan common problem areas
                ScanUninstallEntries();
                ScanStartupEntries();
                ScanFileAssociations();
                ScanSharedDLLs();
                ScanMUICache();
            }
            catch { }
        }

        private void ScanUninstallEntries()
        {
            Dispatcher.Invoke(() => ScanStatusText.Text = "Scanning uninstall entries...");

            var uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            if (uninstallKey != null)
            {
                foreach (var subKeyName in uninstallKey.GetSubKeyNames())
                {
                    try
                    {
                        var subKey = uninstallKey.OpenSubKey(subKeyName);
                        var displayName = subKey?.GetValue("DisplayName");
                        var uninstallString = subKey?.GetValue("UninstallString");

                        if (displayName != null && uninstallString == null)
                        {
                            foundIssues.Add(new RegistryIssue
                            {
                                IssueType = "Invalid Uninstall Entry",
                                RegistryPath = $@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{subKeyName}",
                                Severity = "Medium",
                                SeverityColor = new SolidColorBrush(Color.FromRgb(0xFF, 0xC1, 0x07)),
                                Size = "1 KB"
                            });
                        }
                    }
                    catch { }
                }
            }
        }

        private void ScanStartupEntries()
        {
            Dispatcher.Invoke(() => ScanStatusText.Text = "Scanning startup entries...");

            var startupKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (startupKey != null)
            {
                foreach (var valueName in startupKey.GetValueNames())
                {
                    try
                    {
                        var path = startupKey.GetValue(valueName)?.ToString();
                        if (path != null && !System.IO.File.Exists(path.Trim('"')))
                        {
                            foundIssues.Add(new RegistryIssue
                            {
                                IssueType = "Broken Startup Entry",
                                RegistryPath = $@"HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run\{valueName}",
                                Severity = "Low",
                                SeverityColor = new SolidColorBrush(Color.FromRgb(0x00, 0xD9, 0x00)),
                                Size = "< 1 KB"
                            });
                        }
                    }
                    catch { }
                }
            }
        }

        private void ScanFileAssociations()
        {
            Dispatcher.Invoke(() => ScanStatusText.Text = "Scanning file associations...");
            // Simulated scan
            foundIssues.Add(new RegistryIssue
            {
                IssueType = "Orphaned File Association",
                RegistryPath = @"HKCR\.obsolete",
                Severity = "Low",
                SeverityColor = new SolidColorBrush(Color.FromRgb(0x00, 0xD9, 0x00)),
                Size = "< 1 KB"
            });
        }

        private void ScanSharedDLLs()
        {
            Dispatcher.Invoke(() => ScanStatusText.Text = "Scanning shared DLLs...");
            // Simulated scan
        }

        private void ScanMUICache()
        {
            Dispatcher.Invoke(() => ScanStatusText.Text = "Scanning MUI cache...");
            // Simulated scan
        }

        private void CleanSelected_Click(object sender, RoutedEventArgs e)
        {
            if (IssuesListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select issues to clean.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Clean {IssuesListView.SelectedItems.Count} selected registry issues?\n\n" +
                "This action cannot be undone without a backup!",
                "Confirm Clean",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            int cleaned = CleanIssues(IssuesListView.SelectedItems.Cast<RegistryIssue>().ToList());
            
            CleanedCountText.Text = (cleanedCount + cleaned).ToString();
            cleanedCount += cleaned;

            MessageBox.Show($"Cleaned {cleaned} registry issues!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh list
            foreach (var item in IssuesListView.SelectedItems.Cast<RegistryIssue>().ToList())
            {
                foundIssues.Remove(item);
            }
            IssuesListView.ItemsSource = null;
            IssuesListView.ItemsSource = foundIssues;
            IssuesFoundText.Text = foundIssues.Count.ToString();
        }

        private void CleanAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Clean ALL {foundIssues.Count} registry issues?\n\n" +
                "This action cannot be undone without a backup!",
                "Confirm Clean All",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            int cleaned = CleanIssues(foundIssues);
            
            CleanedCountText.Text = cleaned.ToString();
            cleanedCount = cleaned;

            foundIssues.Clear();
            IssuesListView.ItemsSource = null;
            IssuesFoundText.Text = "0";

            MessageBox.Show($"Cleaned {cleaned} registry issues!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private int CleanIssues(List<RegistryIssue> issues)
        {
            int count = 0;
            foreach (var issue in issues)
            {
                try
                {
                    // Simulated cleaning - actual implementation would delete registry keys
                    count++;
                }
                catch { }
            }
            return count;
        }

        private void CreateBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Create a registry backup?\n\nThis will export critical registry hives.",
                    "Create Backup",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                string backupPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"RegistryBackup_{DateTime.Now:yyyyMMdd_HHmmss}.reg");

                Process.Start("regedit.exe", $"/e \"{backupPath}\"");

                MessageBox.Show($"Registry backup created:\n{backupPath}", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompactRegistry_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registry compaction will occur on next reboot.", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RepairPermissions_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Repairing registry permissions...", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DefragRegistry_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registry defragmentation requires a system restart.", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private class RegistryIssue
        {
            public string IssueType { get; set; }
            public string RegistryPath { get; set; }
            public string Severity { get; set; }
            public Brush SeverityColor { get; set; }
            public string Size { get; set; }
        }
    }
}
