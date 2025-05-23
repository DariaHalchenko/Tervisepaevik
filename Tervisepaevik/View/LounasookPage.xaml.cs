using Microsoft.Maui.Layouts;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class LounasookPage : ContentPage
{
    private string lisafoto;
    private byte[] fotoBytes;
    private LounasookDatabase database;
    private LounasookClass selectedItem;

    private EntryCell ec_roaNimi, ec_valgud, ec_rasvad, ec_susivesikud, ec_kalorid;
    private DatePicker dp_kuupaev;
    private TimePicker tp_kallaaeg;
    private Image img;

    private TableView tableview;
    private TableSection fotoSection;

    private ImageButton btn_salvesta, btn_pildista, btn_valifoto, btn_menu, btn_vesi, btn_trener;
    private StackLayout sl;

    public LounasookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new LounasookDatabase(dbPath);

        Title = "Lounasöök";

        ec_roaNimi = new EntryCell { Label = "Roa nimi", Placeholder = "nt. Puder" };
        ec_valgud = new EntryCell { Label = "Valgud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_rasvad = new EntryCell { Label = "Rasvad", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_susivesikud = new EntryCell { Label = "Süsivesikud", Placeholder = "g", Keyboard = Keyboard.Numeric };
        ec_kalorid = new EntryCell { Label = "Kalorid", Placeholder = "kcal", Keyboard = Keyboard.Numeric };

        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(8) };

        btn_pildista = new ImageButton
        {
            Source = "foto.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 45,
            WidthRequest = 45
        };
        btn_valifoto = new ImageButton
        {
            Source = "valifoto.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 45,
            WidthRequest = 45
        };
        btn_salvesta = new ImageButton
        {
            Source = "salvesta.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 45,
            WidthRequest = 45
        };

        btn_vesi = new ImageButton
        {
            Source = "vesi.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 45,
            WidthRequest = 45
        };
        btn_trener = new ImageButton
        {
            Source = "trener.png",
            BackgroundColor = Colors.Transparent,
            HeightRequest = 45,
            WidthRequest = 45
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

        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_pildista.Clicked += Btn_pildista_Clicked;
        btn_valifoto.Clicked += Btn_valifoto_Clicked;
        btn_menu.Clicked += Btn_menu_Clicked;
        btn_vesi.Clicked += Btn_vesi_Clicked;
        btn_trener.Clicked += Btn_trener_Clicked;

        img = new Image();

        fotoSection = new TableSection("Foto");

        tableview = new TableView
        {
            Intent = TableIntent.Form,
            Root = new TableRoot("Sisesta Lounasöök")
            {
                new TableSection("Üldandmed")
                {
                    new ViewCell { View = dp_kuupaev },
                    new ViewCell { View = tp_kallaaeg },
                    ec_roaNimi,
                    ec_valgud,
                    ec_rasvad,
                    ec_susivesikud,
                    ec_kalorid
                },
                fotoSection
            }
        };


        sl = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Spacing = 15,
            IsVisible = false,
            Children = { btn_valifoto, btn_pildista, btn_salvesta, btn_vesi, btn_trener },
            Margin = new Thickness(0, 0, 0, 10)
        };

        var absolutelayout = new AbsoluteLayout();

        AbsoluteLayout.SetLayoutFlags(tableview, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(tableview, new Rect(0, 0, 1, 1));
        absolutelayout.Children.Add(tableview);

        AbsoluteLayout.SetLayoutFlags(sl, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(sl, new Rect(0.25, 0.95, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        absolutelayout.Children.Add(sl);

        AbsoluteLayout.SetLayoutFlags(btn_menu, AbsoluteLayoutFlags.PositionProportional);
        AbsoluteLayout.SetLayoutBounds(btn_menu, new Rect(0.95, 0.95, 60, 60));
        absolutelayout.Children.Add(btn_menu);

        Content = absolutelayout;
    }

    private async void Btn_trener_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new TreeningudFotoPage());
    }

    private async void Btn_vesi_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new VeejalgiminePage());
    }
    private void Btn_menu_Clicked(object sender, EventArgs e)
    {
        sl.IsVisible = !sl.IsVisible;
    }

    private void Btn_puhastada_Clicked(object sender, EventArgs e) => ClearForm();

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        FileResult foto = await MediaPicker.Default.PickPhotoAsync();
        await SalvestaFoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult foto = await MediaPicker.Default.CapturePhotoAsync();
            await SalvestaFoto(foto);
        }
        else
        {
            await Shell.Current.DisplayAlert("Viga", "Teie seade ei ole toetatud", "Ok");
        }
    }

    private async Task SalvestaFoto(FileResult foto)
    {
        if (foto != null)
        {
            lisafoto = Path.Combine(FileSystem.CacheDirectory, foto.FileName);

            using Stream sourceStream = await foto.OpenReadAsync();
            using MemoryStream ms = new MemoryStream();
            await sourceStream.CopyToAsync(ms);
            fotoBytes = ms.ToArray();

            File.WriteAllBytes(lisafoto, fotoBytes);

            img.Source = ImageSource.FromFile(lisafoto);

            fotoSection.Clear();
            var imageViewCell = new ViewCell
            {
                View = img
            };
            fotoSection.Add(imageViewCell);

            await Shell.Current.DisplayAlert("Edu", "Foto on edukalt salvestatud", "OK");
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ec_roaNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new LounasookClass();

        selectedItem.Roa_nimi = ec_roaNimi.Text;
        selectedItem.Valgud = int.TryParse(ec_valgud.Text, out var valgud) ? valgud : 0;
        selectedItem.Rasvad = int.TryParse(ec_rasvad.Text, out var rasvad) ? rasvad : 0;
        selectedItem.Susivesikud = int.TryParse(ec_susivesikud.Text, out var susivesikud) ? susivesikud : 0;
        selectedItem.Kalorid = int.TryParse(ec_kalorid.Text, out var kalorid) ? kalorid : 0;
        selectedItem.Kuupaev = dp_kuupaev.Date;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveLounasook(selectedItem);
        ClearForm();
    }

    private void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_roaNimi.Text = ec_valgud.Text = ec_rasvad.Text = ec_susivesikud.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        fotoSection.Clear();
        sl.IsVisible = false;
    }
}
