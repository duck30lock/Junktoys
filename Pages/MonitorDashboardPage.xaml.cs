using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class MonitorDashboardPage : Page
    {
        private DispatcherTimer updateTimer;
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;

        public MonitorDashboardPage()
        {
            InitializeComponent();
            
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            }
            catch { }

            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            updateTimer.Tick += (s, e) => UpdateStats();
            updateTimer.Start();

            LoadSystemInfo();
            UpdateStats();
        }

        private void LoadSystemInfo()
        {
            try
            {
                // CPU Info
                using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        CPUInfoText.Text = item["Name"].ToString();
                        break;
                    }
                }

                // RAM Info
                double totalGB = GetTotalPhysicalMemory() / 1024.0 / 1024.0 / 1024.0;
                RAMInfoText.Text = $"{totalGB:F1} GB";

                // GPU Info
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (var item in searcher.Get())
                    {
                        GPUInfoText.Text = item["Name"].ToString();
                        break;
                    }
                }

                // Motherboard
                using (var searcher = new ManagementObjectSearcher("select * from Win32_BaseBoard"))
                {
                    foreach (var item in searcher.Get())
                    {
                        MotherboardText.Text = $"{item["Manufacturer"]} {item["Product"]}";
                        break;
                    }
                }

                // Uptime
                var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
                UptimeText.Text = $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
            }
            catch { }
        }

        private void UpdateStats()
        {
            try
            {
                // CPU Usage
                float cpuUsage = cpuCounter?.NextValue() ?? 0;
                CPUUsageText.Text = $"{cpuUsage:F0}%";
                CPUProgressBar.Value = cpuUsage;

                // RAM Usage
                float ramUsage = ramCounter?.NextValue() ?? 0;
                RAMUsageText.Text = $"{ramUsage:F0}%";
                RAMProgressBar.Value = ramUsage;

                // GPU Usage (simulated - requires specific hardware APIs)
                GPUUsageText.Text = "N/A";
                GPUProgressBar.Value = 0;

                // Disk Usage
                var drive = System.IO.DriveInfo.GetDrives().FirstOrDefault(d => d.Name == "C:\\");
                if (drive != null)
                {
                    double used = (drive.TotalSize - drive.AvailableFreeSpace) * 100.0 / drive.TotalSize;
                    DiskUsageText.Text = $"{used:F0}%";
                    DiskProgressBar.Value = used;
                }

                // Temperature (requires WMI or specific libraries)
                CPUTempText.Text = "N/A";
                GPUTempText.Text = "N/A";

                // Top Processes
                var processes = Process.GetProcesses()
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(10)
                    .Select(p => new
                    {
                        ProcessName = p.ProcessName,
                        CpuUsage = "N/A",
                        MemoryMB = (p.WorkingSet64 / 1024 / 1024).ToString("N0"),
                        DiskUsage = "N/A",
                        ProcessId = p.Id
                    })
                    .ToList();

                ProcessListView.ItemsSource = processes;
            }
            catch { }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadSystemInfo();
            UpdateStats();
        }

        private long GetTotalPhysicalMemory()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        return Convert.ToInt64(item["TotalPhysicalMemory"]);
                    }
                }
            }
            catch { }
            return 16L * 1024 * 1024 * 1024;
        }
    }
}
