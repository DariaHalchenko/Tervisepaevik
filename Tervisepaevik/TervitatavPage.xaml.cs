namespace Tervisepaevik;

public partial class TervitatavPage : ContentPage
{
    Label lbl_tervitav, lbl_sisu;
    Button btn_alusta, themeButton;

    public TervitatavPage()
    {
        Title = "Tervitatav";

        // КНОПКА ТЕМЫ
        themeButton = new Button
        {
            Text = "🌙",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.End,
            BackgroundColor = Colors.Transparent
        };

        themeButton.Clicked += (s, e) =>
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                themeButton.Text = "🌙";
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                themeButton.Text = "☀️";
            }
        };

        lbl_tervitav = new Label
        {
            Text = "Tere tulemast!",
            FontFamily = "Lovesty 400",
            FontSize = 40,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        lbl_sisu = new Label
        {
            Text = "Kas olete valmis alustama oma teekonda tervisliku keha poole?",
            FontSize = 18,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };

        btn_alusta = new Button
        {
            Text = "Alusta",
            BackgroundColor = Colors.Green, 
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 20, 0, 40)
        };
        btn_alusta.Clicked += Btn_alusta_Clicked;

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
                await tervisImage.ScaleTo(1.2, 2000, Easing.SinInOut);
                await tervisImage.ScaleTo(1.0, 2000, Easing.SinInOut);
            }
        };

        Content = new StackLayout
        {
            Padding = 20,
            Children =
            {
                themeButton, // КНОПКА СВЕРХУ

                new StackLayout
                {
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children = { lbl_tervitav, lbl_sisu, tervisImage }
                },

                btn_alusta
            }
        };
    }

    private async void Btn_alusta_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPage1());
    }
}