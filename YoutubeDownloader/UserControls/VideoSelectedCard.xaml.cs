using System.Windows.Controls;
using System.Windows.Media;
using YoutubeDownloader.Services;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for VideoSelectedCard.xaml
    /// </summary>
    public partial class VideoSelectedCard : UserControl
    {
        public VideoSelectedCard()
        {
            InitializeComponent();
            borderThumbnail.BorderBrush = new SolidColorBrush(ServiceProvider.ThemeService.GetPrimary());
        }
    }
}
