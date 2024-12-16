﻿using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace YoutubeDownloader.Helpers
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

        private Theme LightTheme;
        private Theme DarkTheme;


        public ThemeService()
        {
            CreateLightTheme();
            CreateDarkTheme();
        }

        private void CreateLightTheme()
        {
            LightTheme = null!;
            LightTheme = new();

            LightTheme.SetPrimaryColor(PrimaryLight);
            LightTheme.SetSecondaryColor(SecondaryLight);
            LightTheme.Foreground = Colors.Red;
            LightTheme.ForegroundLight = Colors.Red;
            Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(OnSecondaryLight);
            LightTheme.SetLightTheme();
        }

        private void CreateDarkTheme()
        {
            DarkTheme = null!;
            DarkTheme = new();

            DarkTheme.SetPrimaryColor(PrimaryLight);
            DarkTheme.SetSecondaryColor(SecondaryLight);
            DarkTheme.Cards.Background = SurfaceDark;
            DarkTheme.ComboBoxes.FilledBackground = SurfaceDark;
            Application.Current.Resources["MaterialDesignBody"] = new SolidColorBrush(OnSecondaryDark);
            DarkTheme.SetDarkTheme();
        }

        public void SetLightTheme()
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();
            CreateLightTheme();
            paletteHelper.SetTheme(LightTheme);
        }

        public void SetDarkTheme()
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();
            CreateDarkTheme();
            paletteHelper.SetTheme(DarkTheme);
        }
    }
}
