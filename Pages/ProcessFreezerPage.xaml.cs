using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class ProcessFreezerPage : Page
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        private const int THREAD_SUSPEND_RESUME = 0x0002;
        private Dictionary<int, bool> frozenProcesses = new Dictionary<int, bool>();
        private DispatcherTimer refreshTimer;

        public ProcessFreezerPage()
        {
            InitializeComponent();
            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            refreshTimer.Tick += (s, e) => RefreshProcessList();
            refreshTimer.Start();
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            try
            {
                var processes = Process.GetProcesses()
                    .Where(p => !IsCriticalProcess(p.ProcessName))
                    .OrderByDescending(p => p.WorkingSet64)
                    .Select(p => new
                    {
                        ProcessName = p.ProcessName,
                        ProcessId = p.Id,
                        Status = frozenProcesses.ContainsKey(p.Id) && frozenProcesses[p.Id] ? "FROZEN" : "Running",
                        StatusColor = frozenProcesses.ContainsKey(p.Id) && frozenProcesses[p.Id] ? 
                            new SolidColorBrush(Color.FromRgb(0x00, 0xB8, 0xD4)) : 
                            new SolidColorBrush(Color.FromRgb(0x00, 0xD9, 0x00)),
                        MemoryMB = (p.WorkingSet64 / 1024 / 1024).ToString("N0"),
                        CpuPercent = "N/A",
                        ThreadCount = p.Threads.Count
                    })
                    .ToList();

                ProcessListView.ItemsSource = processes;

                // Update summary
                int frozenCount = frozenProcesses.Count(kv => kv.Value);
                long memorySaved = 0;
                foreach (var pid in frozenProcesses.Where(kv => kv.Value).Select(kv => kv.Key))
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        memorySaved += proc.WorkingSet64;
                    }
                    catch { }
                }

                FrozenCountText.Text = frozenCount.ToString();
                MemorySavedText.Text = $"{memorySaved / 1024 / 1024:N0} MB";
            }
            catch { }
        }

        private void Freeze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = ProcessListView.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one process to freeze.", 
                        "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int frozenCount = 0;
                foreach (dynamic item in selectedItems)
                {
                    int pid = item.ProcessId;
                    if (FreezeProcess(pid))
                    {
                        frozenProcesses[pid] = true;
                        frozenCount++;
                    }
                }

                RefreshProcessList();
                MessageBox.Show($"Frozen {frozenCount} process(es)!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItems = ProcessListView.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Please select at least one process to resume.", 
                        "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int resumedCount = 0;
                foreach (dynamic item in selectedItems)
                {
                    int pid = item.ProcessId;
                    if (ResumeProcess(pid))
                    {
                        frozenProcesses[pid] = false;
                        resumedCount++;
                    }
                }

                RefreshProcessList();
                MessageBox.Show($"Resumed {resumedCount} process(es)!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FreezeAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "This will freeze all non-essential processes.\n\nContinue?",
                    "Freeze All",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                string[] nonEssential = { "chrome", "firefox", "msedge", "discord", "spotify", 
                    "steam", "slack", "teams", "outlook", "notepad" };

                var processes = Process.GetProcesses()
                    .Where(p => nonEssential.Any(n => p.ProcessName.ToLower().Contains(n)));

                int frozenCount = 0;
                foreach (var proc in processes)
                {
                    if (FreezeProcess(proc.Id))
                    {
                        frozenProcesses[proc.Id] = true;
                        frozenCount++;
                    }
                }

                RefreshProcessList();
                MessageBox.Show($"Frozen {frozenCount} processes!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResumeAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int resumedCount = 0;
                foreach (var pid in frozenProcesses.Where(kv => kv.Value).Select(kv => kv.Key).ToList())
                {
                    if (ResumeProcess(pid))
                    {
                        frozenProcesses[pid] = false;
                        resumedCount++;
                    }
                }

                RefreshProcessList();
                MessageBox.Show($"Resumed {resumedCount} processes!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcessList();
        }

        private bool FreezeProcess(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)thread.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ResumeProcess(int pid)
        {
            try
            {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)thread.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        ResumeThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsCriticalProcess(string name)
        {
            string[] critical = { "System", "csrss", "smss", "services", "lsass", 
                "winlogon", "explorer", "dwm", "svchost", "RuntimeBroker" };
            return critical.Any(p => name.Equals(p, StringComparison.OrdinalIgnoreCase));
        }
    }
}
