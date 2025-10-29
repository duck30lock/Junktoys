using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WindowsDebloater.Pages
{
    public partial class DiskAnalyzerPage : Page
    {
        public DiskAnalyzerPage()
        {
            InitializeComponent();
            LoadDrives();
        }

        private void LoadDrives()
        {
            var drives = DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                .Select(d => d.Name)
                .ToList();

            DriveComboBox.ItemsSource = drives;
            if (drives.Count > 0)
                DriveComboBox.SelectedIndex = 0;
        }

        private void DriveComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDriveStats();
        }

        private void UpdateDriveStats()
        {
            if (DriveComboBox.SelectedItem == null) return;

            try
            {
                var driveName = DriveComboBox.SelectedItem.ToString();
                var drive = new DriveInfo(driveName);

                long totalBytes = drive.TotalSize;
                long freeBytes = drive.AvailableFreeSpace;
                long usedBytes = totalBytes - freeBytes;

                TotalSpaceText.Text = $"{totalBytes / 1024.0 / 1024.0 / 1024.0:F1} GB";
                UsedSpaceText.Text = $"{usedBytes / 1024.0 / 1024.0 / 1024.0:F1} GB";
                FreeSpaceText.Text = $"{freeBytes / 1024.0 / 1024.0 / 1024.0:F1} GB";

                double usedPercent = (usedBytes * 100.0) / totalBytes;
                UsageProgressBar.Value = usedPercent;
                UsagePercentText.Text = $"{usedPercent:F1}% Used";
            }
            catch { }
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            if (DriveComboBox.SelectedItem == null) return;

            try
            {
                var driveName = DriveComboBox.SelectedItem.ToString();
                var folders = new List<FolderInfo>();

                // Common large folders
                var pathsToCheck = new[]
                {
                    Path.Combine(driveName, "Windows"),
                    Path.Combine(driveName, "Program Files"),
                    Path.Combine(driveName, "Program Files (x86)"),
                    Path.Combine(driveName, "Users"),
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
                };

                var drive = new DriveInfo(driveName);
                long totalBytes = drive.TotalSize;

                foreach (var path in pathsToCheck)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            long size = GetDirectorySize(path);
                            double percentage = (size * 100.0) / totalBytes;

                            folders.Add(new FolderInfo
                            {
                                Path = path,
                                Size = FormatBytes(size),
                                Percentage = $"{percentage:F2}%"
                            });
                        }
                    }
                    catch { }
                }

                FoldersListView.ItemsSource = folders.OrderByDescending(f => 
                    ParseSize(f.Size)).Take(20).ToList();

                MessageBox.Show("Disk analysis complete!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error analyzing disk: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private long GetDirectorySize(string path)
        {
            try
            {
                var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
                return files.Sum(f => new FileInfo(f).Length);
            }
            catch
            {
                return 0;
            }
        }

        private string FormatBytes(long bytes)
        {
            if (bytes >= 1024L * 1024 * 1024)
                return $"{bytes / 1024.0 / 1024.0 / 1024.0:F2} GB";
            if (bytes >= 1024L * 1024)
                return $"{bytes / 1024.0 / 1024.0:F2} MB";
            if (bytes >= 1024L)
                return $"{bytes / 1024.0:F2} KB";
            return $"{bytes} B";
        }

        private long ParseSize(string sizeStr)
        {
            var parts = sizeStr.Split(' ');
            if (parts.Length != 2) return 0;

            if (!double.TryParse(parts[0], out double value)) return 0;

            return parts[1] switch
            {
                "GB" => (long)(value * 1024 * 1024 * 1024),
                "MB" => (long)(value * 1024 * 1024),
                "KB" => (long)(value * 1024),
                _ => (long)value
            };
        }

        private void EmptyRecycleBin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("cmd.exe", "/c rd /s /q C:\\$Recycle.Bin");
                MessageBox.Show("Recycle bin emptied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearTemp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("cmd.exe", "/c del /q /f /s %temp%\\* 2>nul");
                MessageBox.Show("Temp files cleared!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearDownloads_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Clear all files from Downloads folder?\n\nThis cannot be undone!",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    foreach (var file in Directory.GetFiles(downloads))
                    {
                        try { File.Delete(file); } catch { }
                    }
                    MessageBox.Show("Downloads folder cleared!", "Success", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearBrowserCache_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Browser cache clearing coming soon!", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private class FolderInfo
        {
            public string Path { get; set; }
            public string Size { get; set; }
            public string Percentage { get; set; }
        }
    }
}
