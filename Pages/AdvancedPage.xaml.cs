using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WindowsDebloater.Pages
{
    public partial class AdvancedPage : Page
    {
        public AdvancedPage()
        {
            InitializeComponent();
        }

        private void FullOptimize_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will apply ALL optimizations:\n\n" +
                "• Disable telemetry\n• Disable Cortana\n• Optimize performance\n" +
                "• Enhance privacy\n• Network optimization\n• Visual effects optimization\n\n" +
                "Requires admin & restart. Continue?",
                "Full Optimization",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DisableTelemetryRegistry();
                    DisableCortanaRegistry();
                    DisableGameDVRRegistry();
                    OptimizeVisualsRegistry();
                    DisableLocationRegistry();
                    DisableActivityHistoryRegistry();
                    DisableAdIDRegistry();
                    
                    RunPowerShellCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                    RunPowerShellCommand("Disable-MMAgent -MemoryCompression");
                    
                    StatusTextBlock.Text = "✓ Full optimization complete! Restart for changes to take effect.";
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                }
            }
        }

        private void DisableTelemetry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableTelemetryRegistry();
                StatusTextBlock.Text = "✓ Telemetry disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableCortana_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableCortanaRegistry();
                StatusTextBlock.Text = "✓ Cortana disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableWindowsUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunPowerShellCommand("Stop-Service wuauserv; Set-Service wuauserv -StartupType Disabled");
                StatusTextBlock.Text = "✓ Windows Update disabled (set to Manual in Services to re-enable)";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableGameDVR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableGameDVRRegistry();
                StatusTextBlock.Text = "✓ Game DVR/Game Bar disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void OptimizeVisuals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OptimizeVisualsRegistry();
                StatusTextBlock.Text = "✓ Visual effects optimized for performance";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void UltimatePower_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunPowerShellCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                StatusTextBlock.Text = "✓ Ultimate Performance power plan enabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableSuperfetch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunPowerShellCommand("Stop-Service SysMain; Set-Service SysMain -StartupType Disabled");
                StatusTextBlock.Text = "✓ Superfetch/SysMain disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void OptimizeNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters"))
                {
                    key.SetValue("TcpAckFrequency", 1, RegistryValueKind.DWord);
                    key.SetValue("TCPNoDelay", 1, RegistryValueKind.DWord);
                    key.SetValue("DefaultTTL", 64, RegistryValueKind.DWord);
                }
                StatusTextBlock.Text = "✓ Network optimized for gaming/low latency";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void ClearAllTemp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c del /q /f /s %temp%\\* & del /q /f /s C:\\Windows\\Temp\\* & cleanmgr /sageset:1 & cleanmgr /sagerun:1",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                });
                StatusTextBlock.Text = "✓ Clearing all temp files and cache...";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void OptimizeSSD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunPowerShellCommand("Optimize-Volume -DriveLetter C -ReTrim -Verbose");
                StatusTextBlock.Text = "✓ SSD optimized (TRIM command sent)";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableLocationRegistry();
                StatusTextBlock.Text = "✓ Location tracking disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void DisableActivityHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableActivityHistoryRegistry();
                StatusTextBlock.Text = "✓ Activity history disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void BlockTelemetryIPs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hostsContent = "\n# Block Microsoft Telemetry\n" +
                                  "0.0.0.0 vortex.data.microsoft.com\n" +
                                  "0.0.0.0 vortex-win.data.microsoft.com\n" +
                                  "0.0.0.0 telecommand.telemetry.microsoft.com\n" +
                                  "0.0.0.0 oca.telemetry.microsoft.com\n" +
                                  "0.0.0.0 sqm.telemetry.microsoft.com\n" +
                                  "0.0.0.0 watson.telemetry.microsoft.com\n";
                
                System.IO.File.AppendAllText(@"C:\Windows\System32\drivers\etc\hosts", hostsContent);
                StatusTextBlock.Text = "✓ Telemetry IPs blocked in hosts file";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message} (needs admin)";
            }
        }

        private void DisableAdID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisableAdIDRegistry();
                StatusTextBlock.Text = "✓ Advertising ID disabled";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        // Helper methods
        private void DisableTelemetryRegistry()
        {
            using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
            {
                key.SetValue("AllowTelemetry", 0, RegistryValueKind.DWord);
            }
        }

        private void DisableCortanaRegistry()
        {
            using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Windows Search"))
            {
                key.SetValue("AllowCortana", 0, RegistryValueKind.DWord);
            }
        }

        private void DisableGameDVRRegistry()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR"))
            {
                key.SetValue("AppCaptureEnabled", 0, RegistryValueKind.DWord);
            }
            using (var key = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore"))
            {
                key.SetValue("GameDVR_Enabled", 0, RegistryValueKind.DWord);
            }
        }

        private void OptimizeVisualsRegistry()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects"))
            {
                key.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord); // Best performance
            }
        }

        private void DisableLocationRegistry()
        {
            using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\location"))
            {
                key.SetValue("Value", "Deny", RegistryValueKind.String);
            }
        }

        private void DisableActivityHistoryRegistry()
        {
            using (var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\System"))
            {
                key.SetValue("EnableActivityFeed", 0, RegistryValueKind.DWord);
                key.SetValue("PublishUserActivities", 0, RegistryValueKind.DWord);
                key.SetValue("UploadUserActivities", 0, RegistryValueKind.DWord);
            }
        }

        private void DisableAdIDRegistry()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo"))
            {
                key.SetValue("Enabled", 0, RegistryValueKind.DWord);
            }
        }

        private void RunPowerShellCommand(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command \"{command}\"",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(psi)?.WaitForExit();
        }
    }
}
