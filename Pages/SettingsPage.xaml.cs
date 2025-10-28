using ModernWpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WindowsDebloater.Pages
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            
            // Set current theme on load
            if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Dark)
            {
                DarkThemeRadio.IsChecked = true;
            }
            else
            {
                LightThemeRadio.IsChecked = true;
            }
        }

        private void LightTheme_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            StatusTextBlock.Text = "Switched to light theme";
        }

        private void DarkTheme_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            StatusTextBlock.Text = "Switched to dark theme";
        }

        private void CreateRestorePoint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will create a system restore point. This may take several minutes. Continue?",
                    "Create Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-Command \"Checkpoint-Computer -Description 'Junktoys' -RestorePointType 'MODIFY_SETTINGS'\"",
                        Verb = "runas",
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                    StatusTextBlock.Text = "Creating system restore point...";
                }
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void OpenTaskManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("taskmgr.exe");
                StatusTextBlock.Text = "Opened Task Manager";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will clear temporary files and cache. Continue?",
                "Clear Cache",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "cleanmgr.exe",
                        UseShellExecute = true
                    };

                    Process.Start(startInfo);
                    StatusTextBlock.Text = "Launched Disk Cleanup utility";
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                }
            }
        }
    }
}
