using Microsoft.Maui.Layouts;
using System.Globalization;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class TreeningudPage : ContentPage
{
    private string lisafoto;
    private byte[] fotoBytes;
    private TreeningudDatabase database;
    private TreeningudClass selectedItem;

    private EntryCell ec_treeninguNimi, ec_tuup, ec_kirjeldus, ec_link, ec_kalorid;
    private DatePicker dp_kuupaev;
    private TimePicker tp_kallaaeg;
    private Image img;
    private Switch redirectSwitch;

    private TableView tableview;
    private TableSection fotoSection;

    private ImageButton btn_salvesta, btn_pildista, btn_valifoto, btn_menu, btn_vesi, btn_trener;
    private StackLayout sl;

    public TreeningudPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new TreeningudDatabase(dbPath);

        Title = "Treeningud";

        ec_treeninguNimi = new EntryCell { Label = "Treeningu nimi", Placeholder = "nt. Jooksmine" };
        ec_tuup = new EntryCell { Label = "Tüüp", Placeholder = "nt. Kardio" };
        ec_kirjeldus = new EntryCell { Label = "Kirjeldus" };
        ec_link = new EntryCell { Label = "Video link" };
        ec_kalorid = new EntryCell { Label = "Kulutatud kalorid", Keyboard = Keyboard.Numeric };

        dp_kuupaev = new DatePicker { Date = DateTime.Now };
        tp_kallaaeg = new TimePicker { Time = TimeSpan.FromHours(8) };

        btn_pildista = new ImageButton
        {
            Source = "foto.png",
            BackgroundColor = Colors.LightGrey,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 10
        };
        btn_valifoto = new ImageButton
        {
            Source = "valifoto.png",
            BackgroundColor = Colors.LightSkyBlue,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 10
        };
        btn_salvesta = new ImageButton
        {
            Source = "salvesta.png",
            BackgroundColor = Colors.LightGreen,
            HeightRequest = 45,
            WidthRequest = 45,
            CornerRadius = 22 
        };

        btn_vesi = new ImageButton
        {
            Source = "vesi.png",
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

        redirectSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.End,
            ThumbColor = Colors.DarkViolet,
            OnColor = Colors.LightGreen
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
            Root = new TableRoot("Sisesta treening")
            {
                new TableSection("Üldandmed")
                {
                    new ViewCell { View = dp_kuupaev },
                    new ViewCell { View = tp_kallaaeg },
                    ec_treeninguNimi,
                    ec_tuup,
                    ec_kirjeldus,
                    ec_link,
                    ec_kalorid
                },
                fotoSection,
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

    private async void RedirectSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            await Navigation.PushAsync(new EnesetunnePage());
            Device.BeginInvokeOnMainThread(() => redirectSwitch.IsToggled = false);
        }
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
            await Application.Current.MainPage.DisplayAlert("Viga", "Teie seade ei ole toetatud", "Ok");
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
            fotoSection.Add(new ViewCell { View = img });

            await Application.Current.MainPage.DisplayAlert("Edu", "Foto on edukalt salvestatud", "OK");
        }
    }

    private void Btn_salvesta_Clicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ec_treeninguNimi.Text)) return;

        if (selectedItem == null)
            selectedItem = new TreeningudClass();

        selectedItem.Treeningu_nimi = ec_treeninguNimi.Text;
        selectedItem.Treeningu_tuup = ec_tuup.Text;
        selectedItem.Kirjeldus = ec_kirjeldus.Text;
        selectedItem.Link = ec_link.Text;
        selectedItem.Kulutud_kalorid = int.TryParse(ec_kalorid.Text, out var kalorid) ? kalorid : 0;
        selectedItem.Kallaaeg = tp_kallaaeg.Time;
        if (fotoBytes != null)
            selectedItem.Treeningu_foto = fotoBytes;

        database.SaveTreeningud(selectedItem);
        ClearForm();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteTreeningud(selectedItem.Treeningud_id);
            ClearForm();
        }
    }

    private void TreeningudListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        selectedItem = e.SelectedItem as TreeningudClass;
        if (selectedItem != null)
        {
            ec_treeninguNimi.Text = selectedItem.Treeningu_nimi;
            ec_tuup.Text = selectedItem.Treeningu_tuup;
            ec_kirjeldus.Text = selectedItem.Kirjeldus;
            ec_link.Text = selectedItem.Link;
            ec_kalorid.Text = selectedItem.Kulutud_kalorid.ToString();
            tp_kallaaeg.Time = selectedItem.Kallaaeg;

            if (selectedItem.Treeningu_foto != null && selectedItem.Treeningu_foto.Length > 0)
            {
                fotoSection.Clear();
                string imageFileName = $"img_{Guid.NewGuid()}.jpg";
                string imagePath = Path.Combine(FileSystem.AppDataDirectory, imageFileName);
                File.WriteAllBytes(imagePath, selectedItem.Treeningu_foto);

                var newImage = new Image
                {
                    Source = ImageSource.FromFile(imagePath),
                    HeightRequest = 60,
                    WidthRequest = 60,
                    Aspect = Aspect.AspectFill
                };

                var imageViewCell = new ViewCell { View = newImage };
                fotoSection.Add(imageViewCell);
            }
            else
            {
                fotoSection.Clear();
            }
        }
    }


    public void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_treeninguNimi.Text = ec_tuup.Text = ec_kirjeldus.Text = ec_link.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        fotoSection.Clear();
    }


    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}