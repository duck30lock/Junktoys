using System.Windows;
using Microsoft.Win32;

namespace WindowsDebloater
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Auto-detect and apply system theme
            ApplySystemTheme();
        }

        private void ApplySystemTheme()
        {
            try
            {
                // Read Windows theme setting from registry
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                var appsUseLightTheme = key?.GetValue("AppsUseLightTheme");
                
                if (appsUseLightTheme is int themeValue)
                {
                    // 0 = Dark mode, 1 = Light mode
                    ModernWpf.ThemeManager.Current.ApplicationTheme = 
                        themeValue == 0 ? ModernWpf.ApplicationTheme.Dark : ModernWpf.ApplicationTheme.Light;
                }
                else
                {
                    // Default to Dark mode
                    ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
                }
            }
            catch
            {
                // Default to Dark mode if detection fails
                ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
            }
        }
    }
}
