using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeDownloader.ViewModels;

namespace YoutubeDownloader.UserControls
{
    /// <summary>
    /// Interaction logic for KeywordSearchUC.xaml
    /// </summary>
    public partial class KeywordSearchUC : UserControl
    {
        public KeywordSearchUC()
        {
            InitializeComponent();
        }
        private void SearchResultCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var card = ((SearchResultCard)sender).DataContext as SearchResultCardViewModel;
            if (card == null) return;

            var vm = this.DataContext as KeywordSearchViewModel;
            if (vm == null) return;

            switch (card.ResultType)
            {
                case Enums.SearchResultType.Video:
                    vm.GetVideoData(card.Url);
                    break;
            }
        }
    }
}
