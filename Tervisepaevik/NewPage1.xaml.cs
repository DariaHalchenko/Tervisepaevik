using Tervisepaevik.View;

namespace Tervisepaevik;

public partial class NewPage1 : ContentPage
{
    // Список обычных страниц (без Flyout_Page)
    public List<ContentPage> lehed = new List<ContentPage>()
    {
        new TervitatavPage(),
        new TreeningudPage(),
        new EnesetunnePage(),
        new VeejalgiminePage()
    };

    // Отображаемые элементы с текстом и изображениями
    public List<(string Tekst, string Pilt)> valikud = new List<(string, string)>
    {
        ("Tervitus", "tervitus.png"),
        ("Menüü", "menuu.png"),
        ("Treeningud", "trener.png"),
        ("Enesetunne", "enesetunne.png"),
        ("Vee jälgimine", "veejalgimine.png")
    };

    public NewPage1()
    {
        Title = "Tervise Päevik";

        ScrollView sv = new ScrollView();
        VerticalStackLayout vsl = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20,
            BackgroundColor = Color.FromArgb("#FFF0F5")
        };

        for (int i = 0; i < valikud.Count; i++)
        {
            var frame = new Frame
            {
                BorderColor = Colors.LightGray,
                CornerRadius = 20,
                HasShadow = true,
                BackgroundColor = Colors.White,
                Padding = 10
            };

            var imgButton = new ImageButton
            {
                Source = valikud[i].Pilt,
                HeightRequest = 100,
                WidthRequest = 100,
                Aspect = Aspect.AspectFit,
                BackgroundColor = Colors.Transparent,
                HorizontalOptions = LayoutOptions.Center,
                CornerRadius = 15
            };

            var label = new Label
            {
                Text = valikud[i].Tekst,
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.DarkMagenta,
                HorizontalOptions = LayoutOptions.Center
            };

            int index = i; // Захват текущего индекса
            imgButton.Clicked += async (s, e) =>
            {
                if (valikud[index].Tekst == "Menüü")
                {
                    // Меняем корневую страницу на Flyout_Page
                    Application.Current.MainPage = new Flyout_Page();
                }
                else
                {
                    // Смещаем индекс, потому что Flyout_Page исключена из lehed
                    int realIndex = index > 1 ? index - 1 : index;
                    await Navigation.PushAsync(lehed[realIndex]);
                }
            };

            frame.Content = new VerticalStackLayout
            {
                Children = { imgButton, label }
            };

            vsl.Children.Add(frame);
        }

        sv.Content = vsl;
        Content = sv;
    }
}