using Tervisepaevik.Database;

namespace Tervisepaevik.View;

public partial class HommikusookFotoPage : ContentPage
{
    private HommikusookDatabase database;
    private Grid grid;

    public HommikusookFotoPage()
    {
        Title = "Hommikusöök";

        grid = new Grid
        {
            Padding = 10,
            RowSpacing = 10,
            ColumnSpacing = 10
        };

        for (int i = 0; i < 3; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new HommikusookDatabase(dbPath);

        LoadImages();

        var scrollview = new ScrollView
        {
            Content = grid
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
                    await Navigation.PushAsync(new HommikusookPage());
                })
            }
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
                        Children = { frame }
                    }
                }
        };
    }

    private void LoadImages()
    {
        grid.Children.Clear();
        grid.RowDefinitions.Clear();

        var imageList = database.GetHommikusook()
            .Where(x => x.Toidu_foto != null && x.Toidu_foto.Length > 0)
            .ToList();

        int rida = 0;
        int veerg = 0;

        foreach (var item in imageList)
        {
            if (veerg == 0)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            string tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"image_{item.Hommikusook_id}.jpg");
            File.WriteAllBytes(tempFilePath, item.Toidu_foto);

            var imagegrid = new Grid
            {
                HeightRequest = 100,
                WidthRequest = 100
            };

            var image = new Image
            {
                Source = ImageSource.FromFile(tempFilePath),
                Aspect = Aspect.AspectFill
            };

            var img_btn_kustuta = new ImageButton
            {
                Source = "kustuta.png",
                BackgroundColor = Colors.Transparent,
                HeightRequest = 24,
                WidthRequest = 24,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Padding = 0,
                Margin = new Thickness(0, 0, 0, 0)
            };

            img_btn_kustuta.Clicked += async (s, e) =>
            {
                bool answer = await DisplayAlert("Kinnita kustutamine", "Kas oled kindel, et soovid selle foto kustutada?", "Jah", "Ei");
                if (answer)
                {
                    database.DeleteHommikusook(item.Hommikusook_id);
                    LoadImages();
                }
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                string info = $"Roa nimi: {item.Roa_nimi}\n" +
                              $"Kuupäev: {item.Kuupaev:dd.MM.yyyy}\n" +
                              $"Kellaaeg: {item.Kallaaeg:hh\\:mm}\n" +
                              $"Valgud: {item.Valgud} g\n" +
                              $"Rasvad: {item.Rasvad} g\n" +
                              $"Süsivesikud: {item.Susivesikud} g\n" +
                              $"Kalorid: {item.Kalorid} kcal";

                await Application.Current.MainPage.DisplayAlert("Toiduandmed", info, "OK");
            };

            image.GestureRecognizers.Add(tap);

            imagegrid.Children.Add(image);
            imagegrid.Children.Add(img_btn_kustuta);

            grid.Children.Add(imagegrid);
            Grid.SetRow(imagegrid, rida);
            Grid.SetColumn(imagegrid, veerg);

            veerg++;
            if (veerg == 3)
            {
                veerg = 0;
                rida++;
            }
        }
    }
}
