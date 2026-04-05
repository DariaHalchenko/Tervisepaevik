using System.Globalization;
using System.Threading;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik;

public partial class TervitatavPage : ContentPage
{
    Label lbl_tervitav, lbl_sisu;
    Button btn_alusta;
    ImageButton themeButton, langButton;

    public TervitatavPage()
    {
        Title = AppResources.WelcomePageTitle;

        // ФЛАГ ЯЗЫКА
        langButton = new ImageButton
        {
            Source = "flag_en.svg",
            WidthRequest = 35,
            HeightRequest = 35,
            BackgroundColor = Colors.Transparent
        };

        // КНОПКА ТЕМЫ
        themeButton = new ImageButton
        {
            Source = "moon.svg",
            WidthRequest = 30,
            HeightRequest = 30,
            BackgroundColor = Colors.Transparent
        };

        // СМЕНА ЯЗЫКА
        langButton.Clicked += (s, e) =>
        {
            var current = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            var newLang = current == "et" ? "en" : "et";

            SetLanguage(newLang);

            // меняем флаг
            langButton.Source = newLang == "et" ? "flag_et.svg" : "flag_en.svg";

            // обновляем текст
            lbl_tervitav.Text = AppResources.Welcome;
            lbl_sisu.Text = AppResources.Text;
            btn_alusta.Text = AppResources.Start;
        };

        // СМЕНА ТЕМЫ
        themeButton.Clicked += (s, e) =>
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                themeButton.Source = "moon.svg";
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                themeButton.Source = "sun.svg";
            }
        };

        // ТЕКСТЫ
        lbl_tervitav = new Label
        {
            Text = AppResources.Welcome,
            FontFamily = "Lovesty 400",
            FontSize = 40,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        lbl_sisu = new Label
        {
            Text = AppResources.Text,
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        btn_alusta = new Button
        {
            Text = AppResources.Start,
            BackgroundColor = Colors.Green,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 20, 0, 40)
        };
        btn_alusta.Clicked += Btn_alusta_Clicked;

        // КАРТИНКА
        var tervisImage = new Image
        {
            Source = "tervis.png",
            WidthRequest = 300,
            HeightRequest = 300,
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Center
        };

        tervisImage.Loaded += async (s, e) =>
        {
            while (true)
            {
                await tervisImage.ScaleTo(1.1, 1500);
                await tervisImage.ScaleTo(1.0, 1500);
            }
        };

        // ВЕРХНЯЯ ПАНЕЛЬ (справа)
        var topBar = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            }
        };

        // язык
        Grid.SetColumn(langButton, 1);
        topBar.Children.Add(langButton);

        // тема
        Grid.SetColumn(themeButton, 2);
        topBar.Children.Add(themeButton);

        // ОСНОВНОЙ ЛЕЙАУТ
        Content = new StackLayout
        {
            Padding = 20,
            Spacing = 15,
            Children =
            {
                topBar,
                lbl_tervitav,
                lbl_sisu,
                tervisImage,
                btn_alusta
            }
        };
    }

    private async void Btn_alusta_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1());
    }

    void SetLanguage(string lang)
    {
        var culture = new CultureInfo(lang);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}