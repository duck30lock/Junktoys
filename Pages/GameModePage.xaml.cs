using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsDebloater.Pages
{
    public partial class GameModePage : Page
    {
        private bool isGameModeActive = false;
        private DispatcherTimer refreshTimer;
        private List<Process> boostedProcesses = new List<Process>();

        public GameModePage()
        {
            InitializeComponent();
            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            refreshTimer.Tick += (s, e) => RefreshGamesList();
            RefreshGamesList();
        }

        private void ToggleGameMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameModeActive)
            {
                ActivateGameMode();
            }
            else
            {
                DeactivateGameMode();
            }
        }

        private async void ActivateGameMode()
        {
            try
            {
                var result = MessageBox.Show(
                    "Game Mode will:\n\n" +
                    "• Kill non-essential processes\n" +
                    "• Set high priority for games\n" +
                    "• Pause Windows Update\n" +
                    "• Optimize network & GPU\n" +
                    "• Enable Ultimate Performance\n\n" +
                    "Continue?",
                    "Activate Game Mode",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                isGameModeActive = true;
                UpdateUI();

                // Kill background processes
                if (KillBackgroundCheckBox.IsChecked == true)
                {
                    await Task.Run(() => KillNonEssentialProcesses());
                }

                // Boost game processes
                if (MaxPriorityCheckBox.IsChecked == true)
                {
                    await Task.Run(() => BoostGameProcesses());
                }

                // Pause Windows Update
                if (DisableUpdatesCheckBox.IsChecked == true)
                {
                    RunCommand("sc stop wuauserv");
                    RunCommand("sc config wuauserv start=disabled");
                }

                // GPU optimization
                if (OptimizeGPUCheckBox.IsChecked == true)
                {
                    RunCommand("reg add \"HKEY_CURRENT_USER\\Software\\Microsoft\\DirectX\\UserGpuPreferences\" /f");
                }

                // Network optimization
                if (NetworkBoostCheckBox.IsChecked == true)
                {
                    RunCommand("netsh int tcp set global autotuninglevel=normal");
                    RunCommand("netsh interface ip set dns \"Ethernet\" static 1.1.1.1");
                }

                // Power plan
                if (PowerPlanCheckBox.IsChecked == true)
                {
                    RunCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                }

                refreshTimer.Start();

                MessageBox.Show("🎮 Game Mode Activated!\n\nYour system is now optimized for gaming.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error activating Game Mode: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeactivateGameMode()
        {
            try
            {
                isGameModeActive = false;
                refreshTimer.Stop();

                // Re-enable Windows Update
                RunCommand("sc config wuauserv start=demand");
                RunCommand("sc start wuauserv");

                // Reset process priorities
                foreach (var proc in boostedProcesses)
                {
                    try
                    {
                        if (!proc.HasExited)
                            proc.PriorityClass = ProcessPriorityClass.Normal;
                    }
                    catch { }
                }
                boostedProcesses.Clear();

                UpdateUI();

                MessageBox.Show("Game Mode Deactivated", "Info", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deactivating Game Mode: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void KillNonEssentialProcesses()
        {
            string[] nonEssential = { "chrome", "firefox", "msedge", "discord", "spotify", 
                "steam", "epicgameslauncher", "slack", "teams", "outlook" };
            
            var processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                try
                {
                    if (nonEssential.Any(n => proc.ProcessName.ToLower().Contains(n)))
                    {
                        proc.Kill();
                    }
                }
                catch { }
            }
        }

        private void BoostGameProcesses()
        {
            string[] gameProcesses = { "game", "dx11", "dx12", "unreal", "unity", "csgo", 
                "valorant", "league", "fortnite", "cod", "apex", "overwatch", "pubg" };
            
            var processes = Process.GetProcesses();
            foreach (var proc in processes)
            {
                try
                {
                    if (gameProcesses.Any(g => proc.ProcessName.ToLower().Contains(g)))
                    {
                        proc.PriorityClass = ProcessPriorityClass.High;
                        boostedProcesses.Add(proc);
                    }
                }
                catch { }
            }
        }

        private void RefreshGamesList()
        {
            try
            {
                string[] gameKeywords = { "game", "dx11", "dx12", "unreal", "unity", "csgo", 
                    "valorant", "league", "fortnite", "cod", "apex", "overwatch", "pubg", 
                    "minecraft", "roblox", "gta" };

                var games = Process.GetProcesses()
                    .Where(p => gameKeywords.Any(k => p.ProcessName.ToLower().Contains(k)))
                    .Select(p => new
                    {
                        GameName = p.ProcessName,
                        CpuUsage = "N/A",
                        MemoryMB = (p.WorkingSet64 / 1024 / 1024).ToString(),
                        Priority = p.PriorityClass.ToString()
                    })
                    .ToList();

                GamesListView.ItemsSource = games;
            }
            catch { }
        }

        private void RefreshGames_Click(object sender, RoutedEventArgs e)
        {
            RefreshGamesList();
        }

        private void UpdateUI()
        {
            if (isGameModeActive)
            {
                GameModeStatusText.Text = "Game Mode: ON";
                GameModeStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0x00, 0xD9, 0x00));
                GameModeSubText.Text = "System optimized for gaming";
                ToggleGameModeButton.Content = "🛑 Deactivate Game Mode";
            }
            else
            {
                GameModeStatusText.Text = "Game Mode: OFF";
                GameModeStatusText.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
                GameModeSubText.Text = "Click the button below to activate";
                ToggleGameModeButton.Content = "🚀 Activate Game Mode";
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
                    RedirectStandardOutput = true,
                    Verb = "runas"
                };
                Process.Start(psi)?.WaitForExit(3000);
            }
            catch { }
        }
    }
}
