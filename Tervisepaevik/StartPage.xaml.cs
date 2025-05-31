using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.View;

namespace Tervisepaevik;

public partial class StartPage : ContentPage
{
    ImageButton btn_hommikusook, btn_louna, btn_ohtusook, btn_vahepala;
    Label lbl_hommikusook, lbl_louna, lbl_ohtusook, lbl_vahepala;

    string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");

    private List<DayFoodGroup> GetNadalaToidud(string dbPath)
    {
        DateTime startDate = DateTime.Today; // Сегодня
        DateTime endDate = DateTime.Today.AddDays(6); // Через 6 дней

        var hommik = new HommikusookDatabase(dbPath).GetHommikusook().Where(x => x.Kuupaev >= startDate && x.Kuupaev <= endDate);
        var louna = new LounasookDatabase(dbPath).GetLounasook().Where(x => x.Kuupaev >= startDate && x.Kuupaev <= endDate);
        var ohtu = new OhtusookDatabase(dbPath).GetOhtusook().Where(x => x.Kuupaev >= startDate && x.Kuupaev <= endDate);
        var vahepala = new VahepalaDatabase(dbPath).GetVahepala().Where(x => x.Kuupaev >= startDate && x.Kuupaev <= endDate);

        var allMeals = new List<ToidukorradClass>();
        allMeals.AddRange(hommik.Select(x => new ToidukorradClass
        {
            Tuup = "Hommikusöök",
            Roa_nimi = x.Roa_nimi,
            Kuupaev = x.Kuupaev,
            Kalorid = x.Kalorid,
            Toidu_foto = x.Toidu_foto
        }));
        allMeals.AddRange(louna.Select(x => new ToidukorradClass
        {
            Tuup = "Lõuna",
            Roa_nimi = x.Roa_nimi,
            Kuupaev = x.Kuupaev,
            Kalorid = x.Kalorid,
            Toidu_foto = x.Toidu_foto
        }));
        allMeals.AddRange(ohtu.Select(x => new ToidukorradClass
        {
            Tuup = "Õhtusöök",
            Roa_nimi = x.Roa_nimi,
            Kuupaev = x.Kuupaev,
            Kalorid = x.Kalorid,
            Toidu_foto = x.Toidu_foto
        }));
        allMeals.AddRange(vahepala.Select(x => new ToidukorradClass
        {
            Tuup = "Vahepala",
            Roa_nimi = x.Roa_nimi,
            Kuupaev = x.Kuupaev,
            Kalorid = x.Kalorid,
            Toidu_foto = x.Toidu_foto
        }));

        return Enumerable.Range(0, 7) // Сегодня + 6 дней
            .Select(offset =>
            {
                var date = startDate.AddDays(offset);
                var foods = allMeals.Where(x => x.Kuupaev.Date == date).ToList();
                return new DayFoodGroup { DateTime = date, Foods = foods };
            })
            .Where(group => group.Foods.Any()) // Исключаем пустые дни
            .ToList();
    }


    public class DayFoodGroup
    {
        public DateTime DateTime { get; set; }
        public List<ToidukorradClass> Foods { get; set; }
    }

    public StartPage()
    {
        var toidud = GetNadalaToidud(dbPath);

        var carousel = new CarouselView
        {
            ItemsSource = toidud,
            HeightRequest = 380,
            ItemTemplate = new DataTemplate(() =>
            {
                var dateLabel = new Label
                {
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center
                };
                dateLabel.SetBinding(Label.TextProperty, new Binding("DateTime", stringFormat: "{0:dd.MM.yyyy}"));

                var foodsLayout = new StackLayout();
                foodsLayout.SetBinding(BindableLayout.ItemsSourceProperty, "Foods");

                BindableLayout.SetItemTemplate(foodsLayout, new DataTemplate(() =>
                {
                    var image = new Image
                    {
                        HeightRequest = 50,
                        WidthRequest = 50,
                        Aspect = Aspect.AspectFill,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    image.SetBinding(Image.SourceProperty, "FotoSource");

                    var typeLabel = new Label
                    {
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16
                    };
                    typeLabel.SetBinding(Label.TextProperty, "Tuup");

                    var nameLabel = new Label();
                    nameLabel.SetBinding(Label.TextProperty, "Roa_nimi");

                    var kcalLabel = new Label();
                    kcalLabel.SetBinding(Label.TextProperty, new Binding("Kalorid", stringFormat: "{0} kcal"));

                    var textStack = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children = { typeLabel, nameLabel, kcalLabel }
                    };

                    return new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = { image, textStack },
                        Margin = new Thickness(0, 10, 0, 10)
                    };
                }));

                var frame = new Frame
                {
                    CornerRadius = 15,
                    BackgroundColor = Colors.Beige,
                    Padding = 10,
                    Margin = new Thickness(10),
                    Content = new StackLayout
                    {
                        Children = { dateLabel, foodsLayout }
                    }
                };

                return frame;
            })
        };

        btn_hommikusook = new ImageButton
        {
            Source = "hommikusook.jpg",
            WidthRequest = 100,
            HeightRequest = 100,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btn_hommikusook.Clicked += Btn_hommikusook_Clicked;

        btn_louna = new ImageButton
        {
            Source = "louna.jpg",
            WidthRequest = 100,
            HeightRequest = 100,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btn_louna.Clicked += Btn_louna_Clicked;

        btn_ohtusook = new ImageButton
        {
            Source = "ohtusook.jpg",
            WidthRequest = 100,
            HeightRequest = 100,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btn_ohtusook.Clicked += Btn_ohtusook_Clicked;

        btn_vahepala = new ImageButton
        {
            Source = "vahepala.jpg",
            WidthRequest = 100,
            HeightRequest = 100,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btn_vahepala.Clicked += Btn_vahepala_Clicked;

        lbl_hommikusook = new Label { Text = "Hommikusöök", HorizontalOptions = LayoutOptions.Center };
        lbl_louna = new Label { Text = "Lõuna", HorizontalOptions = LayoutOptions.Center };
        lbl_ohtusook = new Label { Text = "Õhtusöök", HorizontalOptions = LayoutOptions.Center };
        lbl_vahepala = new Label { Text = "Vahepala", HorizontalOptions = LayoutOptions.Center };

        var grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        grid.Add(carousel, 0, 0);
        Grid.SetColumnSpan(carousel, 2);

        grid.Add(btn_hommikusook, 0, 1);
        grid.Add(btn_louna, 1, 1);
        grid.Add(lbl_hommikusook, 0, 2);
        grid.Add(lbl_louna, 1, 2);
        grid.Add(btn_ohtusook, 0, 3);
        grid.Add(btn_vahepala, 1, 3);
        grid.Add(lbl_ohtusook, 0, 4);
        grid.Add(lbl_vahepala, 1, 4);

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = new Thickness(20),
                Children = { grid }
            }
        };
    }

    private async void Btn_vahepala_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new VahepalaFotoPage());
    }

    private async void Btn_ohtusook_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new OhtusookFotoPage());
    }

    private async void Btn_louna_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new LounasookFotoPage());
    }

    private async void Btn_hommikusook_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new HommikusookFotoPage());
    }
}
