using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using YoutubeDownloader.Enums;
using YoutubeDownloader.Services;

namespace YoutubeDownloader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        buttonClose.Click += (s, e) => Close();
        buttonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;
        buttonClose.MouseEnter += OnButtonCloseMouseEnter;
        buttonClose.MouseLeave += OnButtonCloseMouseLeave;
        ServiceProvider.ThemeService.ThemeChanged += OnThemeChanged;
        SetAppTheme();
    }

    private void SetAppTheme()
    {
        switch (ServiceProvider.SettingsService.UserPreferences.ThemePreference)
        {
            case ThemeStyles.System:
                ServiceProvider.ThemeService.SetSystemTheme();
                break;
            case ThemeStyles.Dark:
                ServiceProvider.ThemeService.SetDarkTheme();
                break;
            case ThemeStyles.Light:
                ServiceProvider.ThemeService.SetLightTheme();
                break;
        }
    }

    private void OnThemeChanged(object? sender, bool e)
    {
        iconClose.Foreground = ServiceProvider.ThemeService.GetTextColorBrush();
    }

    private void OnButtonCloseMouseLeave(object sender, MouseEventArgs e)
    {
        iconClose.Foreground = !ServiceProvider.ThemeService.IsLightTheme ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.DarkSlateGray);
    }

    private void OnButtonCloseMouseEnter(object sender, MouseEventArgs e)
    {
        iconClose.Foreground = new SolidColorBrush(Colors.White);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ServiceProvider.YoutubeService.DownloadCancellationToken?.Cancel();
    }
}