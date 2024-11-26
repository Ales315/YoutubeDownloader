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
    public MainWindow()
    {
        InitializeComponent();
        buttonClose.Click += (s, e) => Close();
        buttonMinimize.Click += (s, e) => WindowState = WindowState.Minimized;
    }
}