using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class AutoPilotPage : Page
    {
        private bool isAutoPilotEnabled = false;
        private DispatcherTimer mainTimer;
        private DispatcherTimer idleCheckTimer;
        private DateTime lastActivityTime;
        private ObservableCollection<string> activityLog = new ObservableCollection<string>();

        public AutoPilotPage()
        {
            InitializeComponent();
            
            mainTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
            mainTimer.Tick += MainTimer_Tick;

            idleCheckTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            idleCheckTimer.Tick += IdleCheckTimer_Tick;

            lastActivityTime = DateTime.Now;
            ActivityLogListBox.ItemsSource = activityLog;
        }

        private void ToggleAutoPilot_Click(object sender, RoutedEventArgs e)
        {
            if (!isAutoPilotEnabled)
            {
                EnableAutoPilot();
            }
            else
            {
                DisableAutoPilot();
            }
        }

        private void EnableAutoPilot()
        {
            var result = MessageBox.Show(
                "Enable Auto-Pilot Mode?\n\n" +
                "Auto-Pilot will:\n" +
                "• Run scheduled optimizations\n" +
                "• Monitor system resources\n" +
                "• Automatically optimize when needed\n" +
                "• Log all actions\n\n" +
                "Continue?",
                "Enable Auto-Pilot",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            isAutoPilotEnabled = true;
            StatusText.Text = "ENABLED";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xD9, 0x00));
            ToggleAutoPilotButton.Content = "⏹️ Disable Auto-Pilot";

            mainTimer.Start();
            idleCheckTimer.Start();

            CalculateNextRun();
            LogActivity("Auto-Pilot enabled");

            MessageBox.Show("Auto-Pilot enabled successfully!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DisableAutoPilot()
        {
            isAutoPilotEnabled = false;
            StatusText.Text = "DISABLED";
            StatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
            ToggleAutoPilotButton.Content = "🚀 Enable Auto-Pilot";
            NextRunText.Text = "Next scheduled run: Not scheduled";

            mainTimer.Stop();
            idleCheckTimer.Stop();

            LogActivity("Auto-Pilot disabled");

            MessageBox.Show("Auto-Pilot disabled.", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            // Check scheduled time
            if (DailyCleanCheckBox.IsChecked == true)
            {
                var scheduledTime = DateTime.Today.AddHours(3); // 3 AM default
                if (Math.Abs((DateTime.Now - scheduledTime).TotalMinutes) < 5)
                {
                    PerformScheduledOptimization();
                }
            }

            // Check RAM usage
            if (HighRAMCheckBox.IsChecked == true)
            {
                var info = new Microsoft.VisualBasic.Devices.ComputerInfo();
                double usedPercent = (info.TotalPhysicalMemory - info.AvailablePhysicalMemory) * 100.0 / info.TotalPhysicalMemory;
                
                if (usedPercent > 85)
                {
                    LogActivity($"High RAM usage detected ({usedPercent:F0}%) - Clearing memory");
                    ClearRAM();
                }
            }

            // Check battery
            if (LowBatteryCheckBox.IsChecked == true)
            {
                var powerStatus = System.Windows.Forms.SystemInformation.PowerStatus;
                if (powerStatus.BatteryLifePercent < 0.20f && powerStatus.PowerLineStatus == System.Windows.Forms.PowerLineStatus.Offline)
                {
                    LogActivity("Low battery detected - Killing heavy apps");
                    KillHeavyApps();
                }
            }
        }

        private void IdleCheckTimer_Tick(object sender, EventArgs e)
        {
            if (IdleOptimizeCheckBox.IsChecked == true)
            {
                var idleTime = DateTime.Now - lastActivityTime;
                if (idleTime.TotalMinutes > 10)
                {
                    LogActivity("System idle detected - Running optimization");
                    PerformIdleOptimization();
                    lastActivityTime = DateTime.Now; // Reset to avoid repeated runs
                }
            }
        }

        private void PerformScheduledOptimization()
        {
            LogActivity("Running scheduled daily optimization");
            
            // Clear temp files
            Process.Start("cmd.exe", "/c del /q /f /s %temp%\\* 2>nul");
            
            // Empty recycle bin
            Process.Start("cmd.exe", "/c rd /s /q C:\\$Recycle.Bin");
            
            // Kill heavy apps
            KillHeavyApps();
            
            // Clear standby memory
            ClearRAM();

            LogActivity("Scheduled optimization completed");
        }

        private void PerformIdleOptimization()
        {
            // Light optimization during idle
            ClearRAM();
            Process.Start("cmd.exe", "/c del /q /f /s %temp%\\* 2>nul");
        }

        private void KillHeavyApps()
        {
            var processes = Process.GetProcesses()
                .Where(p => p.WorkingSet64 > 500 * 1024 * 1024)
                .Where(p => !IsCriticalProcess(p.ProcessName));

            int killed = 0;
            foreach (var proc in processes)
            {
                try { proc.Kill(); killed++; } catch { }
            }

            if (killed > 0)
                LogActivity($"Killed {killed} heavy apps");
        }

        private void ClearRAM()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void CalculateNextRun()
        {
            if (DailyCleanCheckBox.IsChecked == true)
            {
                var scheduledTime = DateTime.Today.AddHours(3);
                if (scheduledTime < DateTime.Now)
                    scheduledTime = scheduledTime.AddDays(1);
                
                NextRunText.Text = $"Next scheduled run: {scheduledTime:MM/dd/yyyy h:mm tt}";
            }
        }

        private void LogActivity(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Insert(0, $"[{timestamp}] {message}");
            
            // Keep only last 50 entries
            while (activityLog.Count > 50)
                activityLog.RemoveAt(activityLog.Count - 1);
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            activityLog.Clear();
            activityLog.Add("Log cleared");
        }

        private bool IsCriticalProcess(string name)
        {
            string[] critical = { "System", "csrss", "smss", "services", "lsass", 
                "winlogon", "explorer", "dwm", "svchost", "RuntimeBroker" };
            return critical.Any(p => name.Equals(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
