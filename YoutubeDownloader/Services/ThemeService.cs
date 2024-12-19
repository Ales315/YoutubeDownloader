using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace YoutubeDownloader.Services
{
    public class ThemeService
    {
        private Color BackgroundLight = (Color)ColorConverter.ConvertFromString("#e9e9e9");
        private Color SurfaceLight = (Color)ColorConverter.ConvertFromString("#fbfbfb");
        private Color PrimaryLight = (Color)ColorConverter.ConvertFromString("#E43C42");
        private Color SecondaryLight = (Color)ColorConverter.ConvertFromString("#7fe3e8");
        private Color AccentLight = (Color)ColorConverter.ConvertFromString("#65bbe3");
        private Color OnSecondaryLight = (Color)ColorConverter.ConvertFromString("#1d1d1d");

        private Color BackgroundDark = (Color)ColorConverter.ConvertFromString("#121212");
        private Color SurfaceDark = (Color)ColorConverter.ConvertFromString("#282828");
        private Color PrimaryDark = (Color)ColorConverter.ConvertFromString("#E43C42");
        private Color SecondaryDark = (Color)ColorConverter.ConvertFromString("#0f2ba3");
        private Color AccentDark = (Color)ColorConverter.ConvertFromString("#5634ed");
        private Color OnPrimaryDark = (Color)ColorConverter.ConvertFromString("#121212");
        private Color OnSecondaryDark = (Color)ColorConverter.ConvertFromString("#e9e9e9");

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
            Application.Current.Resources["MaterialDesignBackground"] = new SolidColorBrush(SurfaceLight);
            Application.Current.Resources["MaterialDesignPaper"] = new SolidColorBrush(BackgroundLight);
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
            Application.Current.Resources["MaterialDesignPaper"] = new SolidColorBrush(BackgroundDark);
            Application.Current.Resources["MaterialDesignBackground"] = new SolidColorBrush(SurfaceDark);
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

        public SolidColorBrush GetTextColorBrush()
        {
            return (SolidColorBrush)Application.Current.Resources["MaterialDesignBody"];
        }

        private bool IsSystemLightTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }

        public void SetSystemTheme()
        {
            if(IsSystemLightTheme())
                SetLightTheme();
            else SetDarkTheme();
        }
    }
}
