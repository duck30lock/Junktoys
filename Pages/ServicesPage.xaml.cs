using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsDebloater.Pages
{
    public partial class ServicesPage : Page
    {
        private ObservableCollection<ServiceInfo> services = new ObservableCollection<ServiceInfo>();

        // Services that can be safely disabled for debloating
        private readonly string[] optimizableServices = new[]
        {
            "DiagTrack", // Connected User Experiences and Telemetry
            "dmwappushservice", // WAP Push Message Routing Service
            "RetailDemo", // Retail Demo Service
            "RemoteRegistry", // Remote Registry
            "XblAuthManager", // Xbox Live Auth Manager
            "XblGameSave", // Xbox Live Game Save
            "XboxNetApiSvc", // Xbox Live Networking Service
            "WSearch", // Windows Search (if not needed)
            "SysMain", // Superfetch
            "MapsBroker" // Downloaded Maps Manager
        };

        public ServicesPage()
        {
            InitializeComponent();
            ServicesListView.ItemsSource = services;
            LoadServices();
        }

        private void LoadServices()
        {
            StatusTextBlock.Text = "Loading optimizable services...";
            services.Clear();

            // Load predefined list of services
            foreach (var serviceName in optimizableServices)
            {
                var recommendation = GetRecommendation(serviceName);
                services.Add(new ServiceInfo
                {
                    ServiceName = serviceName,
                    DisplayName = GetDisplayName(serviceName),
                    Status = "Unknown",
                    StatusColor = Brushes.Gray,
                    Recommendation = recommendation,
                    RecommendationColor = recommendation == "Can Disable" ? Brushes.Orange : Brushes.Green
                });
            }
            
            StatusTextBlock.Text = $"Loaded {services.Count} optimizable services";
        }

        private string GetDisplayName(string serviceName)
        {
            return serviceName switch
            {
                "DiagTrack" => "Connected User Experiences and Telemetry",
                "dmwappushservice" => "WAP Push Message Routing Service",
                "RetailDemo" => "Retail Demo Service",
                "RemoteRegistry" => "Remote Registry",
                "XblAuthManager" => "Xbox Live Auth Manager",
                "XblGameSave" => "Xbox Live Game Save",
                "XboxNetApiSvc" => "Xbox Live Networking Service",
                "WSearch" => "Windows Search",
                "SysMain" => "Superfetch",
                "MapsBroker" => "Downloaded Maps Manager",
                _ => serviceName
            };
        }

        private string GetRecommendation(string serviceName)
        {
            return serviceName switch
            {
                "DiagTrack" => "Can Disable",
                "dmwappushservice" => "Can Disable",
                "RetailDemo" => "Can Disable",
                "XblAuthManager" => "Can Disable",
                "XblGameSave" => "Can Disable",
                "XboxNetApiSvc" => "Can Disable",
                _ => "Optional"
            };
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }

        private void OptimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedServices = ServicesListView.SelectedItems.Cast<ServiceInfo>().ToList();
            
            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Please select at least one service to optimize.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"This will attempt to stop and disable {selectedServices.Count} service(s).\n\n" +
                "This requires administrator privileges. Continue?", 
                "Confirm Optimization", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (var serviceInfo in selectedServices)
                    {
                        // Use PowerShell to stop and disable services
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = $"-Command \"Stop-Service -Name '{serviceInfo.ServiceName}' -Force; Set-Service -Name '{serviceInfo.ServiceName}' -StartupType Disabled\"",
                            Verb = "runas",
                            UseShellExecute = true,
                            CreateNoWindow = true
                        };

                        Process.Start(startInfo);
                        serviceInfo.Status = "Disabled";
                        serviceInfo.StatusColor = Brushes.Gray;
                    }
                    
                    StatusTextBlock.Text = $"Sent optimize commands for {selectedServices.Count} service(s)";
                }
                catch (Exception ex)
                {
                    StatusTextBlock.Text = $"Error: {ex.Message}";
                }
            }
        }
    }

    public class ServiceInfo
    {
        public string ServiceName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Status { get; set; } = "";
        public Brush StatusColor { get; set; } = Brushes.Black;
        public string Recommendation { get; set; } = "";
        public Brush RecommendationColor { get; set; } = Brushes.Black;
    }
}
