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
    private ListView treeningudListView;

    private Button btn_salvesta, btn_kustuta, btn_puhastada, btn_pildista, btn_valifoto, btn_hide, btn_usertreeningud;

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

        btn_salvesta = new Button { Text = "Salvesta" };
        btn_kustuta = new Button { Text = "Kustuta", IsVisible = false };
        btn_puhastada = new Button { Text = "Uus sisestus" };
        btn_pildista = new Button { Text = "Tee foto" };
        btn_valifoto = new Button { Text = "Vali foto" };
        btn_hide = new Button { Text = "Näita loendit" };
        btn_usertreeningud = new Button { Text = "Näita carousel" };

        redirectSwitch = new Switch
        {
            HorizontalOptions = LayoutOptions.End,
            ThumbColor = Colors.DarkViolet,
            OnColor = Colors.LightGreen
        };

        btn_salvesta.Clicked += Btn_salvesta_Clicked;
        btn_kustuta.Clicked += Btn_kustuta_Clicked;
        btn_puhastada.Clicked += Btn_puhastada_Clicked;
        btn_pildista.Clicked += Btn_pildista_Clicked;
        btn_valifoto.Clicked += Btn_valifoto_Clicked;
        btn_hide.Clicked += Btn_hide_Clicked;
        btn_usertreeningud.Clicked += Btn_usertreeningud_Clicked;
        redirectSwitch.Toggled += RedirectSwitch_Toggled;

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
                new TableSection("Tegevused")
                {
                    new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 10,
                            Children =
                            {
                                new Label
                                {
                                    Text = "Minu enesetunne",
                                    VerticalOptions = LayoutOptions.Center,
                                    FontSize = 14
                                },
                                redirectSwitch,
                                btn_salvesta,
                                btn_kustuta,
                                btn_puhastada,
                                btn_usertreeningud
                            }
                        }
                    }
                },
                new TableSection("FOTO")
                {
                    new ViewCell
                    {
                        View = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Children = { btn_valifoto, btn_pildista }
                        }
                    }
                }
            }
        };

        treeningudListView = new ListView
        {
            SeparatorColor = Colors.DarkViolet,
            BackgroundColor = Colors.WhiteSmoke,
            Header = "Treeningud",
            HasUnevenRows = true,
            ItemTemplate = new DataTemplate(() =>
            {
                Image img = new Image { WidthRequest = 60, HeightRequest = 60 };
                img.SetBinding(Image.SourceProperty, new Binding("Treeningu_foto", converter: new ByteArrayToImageSourceConverter()));

                Label nimi = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
                nimi.SetBinding(Label.TextProperty, "Treeningu_nimi");

                Label kirjeldus = new Label { FontSize = 14 };
                kirjeldus.SetBinding(Label.TextProperty, new Binding("Kirjeldus", stringFormat: "{0}"));

                Label kuupaev = new Label { FontSize = 14 };
                kuupaev.SetBinding(Label.TextProperty, new Binding("Kuupaev", stringFormat: "Kuupäev: {0:d}"));

                return new ViewCell
                {
                    View = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Padding = new Thickness(10),
                        Children =
                        {
                            img,
                            new StackLayout
                            {
                                Orientation = StackOrientation.Vertical,
                                Padding = new Thickness(10, 0),
                                Children = { nimi, kirjeldus, kuupaev }
                            }
                        }
                    }
                };
            })
        };

        treeningudListView.ItemSelected += TreeningudListView_ItemSelected;

        Content = new ScrollView
        {
            Content = new StackLayout
            {
                Padding = 10,
                Children =
                {
                    btn_hide,
                    tableview,
                    new Label { Text = "Salvestatud treeningud", FontAttributes = FontAttributes.Bold },
                    treeningudListView
                }
            }
        };

        AndmeteLaadimine();
    }

    private async void RedirectSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            await Navigation.PushAsync(new EnesetunnePage());
            Device.BeginInvokeOnMainThread(() => redirectSwitch.IsToggled = false);
        }
    }

    private async void Btn_usertreeningud_Clicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new TreeningudFotoPage());
    }

    private void Btn_puhastada_Clicked(object sender, EventArgs e) => SelgeVorm();

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
            var imageViewCell = new ViewCell { View = img };
            fotoSection.Add(imageViewCell);

            await Shell.Current.DisplayAlert("Edu", "Foto on edukalt salvestatud", "OK");
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
        SelgeVorm();
        AndmeteLaadimine();
    }

    private void Btn_kustuta_Clicked(object? sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            database.DeleteTreeningud(selectedItem.Treeningud_id);
            SelgeVorm();
            AndmeteLaadimine();
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
            btn_kustuta.IsVisible = true;

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

    public void AndmeteLaadimine()
    {
        treeningudListView.ItemsSource = database.GetTreeningud().OrderByDescending(x => x.Kallaaeg).ToList();
    }

    public void SelgeVorm()
    {
        selectedItem = null;
        fotoBytes = null;
        ec_treeninguNimi.Text = ec_tuup.Text = ec_kirjeldus.Text = ec_link.Text = ec_kalorid.Text = string.Empty;
        dp_kuupaev.Date = DateTime.Now;
        tp_kallaaeg.Time = TimeSpan.FromHours(8);
        treeningudListView.SelectedItem = null;
        btn_kustuta.IsVisible = false;
        fotoSection.Clear();
    }

    private void Btn_hide_Clicked(object sender, EventArgs e)
    {
        if (tableview.IsVisible)
        {
            tableview.IsVisible = false;
            treeningudListView.IsVisible = true;
        }
        else
        {
            tableview.IsVisible = true;
            treeningudListView.IsVisible = false;
        }
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