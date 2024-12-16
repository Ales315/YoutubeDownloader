using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace YoutubeDownloader.Services
{
    public class ThemeService
    {
        private Color BackgroundLight = (Color)ColorConverter.ConvertFromString("#e6f3ea");
        private Color SurfaceLight = (Color)ColorConverter.ConvertFromString("#c1e1d6");
        private Color PrimaryLight = (Color)ColorConverter.ConvertFromString("#4cf8be");
        private Color SecondaryLight = (Color)ColorConverter.ConvertFromString("#7fe3e8");
        private Color AccentLight = (Color)ColorConverter.ConvertFromString("#65bbe3");
        private Color OnSecondaryLight = (Color)ColorConverter.ConvertFromString("#121212");

        private Color BackgroundDark = (Color)ColorConverter.ConvertFromString("#121212");
        private Color SurfaceDark = (Color)ColorConverter.ConvertFromString("#282828");
        private Color PrimaryDark = (Color)ColorConverter.ConvertFromString("#4cf8be");
        private Color SecondaryDark = (Color)ColorConverter.ConvertFromString("#0f2ba3");
        private Color AccentDark = (Color)ColorConverter.ConvertFromString("#5634ed");
        private Color OnPrimaryDark = (Color)ColorConverter.ConvertFromString("#121212");
        private Color OnSecondaryDark = (Color)ColorConverter.ConvertFromString("#d2f8ed");

        private Theme LightTheme = null!;
        private Theme DarkTheme = null!;
        public bool IsLightTheme { get; set; } = true;
        /// <summary>
        /// Light theme: true.  Dark theme: false
        /// </summary>
        public event EventHandler<bool> ThemeChanged = null!;


        public ThemeService()
        {
            CreateLightTheme();
            CreateDarkTheme();
            SetLightTheme();
        }

        private void CreateLightTheme()
        {
            LightTheme = null!;
            LightTheme = new();

            LightTheme.SetPrimaryColor(PrimaryLight);
            LightTheme.SetSecondaryColor(SecondaryLight);
            LightTheme.PrimaryLight = PrimaryLight;

            Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(OnSecondaryLight);
            Application.Current.Resources["MaterialDesignCardBackground"] = new SolidColorBrush(BackgroundLight);
            LightTheme.SetLightTheme();
        }

        private void CreateDarkTheme()
        {
            DarkTheme = null!;
            DarkTheme = new();

            DarkTheme.SetPrimaryColor(PrimaryLight);
            DarkTheme.SetSecondaryColor(SecondaryLight);
            DarkTheme.PrimaryDark = PrimaryDark;

            Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(OnSecondaryDark);
            Application.Current.Resources["MaterialDesignCardBackground"] = new SolidColorBrush(BackgroundDark);
            DarkTheme.Background = Colors.Red;
            DarkTheme.SetDarkTheme();
        }

        public void SetLightTheme()
        {
            var paletteHelper = new PaletteHelper();
            CreateLightTheme();
            paletteHelper.SetTheme(LightTheme);
            IsLightTheme = true;
            ThemeChanged?.Invoke(null, true);
        }

        public void SetDarkTheme()
        {
            var paletteHelper = new PaletteHelper();
            CreateDarkTheme();
            paletteHelper.SetTheme(DarkTheme);
            IsLightTheme = false;
            ThemeChanged?.Invoke(null, false);
        }

        public SolidColorBrush GetPrimaryColorBrush()
        {
            if (IsLightTheme)
                return new SolidColorBrush(PrimaryLight);
            else return new SolidColorBrush(PrimaryDark);
        }

        internal SolidColorBrush GetTextColorBrush()
        {
            return (SolidColorBrush)Application.Current.Resources["MaterialDesignBody"];
        }
    }
}
