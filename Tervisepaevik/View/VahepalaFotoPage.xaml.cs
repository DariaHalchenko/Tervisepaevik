using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class VahepalaFotoPage : ContentPage
{
    private VahepalaDatabase database;
    private Grid grid;
    private DatePicker filterDate;
    private Entry searchEntry;
    private Switch filterSwitch;
    private VerticalStackLayout filtersContainer;

    private bool isDateFilterActive = false;

    public VahepalaFotoPage()
    {
        Title = AppResources.Snack;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VahepalaDatabase(dbPath);

        filterDate = new DatePicker
        {
            Date = DateTime.Today,
            HeightRequest = 40
        };

        filterDate.DateSelected += (s, e) =>
        {
            isDateFilterActive = true;
            LoadImages();
        };

        searchEntry = new Entry
        {
            Placeholder = AppResources.SearchFood,
            HeightRequest = 40
        };

        searchEntry.TextChanged += (s, e) => LoadImages();

        filterSwitch = new Switch
        {
            IsToggled = false
        };

        filtersContainer = new VerticalStackLayout
        {
            Spacing = 6,
            IsVisible = false,
            Children =
            {
                filterDate,
                searchEntry
            }
        };

        filterSwitch.Toggled += (s, e) =>
        {
            filtersContainer.IsVisible = filterSwitch.IsToggled;
            LoadImages();
        };

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
            HeightRequest = 100,
            WidthRequest = 100,
            Command = new Command(async () =>
            {
                await Navigation.PushAsync(new VahepalaPage());
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
            Padding = 10,
            CornerRadius = 15,
            HasShadow = false,
            Margin = new Thickness(0, 0, 0, 5),

            Content = new VerticalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Children =
                        {
                            new Label
                            {
                                Text = AppResources.Filters,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 16,
                                VerticalOptions = LayoutOptions.Center
                            },
                            filterSwitch
                        }
                    },

                    filtersContainer
                }
            }
        };
    }

    private void LoadImages()
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        var data = database.GetVahepala()
            .Where(x => x.Toidu_foto != null && x.Toidu_foto.Length > 0)
            .ToList();

        if (filterSwitch.IsToggled)
        {
            if (!string.IsNullOrWhiteSpace(searchEntry.Text))
            {
                data = data
                    .Where(x => x.Roa_nimi.ToLower().Contains(searchEntry.Text.ToLower()))
                    .ToList();
            }

            if (isDateFilterActive)
            {
                data = data
                    .Where(x => x.Kuupaev.Date == filterDate.Date)
                    .ToList();
            }
        }

        int row = 0;
        int col = 0;

        foreach (var item in data)
        {
            if (col == 0)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var tempFile = Path.Combine(FileSystem.CacheDirectory, $"vahepala_{item.Vahepala_id}.jpg");

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
                Text = $"{item.Kalorid} {AppResources.Kcal}",
                Padding = 4,
                FontSize = 12
            };

            var deleteBtn = new ImageButton
            {
                Source = "kustuta.png",
                BackgroundColor = Colors.Transparent,
                HeightRequest = 22,
                WidthRequest = 22,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start
            };

            deleteBtn.Clicked += async (s, e) =>
            {
                bool confirm = await DisplayAlert(
                    AppResources.Delete,
                    AppResources.ConfirmDelete,
                    AppResources.Yes,
                    AppResources.No);

                if (confirm)
                {
                    database.DeleteVahepala(item.Vahepala_id);
                    LoadImages();
                }
            };

            var frame = new Frame
            {
                CornerRadius = 12,
                Padding = 0,
                HasShadow = false,
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
                await Navigation.PushAsync(new VahepalaDetailPage(item));
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