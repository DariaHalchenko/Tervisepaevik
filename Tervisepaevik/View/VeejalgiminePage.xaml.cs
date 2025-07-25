﻿using Microsoft.Maui.Layouts;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class VeejalgiminePage : ContentPage
{
    VeejalgimineDatabase database;

    Entry kogusEntry;
    DatePicker kuupaevPicker;
    Switch aktiivneSwitch;
    Button salvestaButton, kustutaButton, uusSisestusButton, avaGraafikButton;
    ListView veejalgimineListView;
    BoxView bv_klaas;
    Frame f_klaas;
    private ImageButton btn_salvesta, btn_menu, btn_ajavaka, btn_trener;
    VeejalgimineClass selectedItem;
    private StackLayout sl;


    public VeejalgiminePage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VeejalgimineDatabase(dbPath);

        Title = "Vee jälgimine";

        kogusEntry = new Entry { Placeholder = "Joodud vee kogus (ml)", Keyboard = Keyboard.Numeric };
        kuupaevPicker = new DatePicker { Date = DateTime.Now };
        kuupaevPicker.DateSelected += KuupaevPicker_DateSelected;
        aktiivneSwitch = new Switch { IsToggled = true };

        salvestaButton = new Button { Text = "Salvesta" };
        kustutaButton = new Button { Text = "Kustuta", IsVisible = false };
        uusSisestusButton = new Button { Text = "Uus sisestus" };
        avaGraafikButton = new Button { Text = "Ava graafik" };

        veejalgimineListView = new ListView
        {
            ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, new Binding("Kogus", stringFormat: "Kogus: {0} ml"));
                textCell.SetBinding(TextCell.DetailProperty, new Binding("Kuupaev", stringFormat: "{0:d}"));
                return textCell;
            })
        };

        btn_salvesta = new ImageButton
        {
            Source = "salvesta.png",
            BackgroundColor = Colors.LightGreen,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 22
        };

        btn_ajavaka = new ImageButton
        {
            Source = "ajakava.png",
            BackgroundColor = Colors.Aqua,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 22
        };
        btn_trener = new ImageButton
        {
            Source = "trener.png",
            BackgroundColor = Colors.LightCoral,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 22
        };
        btn_menu = new ImageButton
        {
            Source = "menu.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 55,
            WidthRequest = 55,
            CornerRadius = 30,
            Shadow = new Shadow
            {
                Opacity = 0.3f,
                Radius = 10,
                Offset = new Point(3, 3)
            }
        };

        veejalgimineListView.ItemSelected += VeejalgimineListView_ItemSelected;
        kustutaButton.Clicked += KustutaButton_Clicked;
        uusSisestusButton.Clicked += UusSisestusButton_Clicked;
        avaGraafikButton.Clicked += AvaGraafikButton_Clicked;

        btn_trener.Clicked += Btn_trener_Clicked;
        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_menu.Clicked += Btn_menu_Clicked;
        btn_ajavaka.Clicked += Btn_ajavaka_Clicked;

        sl = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 15,
            IsVisible = false,
            Children = { btn_salvesta, btn_ajavaka, btn_trener },
            Margin = new Thickness(0, 0, 0, 10)
        };


        f_klaas = new Frame
        {
            BorderColor = Colors.Blue,
            HeightRequest = 300,
            WidthRequest = 150,
            Content = new Grid
            {
                Children =
                    {
                        (bv_klaas = new BoxView
                        {
                            Color = Colors.LightBlue,
                            VerticalOptions = LayoutOptions.End,
                            HeightRequest = 0
                        })
                    }
            }
        };

        var scrollContent = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 20,
                Children =
                {
                    new Label { Text = "Kuupäev" },
                    kuupaevPicker,
                    new Label { Text = "Kogus (ml)" },
                    kogusEntry,
                    new Label { Text = "Aktiivne" },
                    aktiivneSwitch,
                    new Label { Text = "Klaas" },
                    f_klaas,
                    veejalgimineListView
                }
            }
        };

        var absoluteLayout = new AbsoluteLayout();

        AbsoluteLayout.SetLayoutFlags(scrollContent, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(scrollContent, new Rect(0, 0, 1, 1));
        absoluteLayout.Children.Add(scrollContent);

        AbsoluteLayout.SetLayoutFlags(sl, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(sl, new Rect(0.60, 0.95, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        absoluteLayout.Children.Add(sl);

        AbsoluteLayout.SetLayoutFlags(btn_menu, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(btn_menu, new Rect(0.95, 0.95, 60, 60));
        absoluteLayout.Children.Add(btn_menu);

        Content = absoluteLayout;


        LoadData();
    }

    private async void Btn_ajavaka_Clicked(object? sender, EventArgs e)
    {
        var andmed = database.GetVeejalgimine()
            .OrderBy(v => v.Kuupaev)
            .ToList();

        await Navigation.PushAsync(new VeejalgimineGrafikPage(andmed));
    }

    private void Btn_menu_Clicked(object? sender, EventArgs e)
    {
        sl.IsVisible = !sl.IsVisible;
    }

    private async void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (!aktiivneSwitch.IsToggled)
        {
            var valitud_paev = kuupaevPicker.Date.Date;

            var salvestamine = database.GetVeejalgimine()
                .FirstOrDefault(v => v.Kuupaev.Date == valitud_paev);

            if (salvestamine != null)
            {
                salvestamine.Kogus = 0;
                salvestamine.Aktiivne = false;
                database.SaveVeejalgimine(salvestamine);
            }
            else
            {
                var uus_kirje = new VeejalgimineClass
                {
                    Kuupaev = valitud_paev,
                    Kogus = 0,
                    Aktiivne = false
                };
                database.SaveVeejalgimine(uus_kirje);
            }

            ClearForm();
            LoadData();
            return;
        }

        if (!int.TryParse(kogusEntry.Text, out int kogus) || kogus <= 0)
        {
            await DisplayAlert("Viga", "Sisesta korrektne vee kogus.", "OK");
            return;
        }

        var valitud_paev_active = kuupaevPicker.Date.Date;

        var salvestamineActive = database.GetVeejalgimine()
            .FirstOrDefault(v => v.Kuupaev.Date == valitud_paev_active);

        const int norm = 2000;

        if (salvestamineActive != null)
        {
            int uus_summa = salvestamineActive.Kogus + kogus;
            if (uus_summa > norm)
            {
                await DisplayAlert("Täis!", "Tänaseks on vee norm juba täis (2000ml).", "OK");
                return;
            }

            salvestamineActive.Kogus += kogus;
            salvestamineActive.Aktiivne = true;
            database.SaveVeejalgimine(salvestamineActive);
        }
        else
        {
            if (kogus > norm)
            {
                await DisplayAlert("Liiga palju", "Sisestatud kogus ületab päeva normi.", "OK");
                return;
            }

            var uus_kirje = new VeejalgimineClass
            {
                Kuupaev = valitud_paev_active,
                Kogus = kogus,
                Aktiivne = true
            };
            database.SaveVeejalgimine(uus_kirje);
        }

        ClearForm();
        LoadData();
    }

    private async void Btn_trener_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new TreeningudFotoPage());
    }

    private async void AvaGraafikButton_Clicked(object? sender, EventArgs e)
    {
        var andmed = database.GetVeejalgimine()
            .OrderBy(v => v.Kuupaev)
            .ToList();

        await Navigation.PushAsync(new VeejalgimineGrafikPage(andmed));
    }


    private void KuupaevPicker_DateSelected(object? sender, DateChangedEventArgs e)
    {
        LoadData();
    }

    

    private void KustutaButton_Clicked(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            var kuupaev = selectedItem.Kuupaev.Date;
            var kirjed = database.GetVeejalgimine().Where(v => v.Kuupaev.Date == kuupaev).ToList();

            foreach (var kirje in kirjed)
                database.DeleteVeejalgimine(kirje.Veejalgimine_id);

            ClearForm();
            LoadData();
        }
    }

    private void VeejalgimineListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as VeejalgimineClass;
        if (selectedItem != null)
        {
            kuupaevPicker.Date = selectedItem.Kuupaev;
            kogusEntry.Text = selectedItem.Kogus.ToString();
            aktiivneSwitch.IsToggled = selectedItem.Aktiivne;
            kustutaButton.IsVisible = true;
        }
    }

    private void UusSisestusButton_Clicked(object sender, EventArgs e)
    {
        ClearForm();
    }

    private void ClearForm()
    {
        selectedItem = null;
        kuupaevPicker.Date = DateTime.Now;
        kogusEntry.Text = string.Empty;
        aktiivneSwitch.IsToggled = true;
        veejalgimineListView.SelectedItem = null;
        kustutaButton.IsVisible = false;
        bv_klaas.HeightRequest = 0;
    }

    private void LoadData()
    {
        var koik_andmed = database.GetVeejalgimine()
            .GroupBy(v => v.Kuupaev.Date)
            .Select(g => new VeejalgimineClass
            {
                Kuupaev = g.Key,
                Kogus = g.Sum(x => x.Kogus),
                Aktiivne = g.Any(x => x.Aktiivne)
            })
            .OrderByDescending(v => v.Kuupaev)
            .ToList();

        veejalgimineListView.ItemsSource = koik_andmed;

        var paev = kuupaevPicker.Date.Date;
        int kokku = koik_andmed
            .Where(v => v.Kuupaev.Date == paev && v.Aktiivne)
            .Sum(v => v.Kogus);

        UpdateKlaasImg(kokku);
    }

    private void UpdateKlaasImg(int kogus)
    {
        double max = 2000;
        double protsent = Math.Min(kogus / max, 1.0);
        bv_klaas.HeightRequest = protsent * 300;
    }
}