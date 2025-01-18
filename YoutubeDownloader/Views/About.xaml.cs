using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoutubeDownloader.Services;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            githubHL.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();
            ytexplodeHL.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();
            matDesignHL.Foreground = ServiceProvider.ThemeService.GetPrimaryColorBrush();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo(e.Uri.AbsoluteUri);
            psi.UseShellExecute = true;
            Process.Start(psi);
        }
    }
}
