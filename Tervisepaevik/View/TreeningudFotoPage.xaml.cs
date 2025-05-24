using Microsoft.Maui.Layouts;
using System.Globalization;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class TreeningudFotoPage : ContentPage
{
    private readonly TreeningudDatabase database;
    private Switch redirectSwitch;


    public TreeningudFotoPage()
    {
        Title = "Minu treeningud";

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new TreeningudDatabase(dbPath);

        var treeningud = database.GetTreeningud()
            .OrderByDescending(t => t.Kallaaeg)
            .ToList();

        // Создаем карусель
        var carousel = new CarouselView
        {
            ItemsSource = treeningud,
            ItemTemplate = new DataTemplate(() =>
            {
                var nimiLabel = new Label
                {
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };
                nimiLabel.SetBinding(Label.TextProperty, "Treeningu_nimi");

                var tyyppLabel = new Label();
                tyyppLabel.SetBinding(Label.TextProperty, new Binding("Treeningu_tuup", stringFormat: "Tüüp: {0}"));

                var kellaaegLabel = new Label();
                kellaaegLabel.SetBinding(Label.TextProperty, new Binding("Kallaaeg", stringFormat: "Kellaaeg: {0}"));

                var sammudLabel = new Label();
                sammudLabel.SetBinding(Label.TextProperty, new Binding("Kirjeldus", stringFormat: "Kirjeldus: {0}"));

                var kaloridLabel = new Label();
                kaloridLabel.SetBinding(Label.TextProperty, new Binding("Kulutud_kalorid", stringFormat: "Kalorid: {0}"));

                var image = new Image
                {
                    HeightRequest = 200,
                    Aspect = Aspect.AspectFill
                };
                image.SetBinding(Image.SourceProperty, new Binding("Treeningu_foto", converter: new ByteArrayToImageSourceConverter()));

                var juhisLabel = new Label
                {
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    Text = "Video vaatamiseks klõpsake pildil"
                };

                // TapGestureRecognizer to open video link
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    if (((Image)s).BindingContext is TreeningudClass treening && !string.IsNullOrWhiteSpace(treening.Link))
                    {
                        try
                        {
                            await Browser.OpenAsync(treening.Link, BrowserLaunchMode.SystemPreferred);
                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("Viga", $"Linki ei saa avada: {ex.Message}", "OK");
                        }
                    }
                };
                image.GestureRecognizers.Add(tapGesture);

                return new ScrollView
                {
                    Content = new StackLayout
                    {
                        Padding = new Thickness(20),
                        Spacing = 12,
                        Children =
                        {
                            nimiLabel,
                            tyyppLabel,
                            kellaaegLabel,
                            sammudLabel,
                            kaloridLabel,
                            image,
                            juhisLabel
                        }
                    }
                };
            })
        };

        // Создаем переключатель с текстом
        redirectSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.End,
            ThumbColor = Colors.DarkViolet,
            OnColor = Colors.LightGreen
        };
        redirectSwitch.Toggled += RedirectSwitch_Toggled;

        var switchLayout = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 10,
            Padding = new Thickness(0, 10),
            Children =
            {
                new Label
                {
                    Text = "Minu enesetunne",
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 14
                },
                redirectSwitch
            }
        };

        var frame = new Frame
        {
            CornerRadius = 30,
            HeightRequest = 60,
            WidthRequest = 60,
            BackgroundColor = Colors.White,
            Padding = 10,
            HasShadow = true,
            Content = new ImageButton
            {
                Source = "lisa.png",
                BackgroundColor = Colors.Transparent,
                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new TreeningudPage());
                })
            }
        };

        // Основной макет страницы
        var absoluteLayout = new AbsoluteLayout();

        AbsoluteLayout.SetLayoutFlags(carousel, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(carousel, new Rect(0, 0, 1, 1));
        absoluteLayout.Children.Add(carousel);

        AbsoluteLayout.SetLayoutFlags(switchLayout, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(switchLayout, new Rect(0.5, 0.95, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        absoluteLayout.Children.Add(switchLayout);

        AbsoluteLayout.SetLayoutFlags(frame, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(frame, new Rect(0.95, 0.95, 60, 60));
        absoluteLayout.Children.Add(frame);

        Content = absoluteLayout;

    }

    private async void RedirectSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            await Navigation.PushAsync(new EnesetunnePage());
            Device.BeginInvokeOnMainThread(() => redirectSwitch.IsToggled = false);
        }
    }
}

public class ByteArrayToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is byte[] bytes && bytes.Length > 0)
        {
            return ImageSource.FromStream(() => new MemoryStream(bytes));
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}