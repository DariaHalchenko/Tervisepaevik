using Microsoft.Maui.Layouts;
using System.Globalization;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class TreeningudFotoPage : ContentPage
{
    private readonly TreeningudDatabase database;
    private Switch redirectSwitch;

    public TreeningudFotoPage()
    {
        Title = AppResources.MyWorkouts;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new TreeningudDatabase(dbPath);

        var treeningud = database.GetTreeningud()
            .OrderByDescending(t => t.Kallaaeg)
            .ToList();

        var carousel = new CarouselView
        {
            ItemsSource = treeningud,
            PeekAreaInsets = 20,
            HeightRequest = 450,
            ItemTemplate = new DataTemplate(() =>
            {
                var card = new Frame
                {
                    CornerRadius = 25,
                    Padding = 15,
                    Margin = new Thickness(10),
                    HasShadow = true
                };

                var nimiLabel = new Label
                {
                    FontSize = 22,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };
                nimiLabel.SetBinding(Label.TextProperty, "Treeningu_nimi");

                var tyyppLabel = new Label { FontSize = 14 };
                tyyppLabel.SetBinding(Label.TextProperty,
                    new Binding("Treeningu_tuup", stringFormat: $"{AppResources.Type}: {{0}}"));

                var kellaaegLabel = new Label { FontSize = 14 };
                kellaaegLabel.SetBinding(Label.TextProperty,
                    new Binding("Kallaaeg", stringFormat: $"{AppResources.Time1}: {{0:hh\\:mm}}"));

                var kirjeldusLabel = new Label { FontSize = 14 };
                kirjeldusLabel.SetBinding(Label.TextProperty,
                    new Binding("Kirjeldus", stringFormat: $"{AppResources.Description}: {{0}}"));

                var kaloridLabel = new Label { FontSize = 14 };
                kaloridLabel.SetBinding(Label.TextProperty,
                    new Binding("Kulutud_kalorid", stringFormat: $"{AppResources.Calories}: {{0}} kcal"));

                var image = new Image
                {
                    HeightRequest = 220,
                    Aspect = Aspect.AspectFill,
                    Margin = new Thickness(0, 10)
                };

                image.SetBinding(Image.SourceProperty,
                    new Binding("Treeningu_foto", converter: new ByteArrayToImageSourceConverter()));

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    if (((Image)s).BindingContext is TreeningudClass treening &&
                        !string.IsNullOrWhiteSpace(treening.Link))
                    {
                        try
                        {
                            await Browser.OpenAsync(treening.Link, BrowserLaunchMode.SystemPreferred);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert(
                                AppResources.Error,
                                $"{AppResources.CannotOpenLink}: {ex.Message}",
                                AppResources.OK);
                        }
                    }
                };

                image.GestureRecognizers.Add(tapGesture);

                var infoStack = new VerticalStackLayout
                {
                    Spacing = 5,
                    Children =
                    {
                        nimiLabel,
                        tyyppLabel,
                        kellaaegLabel,
                        kirjeldusLabel,
                        kaloridLabel
                    }
                };

                var mainStack = new VerticalStackLayout
                {
                    Spacing = 10,
                    Children =
                    {
                        infoStack,
                        image
                    }
                };

                card.Content = mainStack;

                return card;
            })
        };

        var addButton = new Button
        {
            Text = $"➕ {AppResources.AddWorkout}",
            BackgroundColor = Colors.MediumPurple,
            TextColor = Colors.White,
            CornerRadius = 15,
            HeightRequest = 50,
            FontAttributes = FontAttributes.Bold
        };

        addButton.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new TreeningudPage());
        };

        redirectSwitch = new Switch
        {
            OnColor = Colors.MediumPurple
        };

        redirectSwitch.Toggled += RedirectSwitch_Toggled;

        var switchLayout = new Frame
        {
            Padding = 10,
            CornerRadius = 20,
            HasShadow = true,
            Content = new HorizontalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = AppResources.Feeling,
                        VerticalOptions = LayoutOptions.Center,
                        FontAttributes = FontAttributes.Bold
                    },
                    redirectSwitch
                }
            }
        };

        var mainStack = new VerticalStackLayout
        {
            Padding = 15,
            Spacing = 15,
            Children =
            {
                addButton,
                carousel,
                switchLayout
            }
        };

        Content = new ScrollView
        {
            Content = mainStack
        };
    }

    private async void RedirectSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            await Navigation.PushAsync(new EnesetunnePage());
            Device.BeginInvokeOnMainThread(() => redirectSwitch.IsToggled = false);
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
}