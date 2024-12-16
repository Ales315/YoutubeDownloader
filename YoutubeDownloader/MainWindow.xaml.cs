using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
        ServiceProvider.ThemeService.SetLightTheme();
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
        ServiceProvider.YoutubeService.CancellationToken?.Cancel();
    }
}