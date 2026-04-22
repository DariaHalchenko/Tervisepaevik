using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System.IO;
using Tervisepaevik.Database;
using MauiView = Microsoft.Maui.Controls.View;

namespace Tervisepaevik.View;

public partial class InfoPage : ContentPage
{
    private VeejalgimineDatabase vesiDb;
    private HommikusookDatabase hommikDb;
    private LounasookDatabase lounasDb;
    private OhtusookDatabase ohtuDb;
    private VahepalaDatabase vahepalaDb;
    private TreeningudDatabase treeningDb;
    private EnesetunneDatabase enesetunneDb;

    private Switch showWater, showFood, showKcal, showMood, showWorkout;
    private DatePicker datePicker;
    private bool isFilterActive = false;

    private VerticalStackLayout cardsLayout;

    public InfoPage()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");

        vesiDb = new VeejalgimineDatabase(path);
        hommikDb = new HommikusookDatabase(path);
        lounasDb = new LounasookDatabase(path);
        ohtuDb = new OhtusookDatabase(path);
        vahepalaDb = new VahepalaDatabase(path);
        treeningDb = new TreeningudDatabase(path);
        enesetunneDb = new EnesetunneDatabase(path);

        Title = "Dashboard";

        datePicker = new DatePicker { Date = DateTime.Today };

        datePicker.DateSelected += (s, e) =>
        {
            isFilterActive = true;
            LoadData();
        };

        showWater = CreateSwitch(true);
        showFood = CreateSwitch(true);
        showKcal = CreateSwitch(true);
        showMood = CreateSwitch(true);
        showWorkout = CreateSwitch(true);

        cardsLayout = new VerticalStackLayout { Spacing = 20 };

        var mainLayout = new VerticalStackLayout
        {
            Padding = 15,
            Spacing = 15,
            Children =
            {
                CreateTopSection(),
                cardsLayout
            }
        };

        // 🔥 AbsoluteLayout для кнопки
        var absolute = new AbsoluteLayout();

        var scroll = new ScrollView { Content = mainLayout };

        AbsoluteLayout.SetLayoutBounds(scroll, new Rect(0, 0, 1, 1));
        AbsoluteLayout.SetLayoutFlags(scroll, AbsoluteLayoutFlags.All);

        absolute.Children.Add(scroll);

        // 🔘 КНОПКА edasi
        var edasiButton = new ImageButton
        {
            Source = "edasi.svg",
            WidthRequest = 60,
            HeightRequest = 60,
            BackgroundColor = Colors.Transparent
        };

        edasiButton.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new NewPage1());
        };

        AbsoluteLayout.SetLayoutBounds(edasiButton, new Rect(1, 1, 70, 70));
        AbsoluteLayout.SetLayoutFlags(edasiButton, AbsoluteLayoutFlags.PositionProportional);

        absolute.Children.Add(edasiButton);

        Content = absolute;

        LoadData();
    }

    private Switch CreateSwitch(bool state)
    {
        var sw = new Switch { IsToggled = state };
        sw.Toggled += (s, e) => LoadData();
        return sw;
    }

    private MauiView CreateTopSection()
    {
        return new VerticalStackLayout
        {
            Spacing = 10,
            Children =
            {
                CreateFilterCard(),
                CreateShowCard()
            }
        };
    }

    private Frame CreateFilterCard()
    {
        datePicker.FontSize = 13;

        var resetButton = new Button
        {
            Text = "Reset",
            FontSize = 12,
            Padding = new Thickness(8, 2),
            HeightRequest = 30
        };

        resetButton.Clicked += (s, e) =>
        {
            isFilterActive = false;
            LoadData();
        };

        return new Frame
        {
            CornerRadius = 20,
            Padding = new Thickness(10, 8),
            HasShadow = true,

            Content = new HorizontalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Filter",
                        FontAttributes = FontAttributes.Bold
                    },
                    new Label { Text = "📅", FontSize = 16 },
                    datePicker,
                    resetButton
                }
            }
        };
    }

    private Frame CreateShowCard()
    {
        return new Frame
        {
            CornerRadius = 20,
            Padding = new Thickness(10, 8),
            HasShadow = true,

            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star },
                    new ColumnDefinition{ Width = GridLength.Star }
                },

                Children =
                {
                    CreateShowItem("💧","Water", showWater,0),
                    CreateShowItem("🍽","Food", showFood,1),
                    CreateShowItem("🔥","Kcal", showKcal,2),
                    CreateShowItem("😊","Mood", showMood,3),
                    CreateShowItem("🏋️","Workout", showWorkout,4)
                }
            }
        };
    }

    private MauiView CreateShowItem(string icon, string text, Switch sw, int col)
    {
        sw.Scale = 0.8;

        var layout = new VerticalStackLayout
        {
            Spacing = 4,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = icon, FontSize = 18, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = text, FontSize = 11, HorizontalOptions = LayoutOptions.Center },
                sw
            }
        };

        Grid.SetColumn(layout, col);
        return layout;
    }

    private void LoadData()
    {
        cardsLayout.Children.Clear();

        var dates = GetAllDates();

        foreach (var d in dates.OrderByDescending(x => x))
        {
            if (isFilterActive && d.Date != datePicker.Date)
                continue;

            var card = CreateCard(d);

            if (card != null)
                cardsLayout.Children.Add(card);
        }
    }

    private List<DateTime> GetAllDates()
    {
        return vesiDb.GetVeejalgimine().Select(x => x.Kuupaev.Date)
            .Union(hommikDb.GetHommikusook().Select(x => x.Kuupaev.Date))
            .Union(lounasDb.GetLounasook().Select(x => x.Kuupaev.Date))
            .Union(ohtuDb.GetOhtusook().Select(x => x.Kuupaev.Date))
            .Union(vahepalaDb.GetVahepala().Select(x => x.Kuupaev.Date))
            .Union(treeningDb.GetTreeningud().Select(x => x.Kuupaev.Date))
            .Union(enesetunneDb.GetEnesetunne().Select(x => x.Kuupaev.Date))
            .Distinct()
            .ToList();
    }

    private Frame CreateCard(DateTime date)
    {
        if (!showWater.IsToggled &&
            !showFood.IsToggled &&
            !showKcal.IsToggled &&
            !showMood.IsToggled &&
            !showWorkout.IsToggled)
            return null;

        var hommik = hommikDb.GetHommikusook().FirstOrDefault(x => x.Kuupaev.Date == date);
        var lounas = lounasDb.GetLounasook().FirstOrDefault(x => x.Kuupaev.Date == date);
        var ohtu = ohtuDb.GetOhtusook().FirstOrDefault(x => x.Kuupaev.Date == date);
        var snack = vahepalaDb.GetVahepala().FirstOrDefault(x => x.Kuupaev.Date == date);

        int water = vesiDb.GetVeejalgimine().Where(x => x.Kuupaev.Date == date).Sum(x => x.Kogus);

        int kcal =
            (hommik?.Kalorid ?? 0) +
            (lounas?.Kalorid ?? 0) +
            (ohtu?.Kalorid ?? 0) +
            (snack?.Kalorid ?? 0);

        var mood = enesetunneDb.GetEnesetunne().FirstOrDefault(x => x.Kuupaev.Date == date);
        bool workout = treeningDb.GetTreeningud().Any(x => x.Kuupaev.Date == date);

        var layout = new VerticalStackLayout { Spacing = 10 };

        layout.Children.Add(new Label
        {
            Text = date.ToString("dd.MM.yyyy"),
            HorizontalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            FontSize = 18
        });

        var statsGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition{ Width = GridLength.Star },
                new ColumnDefinition{ Width = GridLength.Star },
                new ColumnDefinition{ Width = GridLength.Star },
                new ColumnDefinition{ Width = GridLength.Star }
            }
        };

        int col = 0;

        if (showWater.IsToggled)
        {
            var lbl = new Label { Text = $"💧 {water} ml", HorizontalOptions = LayoutOptions.Center };
            statsGrid.Children.Add(lbl);
            Grid.SetColumn(lbl, col++);
        }

        if (showKcal.IsToggled)
        {
            var lbl = new Label { Text = $"🔥 {kcal} kcal", HorizontalOptions = LayoutOptions.Center };
            statsGrid.Children.Add(lbl);
            Grid.SetColumn(lbl, col++);
        }

        if (showMood.IsToggled)
        {
            var lbl = new Label
            {
                Text = mood != null ? $"😊 {mood.Tuju}/5" : "😊 -",
                HorizontalOptions = LayoutOptions.Center
            };
            statsGrid.Children.Add(lbl);
            Grid.SetColumn(lbl, col++);
        }

        if (showWorkout.IsToggled)
        {
            var lbl = new Label
            {
                Text = workout ? "🏋️ Yes" : "🏋️ No",
                HorizontalOptions = LayoutOptions.Center
            };
            statsGrid.Children.Add(lbl);
            Grid.SetColumn(lbl, col++);
        }

        if (statsGrid.Children.Count > 0)
            layout.Children.Add(statsGrid);

        if (showFood.IsToggled)
        {
            var foodRow = new HorizontalStackLayout
            {
                Spacing = 15,
                HorizontalOptions = LayoutOptions.Center
            };

            if (hommik != null)
                foodRow.Children.Add(CreateFoodCard(hommik.Toidu_foto, "Breakfast", hommik.Kalorid));

            if (lounas != null)
                foodRow.Children.Add(CreateFoodCard(lounas.Toidu_foto, "Lunch", lounas.Kalorid));

            if (ohtu != null)
                foodRow.Children.Add(CreateFoodCard(ohtu.Toidu_foto, "Dinner", ohtu.Kalorid));

            if (snack != null)
                foodRow.Children.Add(CreateFoodCard(snack.Toidu_foto, "Snack", snack.Kalorid));

            if (foodRow.Children.Count > 0)
                layout.Children.Add(foodRow);
        }

        return new Frame
        {
            CornerRadius = 25,
            Padding = 15,
            HasShadow = true,
            Content = layout
        };
    }

    private MauiView CreateFoodCard(byte[] imageBytes, string title, int kcal)
    {
        var image = new Image
        {
            HeightRequest = 70,
            WidthRequest = 70,
            Aspect = Aspect.AspectFill
        };

        if (imageBytes != null)
            image.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

        return new VerticalStackLayout
        {
            Spacing = 5,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = title, FontSize = 12, HorizontalOptions = LayoutOptions.Center },
                new Frame
                {
                    CornerRadius = 15,
                    Padding = 0,
                    IsClippedToBounds = true,
                    Content = image
                },
                new Label { Text = $"{kcal} kcal", FontSize = 12, HorizontalOptions = LayoutOptions.Center }
            }
        };
    }
}