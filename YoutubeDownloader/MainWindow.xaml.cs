using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using YoutubeDownloader.ViewModels;

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
    }

    private void OnButtonCloseMouseLeave(object sender, MouseEventArgs e)
    {
        imgClose.Source = new BitmapImage(new Uri(@"/Resources/Images/closeDark24.png", UriKind.Relative));
    }

    private void OnButtonCloseMouseEnter(object sender, MouseEventArgs e)
    {
        imgClose.Source = new BitmapImage(new Uri(@"/Resources/Images/close24.png", UriKind.Relative));
    }
}