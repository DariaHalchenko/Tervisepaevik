using Microsoft.Maui.Layouts;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class VeejalgiminePage : ContentPage
{
    VeejalgimineDatabase database;

    DatePicker kuupaevPicker;
    Switch aktiivneSwitch;

    CollectionView listView;

    const int KlaasideArv = 24;
    const int KlaasiMaht = 200;

    List<Image> klaasid = new();
    int valitudKlaasid = 0;

    Label progressLabel;
    Label goalLabel;

    public VeejalgiminePage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VeejalgimineDatabase(dbPath);

        Title = AppResources.WaterTracking;

        kuupaevPicker = new DatePicker { Date = DateTime.Now };
        kuupaevPicker.DateSelected += (s, e) => LoadData();

        aktiivneSwitch = new Switch { IsToggled = true };

        var infoLabel = new Label
        {
            Text = AppResources.WaterInfo,
            FontSize = 14,
            TextColor = Colors.Gray
        };

        progressLabel = new Label
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold
        };

        goalLabel = new Label
        {
            FontSize = 16
        };

        // Стаканы
        var klaasidLayout = new FlexLayout
        {
            Wrap = FlexWrap.Wrap,
            Direction = FlexDirection.Row,
            Margin = new Thickness(0, 10)
        };

        for (int i = 0; i < KlaasideArv; i++)
        {
            int index = i;

            var img = new Image
            {
                Source = "vesi.svg",
                HeightRequest = 40,
                WidthRequest = 40,
                Opacity = 0.3
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => KlaasTapped(index);

            img.GestureRecognizers.Add(tap);

            klaasid.Add(img);
            klaasidLayout.Children.Add(img);
        }

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

        // История
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

                    infoLabel,
                    progressLabel,
                    goalLabel,

                    klaasidLayout,
                    btn_salvesta
                }
            }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
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
            }
        };

        LoadData();
    }

    private void KlaasTapped(int index)
    {
        valitudKlaasid = index + 1;

        UpdateKlaasid();
        UpdateProgress();
    }

    private void UpdateKlaasid()
    {
        for (int i = 0; i < klaasid.Count; i++)
        {
            klaasid[i].Opacity = i < valitudKlaasid ? 1.0 : 0.3;
        }
    }

    private void UpdateProgress()
    {
        int kogus = valitudKlaasid * KlaasiMaht;
        int max = KlaasideArv * KlaasiMaht;

        progressLabel.Text = $"{kogus} / {max} ml";

        if (kogus >= max)
        {
            goalLabel.Text = AppResources.GoalReached;
            goalLabel.TextColor = Colors.Green;
        }
        else
        {
            goalLabel.Text = "";
        }
    }

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        int kogus = valitudKlaasid * KlaasiMaht;

        if (kogus <= 0)
        {
            await DisplayAlert(AppResources.Error, AppResources.SelectWaterAmount, "OK");
            return;
        }

        SaveOrUpdate(kogus);
        LoadData();

        await DisplayAlert("OK", AppResources.Saved, "OK");
    }

    private void SaveOrUpdate(int kogus)
    {
        var paev = kuupaevPicker.Date.Date;

        var kirje = database.GetVeejalgimine()
            .FirstOrDefault(v => v.Kuupaev.Date == paev);

        if (kirje != null)
        {
            kirje.Kogus = kogus;
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
    }

    private void LoadData()
    {
        var paev = kuupaevPicker.Date.Date;

        var kirje = database.GetVeejalgimine()
            .FirstOrDefault(v => v.Kuupaev.Date == paev);

        if (kirje != null)
        {
            valitudKlaasid = kirje.Kogus / KlaasiMaht;
        }
        else
        {
            valitudKlaasid = 0;
        }

        UpdateKlaasid();
        UpdateProgress();

        listView.ItemsSource = database.GetVeejalgimine()
            .GroupBy(v => v.Kuupaev.Date)
            .Select(g => new VeejalgimineClass
            {
                Kuupaev = g.Key,
                Kogus = g.Sum(x => x.Kogus),
                Aktiivne = g.Any(x => x.Aktiivne)
            })
            .OrderByDescending(v => v.Kuupaev)
            .ToList();
    }
}