using System.Diagnostics;
using System.Reflection;
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
        GetAppVersion();
        buttonClose.Click += (s, e) => Close();
        buttonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;
        buttonMaximize.Click += OnButtonMaximizeClick;
        buttonClose.MouseEnter += OnButtonCloseMouseEnter;
        buttonClose.MouseLeave += OnButtonCloseMouseLeave;
        ServiceProvider.ThemeService.ThemeChanged += OnThemeChanged;
        this.StateChanged += OnWindowStateChanged;
        SetAppTheme();
        iconMaximize.Kind = WindowState == WindowState.Maximized ? MaterialDesignThemes.Wpf.PackIconKind.WindowRestore : MaterialDesignThemes.Wpf.PackIconKind.Maximize;
    }

    private void OnButtonMaximizeClick(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            titlebarButtonsStackPanel.Margin = new Thickness(0, 8, 8, 0);
            titlebarTitleStackPanel.Margin = new Thickness(14, 8, 0, 0);
        }
        else
        {
            titlebarButtonsStackPanel.Margin = new Thickness(0);
            titlebarTitleStackPanel.Margin = new Thickness(10, 0, 0, 0);
        }
        iconMaximize.Kind = WindowState == WindowState.Maximized ? MaterialDesignThemes.Wpf.PackIconKind.CheckboxMultipleBlankOutline : MaterialDesignThemes.Wpf.PackIconKind.Maximize;
        iconMaximize.RenderTransform = new ScaleTransform(-1, -1, 7, 7);
    }

    private void GetAppVersion()
    {
        string version = string.Empty;
        version = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly()!.Location).ProductVersion ?? "-.-.-";
        tbVersion.Text = version;
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