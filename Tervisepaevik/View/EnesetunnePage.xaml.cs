using Microsoft.Maui.Controls;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class EnesetunnePage : ContentPage
{
    EnesetunneDatabase database;

    ListView enesetunneListView;
    DatePicker dp_kuupaev;
    Button btn_salvesta, btn_kustuta, btn_hingeohk;

    StackLayout sl_tuju, sl_energia;

    int selectedTuju = 0;
    int selectedEnergia = 0;

    EnesetunneClass selectedItem;

    public EnesetunnePage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new EnesetunneDatabase(dbPath);

        Title = AppResources.Feeling;

        dp_kuupaev = new DatePicker
        {
            Date = DateTime.Now
        };

        btn_salvesta = CreateButton(AppResources.Save, Colors.Green);
        btn_kustuta = CreateButton(AppResources.Delete, Colors.Red);
        btn_hingeohk = CreateButton(AppResources.BreathingExercise, Colors.Blue);

        btn_kustuta.IsVisible = false;
        btn_hingeohk.IsVisible = false;

        sl_tuju = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0)
        };

        for (int i = 1; i <= 5; i++)
        {
            var img = CreateMoodImage($"tuju{i}.svg", i, true);
            sl_tuju.Children.Add(img);
        }

        sl_energia = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 10
        };

        for (int i = 1; i <= 5; i++)
        {
            sl_energia.Children.Add(CreateMoodImage("energia.svg", i, false));
        }

        enesetunneListView = new ListView(ListViewCachingStrategy.RecycleElement)
        {
            SeparatorVisibility = SeparatorVisibility.None,
            HasUnevenRows = true,
            BackgroundColor = Colors.Transparent
        };

        enesetunneListView.ItemTemplate = new DataTemplate(() =>
        {
            var tujuImage = new Image { HeightRequest = 40, WidthRequest = 40 };
            tujuImage.SetBinding(Image.SourceProperty, "TujuImageSource");

            var energiaImage = new Image { HeightRequest = 40, WidthRequest = 40 };
            energiaImage.SetBinding(Image.SourceProperty, "EnergiaImageSource");

            var dateLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            };
            dateLabel.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "{0:dd.MM.yyyy}"));

            var heart = new Image
            {
                Source = "suda.svg",
                WidthRequest = 25,
                HeightRequest = 25
            };

            heart.SetBinding(IsVisibleProperty,
                new Binding("Energia", converter: new EnergiaToHeartVisibilityConverter()));

            var grid = new Grid
            {
                Padding = 10,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            grid.Add(tujuImage);
            Grid.SetColumn(tujuImage, 0);

            grid.Add(energiaImage);
            Grid.SetColumn(energiaImage, 1);

            grid.Add(dateLabel);
            Grid.SetColumn(dateLabel, 2);

            grid.Add(heart);
            Grid.SetColumn(heart, 3);

            return new ViewCell
            {
                View = new Frame
                {
                    CornerRadius = 15,
                    Margin = 5,
                    Padding = 10,
                    HasShadow = false,
                    Content = grid
                }
            };
        });

        enesetunneListView.ItemSelected += EnesetunneListView_ItemSelected;

        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        btn_hingeohk.Clicked += Btn_hingeohk_Clicked;

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 15,
                Spacing = 15,
                Children =
                {
                    CreateCard(AppResources.Date, dp_kuupaev),
                    CreateCard(AppResources.Mood, sl_tuju),
                    CreateCard(AppResources.Energy, sl_energia),

                    btn_salvesta,
                    btn_kustuta,
                    btn_hingeohk,

                    CreateListCard()
                }
            }
        };

        NaitaAndmeid();
    }

    private Image CreateMoodImage(string source, int level, bool isMood)
    {
        var img = new Image
        {
            Source = source,
            HeightRequest = 55,
            WidthRequest = 55,
            Opacity = 0.5
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            ResetMoodOpacity(isMood);

            if (isMood)
                selectedTuju = level;
            else
                selectedEnergia = level;

            await img.ScaleTo(1.3, 150);
            await img.ScaleTo(1.0, 150);

            img.Opacity = 1;

            KontrolliHingeohkNuppu();
        };

        img.GestureRecognizers.Add(tap);
        return img;
    }

    private void ResetMoodOpacity(bool isMood)
    {
        var stack = isMood ? sl_tuju : sl_energia;

        foreach (var view in stack.Children)
        {
            if (view is Image img)
            {
                img.Opacity = 0.6;
                img.Scale = 1;
            }
        }
    }

    private Frame CreateCard(string title, Microsoft.Maui.Controls.View content)
    {
        return new Frame
        {
            CornerRadius = 20,
            Padding = 15,
            HasShadow = true,
            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = title,
                        FontAttributes = FontAttributes.Bold
                    },
                    content
                }
            }
        };
    }

    private Frame CreateListCard()
    {
        return new Frame
        {
            CornerRadius = 20,
            Padding = 10,
            HasShadow = true,
            Content = enesetunneListView
        };
    }

    private Button CreateButton(string text, Color color)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = color,
            CornerRadius = 10
        };
    }

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (selectedTuju == 0 || selectedEnergia == 0)
        {
            await DisplayAlert(AppResources.Error, AppResources.SelectMoodEnergy, AppResources.OK);
            return;
        }

        if (selectedItem == null)
            selectedItem = new EnesetunneClass();

        selectedItem.Tuju = selectedTuju;
        selectedItem.Energia = selectedEnergia;
        selectedItem.Kuupaev = dp_kuupaev.Date;

        database.SaveEnesetunne(selectedItem);

        await DisplayAlert(AppResources.Info, AppResources.Saved, AppResources.OK);

        SelgeForm();
        NaitaAndmeid();
    }

    private void Btn_kustuta_Clicked(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteEnesetunne(selectedItem.Enesetunne_id);
            SelgeForm();
            NaitaAndmeid();
        }
    }

    private async void Btn_hingeohk_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert(AppResources.BreathingExercise, AppResources.BreathingStarted, AppResources.OK);
    }

    public void NaitaAndmeid()
    {
        enesetunneListView.ItemsSource = database.GetEnesetunne()
            .OrderByDescending(e => e.Kuupaev)
            .ToList();
    }

    private void EnesetunneListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as EnesetunneClass;

        if (selectedItem != null)
        {
            selectedTuju = selectedItem.Tuju;
            selectedEnergia = selectedItem.Energia;
            dp_kuupaev.Date = selectedItem.Kuupaev;

            btn_kustuta.IsVisible = true;
        }
    }

    private void SelgeForm()
    {
        selectedItem = null;
        selectedTuju = 0;
        selectedEnergia = 0;

        dp_kuupaev.Date = DateTime.Now;
        enesetunneListView.SelectedItem = null;

        btn_kustuta.IsVisible = false;
        btn_hingeohk.IsVisible = false;
    }

    private void KontrolliHingeohkNuppu()
    {
        btn_hingeohk.IsVisible = (selectedEnergia >= 1 && (selectedTuju == 1 || selectedTuju == 2));
    }

    public class EnergiaToHeartVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int energia = (int)value;
            return energia >= 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}