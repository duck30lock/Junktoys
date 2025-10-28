using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class BackgroundAppsPage : Page
    {
        private ObservableCollection<ProcessInfo> processes = new ObservableCollection<ProcessInfo>();
        private DispatcherTimer autoKillTimer;

        public BackgroundAppsPage()
        {
            InitializeComponent();
            ProcessListView.ItemsSource = processes;
            autoKillTimer = new DispatcherTimer();
            autoKillTimer.Interval = TimeSpan.FromSeconds(5);
            autoKillTimer.Tick += AutoKillTimer_Tick;
            
            LoadProcesses();
        }

        private async void LoadProcesses()
        {
            await Task.Run(() =>
            {
                var processList = Process.GetProcesses()
                    .Where(p => !string.IsNullOrEmpty(p.MainWindowTitle) || p.WorkingSet64 > 50 * 1024 * 1024)
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(50)
                    .Select(p => new ProcessInfo
                    {
                        ProcessName = p.ProcessName,
                        ProcessId = p.Id,
                        MemoryMB = Math.Round(p.WorkingSet64 / 1024.0 / 1024.0, 2),
                        Status = p.WorkingSet64 > 500 * 1024 * 1024 ? "High Memory" : "Normal",
                        StatusColor = p.WorkingSet64 > 500 * 1024 * 1024 ? Brushes.OrangeRed : Brushes.Green
                    })
                    .ToList();

                Dispatcher.Invoke(() =>
                {
                    processes.Clear();
                    foreach (var proc in processList)
                    {
                        processes.Add(proc);
                    }
                    StatusTextBlock.Text = $"Loaded {processes.Count} background processes";
                });
            });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProcesses();
        }

        private void KillSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProcesses = ProcessListView.SelectedItems.Cast<ProcessInfo>().ToList();
            
            if (selectedProcesses.Count == 0)
            {
                MessageBox.Show("Please select at least one process to kill.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to kill {selectedProcesses.Count} process(es)?", 
                "Confirm Kill", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                int killed = 0;
                foreach (var procInfo in selectedProcesses)
                {
                    try
                    {
                        var process = Process.GetProcessById(procInfo.ProcessId);
                        process.Kill();
                        killed++;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to kill {procInfo.ProcessName}: {ex.Message}");
                    }
                }

                StatusTextBlock.Text = $"Killed {killed} process(es)";
                Task.Delay(1000).ContinueWith(_ => Dispatcher.Invoke(LoadProcesses));
            }
        }

        private void AutoKillCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (AutoKillCheckBox.IsChecked == true)
            {
                autoKillTimer.Start();
                StatusTextBlock.Text = "Auto-kill enabled - monitoring every 5 seconds";
            }
            else
            {
                autoKillTimer.Stop();
                StatusTextBlock.Text = "Auto-kill disabled";
            }
        }

        private void AutoKillTimer_Tick(object? sender, EventArgs e)
        {
            var highMemoryProcesses = processes
                .Where(p => p.MemoryMB > 500)
                .ToList();

            foreach (var procInfo in highMemoryProcesses)
            {
                try
                {
                    var process = Process.GetProcessById(procInfo.ProcessId);
                    // Skip critical system processes
                    if (!IsCriticalProcess(process.ProcessName))
                    {
                        process.Kill();
                        Debug.WriteLine($"Auto-killed: {procInfo.ProcessName}");
                    }
                }
                catch { }
            }

            if (highMemoryProcesses.Count > 0)
            {
                LoadProcesses();
            }
        }

        private bool IsCriticalProcess(string processName)
        {
            string[] criticalProcesses = { "System", "csrss", "smss", "services", "lsass", 
                "winlogon", "explorer", "dwm", "svchost" };
            return criticalProcesses.Any(p => processName.Equals(p, StringComparison.OrdinalIgnoreCase));
        }
    }

    public class ProcessInfo
    {
        public string ProcessName { get; set; } = "";
        public int ProcessId { get; set; }
        public double MemoryMB { get; set; }
        public double CpuPercent { get; set; }
        public string Status { get; set; } = "";
        public Brush StatusColor { get; set; } = Brushes.Black;
    }
}
