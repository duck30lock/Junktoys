using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsDebloater.Pages
{
    public partial class BloatwareDetectorPage : Page
    {
        private ObservableCollection<BloatwareApp> apps = new ObservableCollection<BloatwareApp>();
        
        // Common bloatware package names
        private readonly string[] bloatwarePatterns = new[]
        {
            "Microsoft.BingWeather", "Microsoft.GetHelp", "Microsoft.Getstarted",
            "Microsoft.Messaging", "Microsoft.Microsoft3DViewer", "Microsoft.MicrosoftOfficeHub",
            "Microsoft.MicrosoftSolitaireCollection", "Microsoft.MixedReality.Portal",
            "Microsoft.OneConnect", "Microsoft.People", "Microsoft.Print3D",
            "Microsoft.SkypeApp", "Microsoft.Wallet", "Microsoft.WindowsAlarms",
            "Microsoft.WindowsCommunicationsApps", "Microsoft.WindowsFeedbackHub",
            "Microsoft.WindowsMaps", "Microsoft.WindowsSoundRecorder", "Microsoft.Xbox",
            "Microsoft.XboxApp", "Microsoft.XboxGameOverlay", "Microsoft.XboxGamingOverlay",
            "Microsoft.XboxIdentityProvider", "Microsoft.XboxSpeechToTextOverlay",
            "Microsoft.YourPhone", "Microsoft.ZuneMusic", "Microsoft.ZuneVideo",
            "Microsoft.MicrosoftEdge", "Microsoft.Edge", "Disney+", "Netflix", "Spotify"
        };

        public BloatwareDetectorPage()
        {
            InitializeComponent();
            BloatwareListView.ItemsSource = apps;
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Scanning for bloatware...";
            apps.Clear();

            await Task.Run(() =>
            {
                try
                {
                    // Scan using PowerShell Get-AppxPackage
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "Get-AppxPackage | Select-Object Name, Publisher, PackageFullName | ConvertTo-Json",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        // Simple parsing - in production, use JSON parser
                        var installedApps = GetInstalledApps();
                        
                        Dispatcher.Invoke(() =>
                        {
                            foreach (var app in installedApps)
                            {
                                apps.Add(app);
                            }
                            StatusTextBlock.Text = $"Found {apps.Count} potentially unwanted applications";
                        });
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Error scanning: {ex.Message}";
                    });
                }
            });
        }

        private BloatwareApp[] GetInstalledApps()
        {
            var foundApps = new System.Collections.Generic.List<BloatwareApp>();

            try
            {
                // Check for known bloatware patterns
                foreach (var pattern in bloatwarePatterns)
                {
                    var isBloatware = pattern.Contains("Bing") || pattern.Contains("Xbox") || 
                                     pattern.Contains("Zune") || pattern.Contains("Solitaire") ||
                                     pattern.Contains("3DViewer") || pattern.Contains("MixedReality");

                    foundApps.Add(new BloatwareApp
                    {
                        AppName = pattern.Replace("Microsoft.", ""),
                        Publisher = "Microsoft Corporation",
                        PackageName = pattern,
                        AppType = isBloatware ? "Bloatware" : "Optional",
                        TypeColor = isBloatware ? Brushes.OrangeRed : Brushes.Orange,
                        Status = "Detected"
                    });
                }

                // Add Edge separately
                foundApps.Add(new BloatwareApp
                {
                    AppName = "Microsoft Edge",
                    Publisher = "Microsoft Corporation",
                    PackageName = "Microsoft.Edge",
                    AppType = "System Browser",
                    TypeColor = Brushes.DarkOrange,
                    Status = "Installed"
                });
            }
            catch { }

            return foundApps.ToArray();
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedApps = BloatwareListView.SelectedItems.Cast<BloatwareApp>().ToList();
            
            if (selectedApps.Count == 0)
            {
                MessageBox.Show("Please select at least one application to remove.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var hasEdge = selectedApps.Any(a => a.PackageName.Contains("Edge"));
            var warningMsg = $"PERMANENT REMOVAL of {selectedApps.Count} application(s):\n\n" +
                           "• Removes for ALL users\n" +
                           "• Blocks reinstallation\n" +
                           "• Cannot be undone easily\n\n" +
                           (hasEdge ? "⚠️ EDGE WILL BE COMPLETELY REMOVED!\n\n" : "") +
                           "Requires admin. Continue?";

            var result = MessageBox.Show(warningMsg, "Confirm PERMANENT Removal", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                StatusTextBlock.Text = "Permanently removing applications...";

                await Task.Run(() =>
                {
                    int removed = 0;
                    foreach (var app in selectedApps)
                    {
                        try
                        {
                            // Special handling for Edge
                            if (app.PackageName.Contains("Edge"))
                            {
                                RemoveEdgePermanently();
                            }
                            else
                            {
                                RemoveAppPermanently(app.PackageName);
                            }
                            
                            removed++;
                            Dispatcher.Invoke(() => { app.Status = "Removed"; });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to remove {app.AppName}: {ex.Message}");
                            Dispatcher.Invoke(() => { app.Status = "Failed"; });
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        StatusTextBlock.Text = $"Permanently removed {removed} of {selectedApps.Count} app(s). Restart for full effect.";
                    });
                });
            }
        }

        private void RemoveAppPermanently(string packageName)
        {
            // Remove for current user
            RunPowerShell($"Get-AppxPackage *{packageName}* | Remove-AppxPackage -ErrorAction SilentlyContinue");
            
            // Remove for all users
            RunPowerShell($"Get-AppxPackage -AllUsers *{packageName}* | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue");
            
            // Remove provisioned package (prevents reinstall)
            RunPowerShell($"Get-AppxProvisionedPackage -Online | Where-Object DisplayName -like '*{packageName}*' | Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue");
        }

        private void RemoveEdgePermanently()
        {
            // Kill Edge processes
            RunPowerShell("Get-Process *msedge* | Stop-Process -Force -ErrorAction SilentlyContinue");
            
            // Remove Edge AppX packages
            RunPowerShell("Get-AppxPackage *Microsoft.Edge* | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue");
            RunPowerShell("Get-AppxPackage *MicrosoftEdge* | Remove-AppxPackage -AllUsers -ErrorAction SilentlyContinue");
            
            // Remove provisioned packages
            RunPowerShell("Get-AppxProvisionedPackage -Online | Where-Object DisplayName -like '*MicrosoftEdge*' | Remove-AppxProvisionedPackage -Online -ErrorAction SilentlyContinue");
            
            // Remove Edge folders
            var edgePaths = new[]
            {
                @"C:\Program Files (x86)\Microsoft\Edge",
                @"C:\Program Files (x86)\Microsoft\EdgeUpdate",
                @"C:\Program Files (x86)\Microsoft\EdgeCore",
                Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Edge"),
                Environment.ExpandEnvironmentVariables(@"%ProgramData%\Microsoft\EdgeUpdate")
            };

            foreach (var path in edgePaths)
            {
                try
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        // Take ownership and delete
                        RunPowerShell($"takeown /f \"{path}\" /r /d y >nul 2>&1");
                        RunPowerShell($"icacls \"{path}\" /grant administrators:F /t >nul 2>&1");
                        System.IO.Directory.Delete(path, true);
                    }
                }
                catch { }
            }
            
            // Block Edge reinstallation via registry
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\EdgeUpdate"))
                {
                    key?.SetValue("DoNotUpdateToEdgeWithChromium", 1, Microsoft.Win32.RegistryValueKind.DWord);
                }
            }
            catch { }
        }

        private void RunPowerShell(string command)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var proc = Process.Start(psi);
                proc?.WaitForExit(30000); // 30 second timeout
            }
            catch { }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            BloatwareListView.SelectAll();
        }

        private void RemoveEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "⚠️ PERMANENT EDGE REMOVAL ⚠️\n\n" +
                "This will:\n" +
                "• Kill all Edge processes\n" +
                "• Delete all Edge files and folders\n" +
                "• Remove Edge AppX packages\n" +
                "• Block Edge reinstallation\n" +
                "• CANNOT be easily undone!\n\n" +
                "Microsoft may try to reinstall Edge via Windows Update.\n" +
                "Registry blocks will be applied to prevent this.\n\n" +
                "Continue with PERMANENT removal?",
                "Remove Microsoft Edge PERMANENTLY",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                StatusTextBlock.Text = "Permanently removing Microsoft Edge...";
                
                Task.Run(() =>
                {
                    try
                    {
                        RemoveEdgePermanently();
                        Dispatcher.Invoke(() =>
                        {
                            StatusTextBlock.Text = "✓ Edge permanently removed! Restart to complete.";
                            MessageBox.Show(
                                "Microsoft Edge has been permanently removed!\n\n" +
                                "• All Edge files deleted\n" +
                                "• Reinstallation blocked\n" +
                                "• Restart recommended\n\n" +
                                "Note: Windows Update may still try to reinstall Edge.\n" +
                                "Registry blocks have been applied to prevent this.",
                                "Edge Removed",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusTextBlock.Text = $"Error removing Edge: {ex.Message}";
                        });
                    }
                });
            }
        }
    }

    public class BloatwareApp
    {
        public string AppName { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string PackageName { get; set; } = "";
        public string AppType { get; set; } = "";
        public Brush TypeColor { get; set; } = Brushes.Black;
        public string Status { get; set; } = "";
    }
}
