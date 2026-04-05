using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class OhtusookFotoPage : ContentPage
{
    private OhtusookDatabase database;
    private Grid grid;
    private DatePicker filterDate;
    private Entry searchEntry;

    public OhtusookFotoPage()
    {
        Title = "Õhtusöök";

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        filterDate = new DatePicker
        {
            Date = DateTime.Today
        };

        filterDate.DateSelected += (s, e) => LoadImages();

        searchEntry = new Entry
        {
            Placeholder = "Otsi rooga...",
        };

        searchEntry.TextChanged += (s, e) => LoadImages();

        grid = new Grid
        {
            Padding = 10,
            RowSpacing = 10,
            ColumnSpacing = 10
        };

        for (int i = 0; i < 3; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        var scrollview = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    CreateFilterCard(),
                    grid
                }
            }
        };

        LoadImages();

        var addBtn = new ImageButton
        {
            Source = "lisa.svg",
            CornerRadius = 30,
            HeightRequest = 60,
            WidthRequest = 60,
            Command = new Command(async () =>
            {
                await Navigation.PushAsync(new OhtusookPage());
            })
        };

        Content = new Grid
        {
            Children =
            {
                scrollview,
                new Grid
                {
                    VerticalOptions = LayoutOptions.End,
                    HorizontalOptions = LayoutOptions.End,
                    Padding = 20,
                    Children = { addBtn }
                }
            }
        };
    }

    private Frame CreateFilterCard()
    {
        return new Frame
        {
            Padding = 15,
            CornerRadius = 20,
            HasShadow = true,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Filtrid",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18
                    },
                    filterDate,
                    searchEntry
                }
            }
        };
    }

    private void LoadImages()
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        var data = database.GetOhtusook()
            .Where(x => x.Toidu_foto != null && x.Toidu_foto.Length > 0)
            .Where(x => x.Kuupaev.Date == filterDate.Date)
            .Where(x => string.IsNullOrWhiteSpace(searchEntry.Text) ||
                        x.Roa_nimi.ToLower().Contains(searchEntry.Text.ToLower()))
            .ToList();

        int row = 0;
        int col = 0;

        foreach (var item in data)
        {
            if (col == 0)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tempFile = Path.Combine(FileSystem.CacheDirectory, $"ohtusook_{item.Ohtusook_id}.jpg");

            if (!File.Exists(tempFile))
                File.WriteAllBytes(tempFile, item.Toidu_foto);

            var image = new Image
            {
                Source = ImageSource.FromFile(tempFile),
                Aspect = Aspect.AspectFill,
                HeightRequest = 120
            };

            var label = new Label
            {
                Text = $"{item.Kalorid} kcal",
                Padding = 4
            };

            var deleteBtn = new ImageButton
            {
                Source = "kustuta.png",
                BackgroundColor = Colors.Transparent,
                HeightRequest = 24,
                WidthRequest = 24,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };

            deleteBtn.Clicked += async (s, e) =>
            {
                bool confirm = await DisplayAlert("Kustuta", "Kas oled kindel?", "Jah", "Ei");
                if (confirm)
                {
                    database.DeleteOhtusook(item.Ohtusook_id);
                    LoadImages();
                }
            };

            var frame = new Frame
            {
                CornerRadius = 15,
                Padding = 0,
                HasShadow = true,
                Content = new Grid
                {
                    Children =
                    {
                        image,
                        new VerticalStackLayout
                        {
                            VerticalOptions = LayoutOptions.End,
                            Children = { label }
                        },
                        deleteBtn
                    }
                }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                await Navigation.PushAsync(new OhtusookDetailPage(item));
            };

            frame.GestureRecognizers.Add(tap);

            grid.Children.Add(frame);
            Grid.SetRow(frame, row);
            Grid.SetColumn(frame, col);

            col++;
            if (col == 3)
            {
                col = 0;
                row++;
            }
        }
    }
}