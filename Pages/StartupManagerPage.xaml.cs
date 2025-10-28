using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsDebloater.Pages
{
    public partial class StartupManagerPage : Page
    {
        private ObservableCollection<StartupItem> startupItems = new ObservableCollection<StartupItem>();

        public StartupManagerPage()
        {
            InitializeComponent();
            StartupListView.ItemsSource = startupItems;
            LoadStartupItems();
        }

        private void LoadStartupItems()
        {
            startupItems.Clear();

            try
            {
                // Load from current user startup registry
                LoadFromRegistry(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"), "User");
                
                // Load from local machine startup registry
                try
                {
                    LoadFromRegistry(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run"), "System");
                }
                catch
                {
                    // May need admin rights
                }

                StatusTextBlock.Text = $"Loaded {startupItems.Count} startup items";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error loading startup items: {ex.Message}";
            }
        }

        private void LoadFromRegistry(RegistryKey? key, string scope)
        {
            if (key == null) return;

            foreach (string valueName in key.GetValueNames())
            {
                var value = key.GetValue(valueName)?.ToString() ?? "";
                var isEnabled = !string.IsNullOrEmpty(value);

                startupItems.Add(new StartupItem
                {
                    ProgramName = valueName,
                    Publisher = scope,
                    Path = value,
                    Status = isEnabled ? "Enabled" : "Disabled",
                    StatusColor = isEnabled ? Brushes.Green : Brushes.Gray,
                    Impact = DetermineImpact(valueName),
                    ImpactColor = DetermineImpact(valueName) == "High" ? Brushes.OrangeRed : Brushes.Orange
                });
            }
        }

        private string DetermineImpact(string programName)
        {
            // Simple heuristic - can be improved
            string[] highImpactKeywords = { "updater", "helper", "sync", "cloud" };
            return highImpactKeywords.Any(k => programName.ToLower().Contains(k)) ? "High" : "Medium";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadStartupItems();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = StartupListView.SelectedItems.Cast<StartupItem>().ToList();
            
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one startup item to disable.", "No Selection", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Disable {selectedItems.Count} startup item(s)?", 
                "Confirm", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int disabled = 0;
                foreach (var item in selectedItems)
                {
                    try
                    {
                        var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                        if (key?.GetValue(item.ProgramName) != null)
                        {
                            key.DeleteValue(item.ProgramName);
                            item.Status = "Disabled";
                            item.StatusColor = Brushes.Gray;
                            disabled++;
                        }
                    }
                    catch { }
                }

                StatusTextBlock.Text = $"Disabled {disabled} startup item(s)";
            }
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var nonEssentialItems = startupItems
                .Where(i => i.Impact == "High" && i.Status == "Enabled")
                .ToList();

            if (nonEssentialItems.Count == 0)
            {
                MessageBox.Show("No non-essential startup items found.", "Nothing to Disable", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Disable {nonEssentialItems.Count} non-essential startup items?", 
                "Confirm", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                int disabled = 0;
                foreach (var item in nonEssentialItems)
                {
                    try
                    {
                        var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                        if (key?.GetValue(item.ProgramName) != null)
                        {
                            key.DeleteValue(item.ProgramName);
                            item.Status = "Disabled";
                            item.StatusColor = Brushes.Gray;
                            disabled++;
                        }
                    }
                    catch { }
                }

                StatusTextBlock.Text = $"Disabled {disabled} non-essential startup item(s)";
            }
        }
    }

    public class StartupItem
    {
        public string ProgramName { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string Path { get; set; } = "";
        public string Status { get; set; } = "";
        public Brush StatusColor { get; set; } = Brushes.Black;
        public string Impact { get; set; } = "";
        public Brush ImpactColor { get; set; } = Brushes.Black;
    }
}
