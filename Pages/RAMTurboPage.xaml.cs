using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class RAMTurboPage : Page
    {
        private DispatcherTimer updateTimer;
        private DispatcherTimer autoClearTimer;
        private long totalMemory;

        [DllImport("psapi.dll")]
        static extern bool EmptyWorkingSet(IntPtr hProcess);

        public RAMTurboPage()
        {
            InitializeComponent();
            
            totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
            
            updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            updateTimer.Tick += (s, e) => UpdateRAMStats();
            updateTimer.Start();

            autoClearTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            autoClearTimer.Tick += AutoClear_Tick;

            UpdateRAMStats();
        }

        private void UpdateRAMStats()
        {
            try
            {
                var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
                
                long totalBytes = (long)info.TotalPhysicalMemory;
                long availableBytes = (long)info.AvailablePhysicalMemory;
                long usedBytes = totalBytes - availableBytes;

                double totalGB = totalBytes / 1024.0 / 1024.0 / 1024.0;
                double usedGB = usedBytes / 1024.0 / 1024.0 / 1024.0;
                double availableGB = availableBytes / 1024.0 / 1024.0 / 1024.0;
                double usedPercent = (usedBytes * 100.0) / totalBytes;

                TotalRAMText.Text = $"{totalGB:F1} GB";
                UsedRAMText.Text = $"{usedGB:F1} GB";
                AvailableRAMText.Text = $"{availableGB:F1} GB";
                RAMProgressBar.Value = usedPercent;
                RAMPercentText.Text = $"{usedPercent:F0}% Used";

                // Update top memory consumers
                var processes = Process.GetProcesses()
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(10)
                    .Select(p => new
                    {
                        ProcessName = p.ProcessName,
                        MemoryMB = (p.WorkingSet64 / 1024 / 1024).ToString("N0"),
                        Percentage = $"{(p.WorkingSet64 * 100.0 / totalBytes):F1}%",
                        ProcessId = p.Id
                    })
                    .ToList();

                MemoryListView.ItemsSource = processes;
            }
            catch { }
        }

        private void ClearRAM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will aggressively clear RAM by:\n\n" +
                    "• Emptying working sets\n" +
                    "• Forcing garbage collection\n" +
                    "• Clearing standby memory\n\n" +
                    "Continue?",
                    "Clear RAM",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Empty working sets
                foreach (var process in Process.GetProcesses())
                {
                    try
                    {
                        EmptyWorkingSet(process.Handle);
                    }
                    catch { }
                }

                // Force GC
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Empty standby list
                RunCommand("RAMMap.exe -Ew");

                UpdateRAMStats();

                MessageBox.Show("RAM cleared successfully!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmptyStandby_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand("EmptyStandbyList.exe standbylist");
                UpdateRAMStats();
                MessageBox.Show("Standby list emptied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GarbageCollect_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
            GC.WaitForPendingFinalizers();
            GC.Collect();
            
            UpdateRAMStats();
            
            MessageBox.Show("Garbage collection completed!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AutoClear_Changed(object sender, RoutedEventArgs e)
        {
            if (AutoClearCheckBox.IsChecked == true)
            {
                if (AggressiveModeCheckBox.IsChecked == true)
                {
                    autoClearTimer.Interval = TimeSpan.FromSeconds(30);
                }
                autoClearTimer.Start();
            }
            else
            {
                autoClearTimer.Stop();
            }
        }

        private void AutoClear_Tick(object sender, EventArgs e)
        {
            try
            {
                var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
                long totalBytes = (long)info.TotalPhysicalMemory;
                long availableBytes = (long)info.AvailablePhysicalMemory;
                long usedBytes = totalBytes - availableBytes;
                double usedPercent = (usedBytes * 100.0) / totalBytes;

                if (usedPercent >= ThresholdSlider.Value)
                {
                    // Auto clear
                    foreach (var process in Process.GetProcesses())
                    {
                        try { EmptyWorkingSet(process.Handle); } catch { }
                    }
                    GC.Collect();
                }
            }
            catch { }
        }

        private void ThresholdSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ThresholdText != null)
            {
                ThresholdText.Text = $"{ThresholdSlider.Value:F0}%";
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateRAMStats();
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
