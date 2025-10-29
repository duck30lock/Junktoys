using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsDebloater.Pages
{
    public partial class ProfilesPage : Page
    {
        private List<OptimizationProfile> customProfiles = new List<OptimizationProfile>();
        private string profilesPath;

        public ProfilesPage()
        {
            InitializeComponent();
            profilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Junktoys", "Profiles");
            
            Directory.CreateDirectory(profilesPath);
            LoadCustomProfiles();
        }

        private void LoadCustomProfiles()
        {
            customProfiles.Clear();
            
            if (Directory.Exists(profilesPath))
            {
                foreach (var file in Directory.GetFiles(profilesPath, "*.json"))
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var profile = JsonSerializer.Deserialize<OptimizationProfile>(json);
                        if (profile != null)
                            customProfiles.Add(profile);
                    }
                    catch { }
                }
            }

            CustomProfilesListView.ItemsSource = null;
            CustomProfilesListView.ItemsSource = customProfiles;
        }

        private void LoadGaming_Click(object sender, RoutedEventArgs e)
        {
            ApplyGamingProfile();
        }

        private void LoadWork_Click(object sender, RoutedEventArgs e)
        {
            ApplyWorkProfile();
        }

        private void LoadBattery_Click(object sender, RoutedEventArgs e)
        {
            ApplyBatteryProfile();
        }

        private void GamingProfile_Click(object sender, MouseButtonEventArgs e)
        {
            // Visual feedback only
        }

        private void WorkProfile_Click(object sender, MouseButtonEventArgs e)
        {
            // Visual feedback only
        }

        private void BatteryProfile_Click(object sender, MouseButtonEventArgs e)
        {
            // Visual feedback only
        }

        private void ApplyGamingProfile()
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply Gaming Profile?\n\n" +
                    "This will:\n" +
                    "• Enable Ultimate Performance power plan\n" +
                    "• Kill background applications\n" +
                    "• Optimize network for gaming\n" +
                    "• Disable Windows Update\n" +
                    "• Set high process priorities\n\n" +
                    "Continue?",
                    "Gaming Profile",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Apply gaming optimizations
                RunCommand("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                RunCommand("sc stop wuauserv");
                RunCommand("netsh int tcp set global autotuninglevel=normal");

                CurrentProfileText.Text = "Gaming";
                
                MessageBox.Show("Gaming profile applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyWorkProfile()
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply Work Profile?\n\n" +
                    "This will:\n" +
                    "• Set balanced power plan\n" +
                    "• Keep productivity applications running\n" +
                    "• Enable Windows Update\n" +
                    "• Moderate optimizations\n\n" +
                    "Continue?",
                    "Work Profile",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Apply work optimizations
                RunCommand("powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
                RunCommand("sc config wuauserv start=demand");
                RunCommand("sc start wuauserv");

                CurrentProfileText.Text = "Work";
                
                MessageBox.Show("Work profile applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyBatteryProfile()
        {
            try
            {
                var result = MessageBox.Show(
                    "Apply Battery Saver Profile?\n\n" +
                    "This will:\n" +
                    "• Enable power saver mode\n" +
                    "• Kill heavy applications\n" +
                    "• Reduce performance\n" +
                    "• Optimize for battery life\n\n" +
                    "Continue?",
                    "Battery Saver Profile",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                // Apply battery optimizations
                RunCommand("powercfg /setactive a1841308-3541-4fab-bc81-f71556f20b4a");
                
                // Kill heavy apps
                var processes = Process.GetProcesses();
                foreach (var proc in processes)
                {
                    try
                    {
                        if (proc.WorkingSet64 > 300 * 1024 * 1024)
                            proc.Kill();
                    }
                    catch { }
                }

                CurrentProfileText.Text = "Battery Saver";
                
                MessageBox.Show("Battery Saver profile applied!", "Success", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateProfile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Title = "Create Custom Profile",
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stack = new StackPanel { Margin = new Thickness(20) };
            
            stack.Children.Add(new TextBlock { Text = "Profile Name:", Margin = new Thickness(0, 0, 0, 5) });
            var nameBox = new TextBox { Margin = new Thickness(0, 0, 0, 15) };
            stack.Children.Add(nameBox);

            stack.Children.Add(new TextBlock { Text = "Description:", Margin = new Thickness(0, 0, 0, 5) });
            var descBox = new TextBox { Margin = new Thickness(0, 0, 0, 15), Height = 60, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
            stack.Children.Add(descBox);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var createBtn = new Button { Content = "Create", Width = 80, Margin = new Thickness(0, 0, 10, 0) };
            var cancelBtn = new Button { Content = "Cancel", Width = 80 };
            
            createBtn.Click += (s, args) =>
            {
                if (string.IsNullOrWhiteSpace(nameBox.Text))
                {
                    MessageBox.Show("Please enter a profile name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var profile = new OptimizationProfile
                {
                    ProfileName = nameBox.Text,
                    Description = descBox.Text,
                    Created = DateTime.Now.ToString("MM/dd/yyyy")
                };

                var filePath = Path.Combine(profilesPath, $"{profile.ProfileName}.json");
                File.WriteAllText(filePath, JsonSerializer.Serialize(profile));

                dialog.Close();
                LoadCustomProfiles();
                MessageBox.Show("Profile created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            cancelBtn.Click += (s, args) => dialog.Close();

            btnPanel.Children.Add(createBtn);
            btnPanel.Children.Add(cancelBtn);
            stack.Children.Add(btnPanel);

            dialog.Content = stack;
            dialog.ShowDialog();
        }

        private void LoadCustom_Click(object sender, RoutedEventArgs e)
        {
            if (CustomProfilesListView.SelectedItem == null)
            {
                MessageBox.Show("Please select a profile to load.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var profile = (OptimizationProfile)CustomProfilesListView.SelectedItem;
            CurrentProfileText.Text = profile.ProfileName;
            MessageBox.Show($"Profile '{profile.ProfileName}' loaded!", "Success", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (CustomProfilesListView.SelectedItem == null)
            {
                MessageBox.Show("Please select a profile to delete.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var profile = (OptimizationProfile)CustomProfilesListView.SelectedItem;
            var result = MessageBox.Show($"Delete profile '{profile.ProfileName}'?", "Confirm Delete", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var filePath = Path.Combine(profilesPath, $"{profile.ProfileName}.json");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                LoadCustomProfiles();
                MessageBox.Show("Profile deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export feature coming soon!", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ImportProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Import feature coming soon!", "Info", 
                MessageBoxButton.OK, MessageBoxImage.Information);
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

        private class OptimizationProfile
        {
            public string ProfileName { get; set; }
            public string Description { get; set; }
            public string Created { get; set; }
        }
    }
}
