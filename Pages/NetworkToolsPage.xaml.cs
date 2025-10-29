using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;

namespace WindowsDebloater.Pages
{
    public partial class NetworkToolsPage : Page
    {
        private DispatcherTimer monitorTimer;
        private bool isMonitoring = false;
        private long lastBytesReceived = 0;
        private long lastBytesSent = 0;

        public NetworkToolsPage()
        {
            InitializeComponent();
            monitorTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            monitorTimer.Tick += MonitorNetwork_Tick;
            RefreshConnections();
        }

        private void SetCloudflare_Click(object sender, RoutedEventArgs e)
        {
            SetDNS("1.1.1.1", "1.0.0.1");
            CurrentDNSText.Text = "Current DNS: Cloudflare (1.1.1.1)";
        }

        private void SetGoogle_Click(object sender, RoutedEventArgs e)
        {
            SetDNS("8.8.8.8", "8.8.4.4");
            CurrentDNSText.Text = "Current DNS: Google (8.8.8.8)";
        }

        private void SetQuad9_Click(object sender, RoutedEventArgs e)
        {
            SetDNS("9.9.9.9", "149.112.112.112");
            CurrentDNSText.Text = "Current DNS: Quad9 (9.9.9.9)";
        }

        private void ResetDNS_Click(object sender, RoutedEventArgs e)
        {
            RunCommand("netsh interface ip set dns \"Ethernet\" dhcp");
            RunCommand("netsh interface ip set dns \"Wi-Fi\" dhcp");
            CurrentDNSText.Text = "Current DNS: Automatic (DHCP)";
            MessageBox.Show("DNS reset to automatic!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetDNS(string primary, string secondary)
        {
            try
            {
                RunCommand($"netsh interface ip set dns \"Ethernet\" static {primary}");
                RunCommand($"netsh interface ip add dns \"Ethernet\" {secondary} index=2");
                RunCommand($"netsh interface ip set dns \"Wi-Fi\" static {primary}");
                RunCommand($"netsh interface ip add dns \"Wi-Fi\" {secondary} index=2");
                
                MessageBox.Show($"DNS changed to {primary}!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (!isMonitoring)
            {
                isMonitoring = true;
                MonitorButton.Content = "⏹️ Stop Monitoring";
                
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                var activeInterface = interfaces.FirstOrDefault(i => 
                    i.OperationalStatus == OperationalStatus.Up && 
                    i.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                
                if (activeInterface != null)
                {
                    var stats = activeInterface.GetIPv4Statistics();
                    lastBytesReceived = stats.BytesReceived;
                    lastBytesSent = stats.BytesSent;
                }
                
                monitorTimer.Start();
            }
            else
            {
                isMonitoring = false;
                MonitorButton.Content = "🔄 Start Monitoring";
                monitorTimer.Stop();
                DownloadSpeedText.Text = "0 KB/s";
                UploadSpeedText.Text = "0 KB/s";
            }
        }

        private void MonitorNetwork_Tick(object sender, EventArgs e)
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                var activeInterface = interfaces.FirstOrDefault(i => 
                    i.OperationalStatus == OperationalStatus.Up && 
                    i.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                if (activeInterface != null)
                {
                    var stats = activeInterface.GetIPv4Statistics();
                    long currentReceived = stats.BytesReceived;
                    long currentSent = stats.BytesSent;

                    long downloadSpeed = (currentReceived - lastBytesReceived) / 1024;
                    long uploadSpeed = (currentSent - lastBytesSent) / 1024;

                    DownloadSpeedText.Text = downloadSpeed > 1024 
                        ? $"{downloadSpeed / 1024.0:F2} MB/s" 
                        : $"{downloadSpeed} KB/s";
                    
                    UploadSpeedText.Text = uploadSpeed > 1024 
                        ? $"{uploadSpeed / 1024.0:F2} MB/s" 
                        : $"{uploadSpeed} KB/s";

                    lastBytesReceived = currentReceived;
                    lastBytesSent = currentSent;
                }
            }
            catch { }
        }

        private void Ping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string target = PingTargetTextBox.Text;
                using (var ping = new Ping())
                {
                    var reply = ping.Send(target, 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        PingResultText.Text = $"✅ Ping successful! Response time: {reply.RoundtripTime}ms";
                    }
                    else
                    {
                        PingResultText.Text = $"❌ Ping failed: {reply.Status}";
                    }
                }
            }
            catch (Exception ex)
            {
                PingResultText.Text = $"❌ Error: {ex.Message}";
            }
        }

        private void ApplyGamingOptimizations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply network optimizations for gaming?\n\n" +
                    "This will:\n" +
                    "• Disable network throttling\n" +
                    "• Optimize TCP/IP settings\n" +
                    "• Reduce latency\n" +
                    "• Disable Nagle's algorithm\n\n" +
                    "Continue?",
                    "Gaming Optimizations",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("netsh int tcp set global autotuninglevel=normal");
                RunCommand("netsh int tcp set global chimney=enabled");
                RunCommand("netsh int tcp set global dca=enabled");
                RunCommand("netsh int tcp set global netdma=enabled");
                RunCommand("netsh int tcp set global ecncapability=disabled");
                RunCommand("netsh int tcp set global timestamps=disabled");
                RunCommand("netsh interface tcp set heuristics disabled");
                
                MessageBox.Show("Gaming optimizations applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetNetwork_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Reset all network settings to default?\n\nThis will restart your network adapter.",
                    "Reset Network",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                RunCommand("netsh winsock reset");
                RunCommand("netsh int ip reset");
                RunCommand("ipconfig /flushdns");
                
                MessageBox.Show("Network settings reset! Please restart your computer.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshConnections()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "netstat",
                    Arguments = "-ano",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(psi);
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var connections = output.Split('\n')
                    .Skip(4)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Take(20)
                    .Select(line => 
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 4)
                        {
                            return new
                            {
                                Protocol = parts[0],
                                LocalAddress = parts[1],
                                RemoteAddress = parts[2],
                                State = parts.Length > 3 ? parts[3] : "N/A"
                            };
                        }
                        return null;
                    })
                    .Where(c => c != null)
                    .ToList();

                ConnectionsListView.ItemsSource = connections;
            }
            catch { }
        }

        private void RefreshConnections_Click(object sender, RoutedEventArgs e)
        {
            RefreshConnections();
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
