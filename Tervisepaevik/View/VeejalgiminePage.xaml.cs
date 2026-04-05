using Microsoft.Maui.Layouts;
using System.Globalization;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class VeejalgiminePage : ContentPage
{
    VeejalgimineDatabase database;

    Entry kogusEntry;
    DatePicker kuupaevPicker;
    Switch aktiivneSwitch;

    BoxView bv_klaas;
    Frame f_klaas;

    CollectionView listView;

    public VeejalgiminePage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VeejalgimineDatabase(dbPath);

        Title = AppResources.WaterTracking;

        kogusEntry = new Entry
        {
            Placeholder = AppResources.WaterAmountPlaceholder,
            Keyboard = Keyboard.Numeric
        };

        kogusEntry.TextChanged += KogusEntry_TextChanged;

        kuupaevPicker = new DatePicker { Date = DateTime.Now };
        aktiivneSwitch = new Switch { IsToggled = true };

        f_klaas = new Frame
        {
            CornerRadius = 25,
            HeightRequest = 250,
            WidthRequest = 140,
            HasShadow = false,
            Padding = 0,
            Content = new Grid
            {
                Children =
                {
                    new BoxView
                    {
                        Color = Colors.LightGray,
                        Opacity = 0.2
                    },
                    (bv_klaas = new BoxView
                    {
                        Color = Colors.DodgerBlue,
                        VerticalOptions = LayoutOptions.End,
                        HeightRequest = 0
                    })
                }
            }
        };

        listView = new CollectionView
        {
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 10
            },
            ItemTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    CornerRadius = 15,
                    Padding = 12,
                    HasShadow = false,
                    Margin = new Thickness(0, 5)
                };

                var kogus = new Label { FontAttributes = FontAttributes.Bold, FontSize = 16 };
                kogus.SetBinding(Label.TextProperty,
                    new Binding("Kogus", stringFormat: $"💧 {{0}} {AppResources.Ml}"));

                var kuupaev = new Label { TextColor = Colors.Gray, FontSize = 12 };
                kuupaev.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));

                frame.Content = new VerticalStackLayout
                {
                    Spacing = 3,
                    Children = { kogus, kuupaev }
                };

                return frame;
            })
        };

        var btn_salvesta = new Button
        {
            Text = $"💾 {AppResources.Save}",
            BackgroundColor = Colors.MediumPurple,
            TextColor = Colors.White,
            CornerRadius = 15,
            HeightRequest = 50
        };

        btn_salvesta.Clicked += Btn_salvesta_Clicked;

        var btn_graafik = new Button
        {
            Text = $"📊 {AppResources.ShowGraph}",
            BackgroundColor = Colors.DodgerBlue,
            TextColor = Colors.White,
            CornerRadius = 15,
            HeightRequest = 50
        };

        btn_graafik.Clicked += async (s, e) =>
        {
            var andmed = database.GetVeejalgimine()
                .GroupBy(v => v.Kuupaev.Date)
                .Select(g => new VeejalgimineClass
                {
                    Kuupaev = g.Key,
                    Kogus = g.Sum(x => x.Kogus),
                    Aktiivne = g.Any(x => x.Aktiivne)
                })
                .OrderBy(v => v.Kuupaev)
                .ToList();

            await Navigation.PushAsync(new VeejalgimineGrafikPage(andmed));
        };

        var formCard = new Frame
        {
            CornerRadius = 20,
            Padding = 15,
            HasShadow = false,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label { Text = AppResources.Date, FontAttributes = FontAttributes.Bold },
                    kuupaevPicker,

                    new Label { Text = AppResources.Amount, FontAttributes = FontAttributes.Bold },
                    kogusEntry,

                    new Label { Text = AppResources.Active, FontAttributes = FontAttributes.Bold },
                    aktiivneSwitch,

                    new Label { Text = AppResources.WaterLevel, FontAttributes = FontAttributes.Bold },
                    f_klaas,

                    btn_salvesta
                }
            }
        };

        var mainStack = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 15,
            Children =
            {
                formCard,
                btn_graafik,
                new Label
                {
                    Text = AppResources.History,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                },
                listView
            }
        };

        Content = new ScrollView
        {
            Content = mainStack
        };

        LoadData();
    }

    private void KogusEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(kogusEntry.Text, out int kogus))
            UpdateKlaasImg(kogus);
        else
            UpdateKlaasImg(0);
    }

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (!int.TryParse(kogusEntry.Text, out int kogus) || kogus <= 0)
        {
            await DisplayAlert(AppResources.Error, AppResources.InvalidWater, AppResources.OK);
            return;
        }

        var paev = kuupaevPicker.Date.Date;

        var kirje = database.GetVeejalgimine()
            .FirstOrDefault(v => v.Kuupaev.Date == paev);

        if (kirje != null)
        {
            kirje.Kogus += kogus;
            kirje.Aktiivne = true;
            database.SaveVeejalgimine(kirje);
        }
        else
        {
            database.SaveVeejalgimine(new VeejalgimineClass
            {
                Kuupaev = paev,
                Kogus = kogus,
                Aktiivne = true
            });
        }

        ClearForm();
        LoadData();
    }

    private void LoadData()
    {
        var andmed = database.GetVeejalgimine()
            .GroupBy(v => v.Kuupaev.Date)
            .Select(g => new VeejalgimineClass
            {
                Kuupaev = g.Key,
                Kogus = g.Sum(x => x.Kogus),
                Aktiivne = g.Any(x => x.Aktiivne)
            })
            .OrderByDescending(v => v.Kuupaev)
            .ToList();

        listView.ItemsSource = andmed;

        var paev = kuupaevPicker.Date.Date;

        int kokku = andmed
            .Where(v => v.Kuupaev.Date == paev)
            .Sum(v => v.Kogus);

        UpdateKlaasImg(kokku);
    }

    private void UpdateKlaasImg(int kogus)
    {
        double max = 2000;
        double protsent = Math.Min(kogus / max, 1.0);
        bv_klaas.HeightRequest = protsent * 250;
    }

    private void ClearForm()
    {
        kogusEntry.Text = "";
        aktiivneSwitch.IsToggled = true;
    }
}