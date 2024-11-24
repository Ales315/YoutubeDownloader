using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WpfAnimatedGif;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        _mainViewModel = new MainViewModel();
        DataContext = _mainViewModel;
        buttonClose.Click += (s, e) => Close();
        buttonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;
        textboxInputUrl.TextChanged += (s, e) => textBlockURLHint.Visibility = textboxInputUrl.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;
    }

    private void buttonPaste_Click(object sender, RoutedEventArgs e)
    {
        textboxInputUrl.Text = Regex.Replace(Clipboard.GetText(), @"\t|\n|\r", "");
    }

    private void buttonDownload_Click(object sender, RoutedEventArgs e)
    {
    }

    private void buttonSearch_Click(object sender, RoutedEventArgs e)
    {
        string inputUrl = textboxInputUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(inputUrl)) return;
        textboxInputUrl.Text = inputUrl;
        _mainViewModel.GetVideoMetadata(inputUrl);
    }

    private void Image_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var img = sender as Image;
        var controller = ImageBehavior.GetAnimationController(imgLoadingGif);
        if (controller == null) return;
        if (img?.Visibility == Visibility.Hidden || img?.Visibility == Visibility.Collapsed)
        {
            controller.Pause();
            controller.GotoFrame(1);
        }
        else
        {
            controller.Play();
        }
            
    }
}