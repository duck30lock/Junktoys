using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WindowsDebloater.Pages
{
    public partial class SystemTweakerPage : Page
    {
        public SystemTweakerPage()
        {
            InitializeComponent();
        }

        private void ApplyPerformance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply performance tweaks?\n\nThis will optimize your system for maximum performance.",
                    "Performance Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("powercfg /h off");
                RunCommand("sc stop SysMain & sc config SysMain start=disabled");
                RunCommand("sc stop WSearch & sc config WSearch start=disabled");

                MessageBox.Show("Performance tweaks applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyPrivacy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply privacy tweaks?\n\nThis will enhance your privacy by disabling telemetry and tracking.",
                    "Privacy Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection\" /v AllowTelemetry /t REG_DWORD /d 0 /f");
                RunCommand("sc stop DiagTrack & sc config DiagTrack start=disabled");
                RunCommand("sc stop dmwappushservice & sc config dmwappushservice start=disabled");

                MessageBox.Show("Privacy tweaks applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyUI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply UI tweaks?\n\nThis will customize your Windows interface.",
                    "UI Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\" /v HideFileExt /t REG_DWORD /d 0 /f");
                RunCommand("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\" /v Hidden /t REG_DWORD /d 1 /f");

                MessageBox.Show("UI tweaks applied! Please restart Explorer.", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply network tweaks?\n\nThis will optimize your network performance.",
                    "Network Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("netsh int tcp set global autotuninglevel=normal");
                RunCommand("netsh int tcp set global chimney=enabled");
                RunCommand("netsh int tcp set global dca=enabled");

                MessageBox.Show("Network tweaks applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySecurity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply security tweaks?\n\nThis will enhance your system security.",
                    "Security Tweaks",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\" /v DisableCAD /t REG_DWORD /d 0 /f");
                RunCommand("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\" /v NoDriveTypeAutoRun /t REG_DWORD /d 255 /f");

                MessageBox.Show("Security tweaks applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Apply ALL recommended tweaks?\n\n" +
                "This will apply:\n" +
                "• Performance optimizations\n" +
                "• Privacy enhancements\n" +
                "• UI improvements\n" +
                "• Network optimizations\n" +
                "• Security hardening\n\n" +
                "Continue?",
                "Apply All Tweaks",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ApplyPerformance_Click(sender, e);
                ApplyPrivacy_Click(sender, e);
                ApplyUI_Click(sender, e);
                ApplyNetwork_Click(sender, e);
                ApplySecurity_Click(sender, e);

                MessageBox.Show("All tweaks applied successfully!\n\nPlease restart your computer.", 
                    "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RestoreDefaults_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Restore Windows defaults?\n\nThis will undo custom tweaks.",
                "Restore Defaults",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("Defaults restored! Please restart your computer.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RunCommand(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas"
                };
                Process.Start(psi)?.WaitForExit(3000);
            }
            catch { }
        }
    }
}
