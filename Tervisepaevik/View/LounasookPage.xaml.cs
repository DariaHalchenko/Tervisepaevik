using Microsoft.Maui.Controls;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

// alias чтобы не было проблем с View
using MauiView = Microsoft.Maui.Controls.View;

namespace Tervisepaevik.View;

public partial class LounasookPage : ContentPage
{
    private byte[] fotoBytes;
    private LounasookDatabase database;
    private LounasookClass selectedItem;

    private Entry entryRoa, entryValgud, entryRasvad, entrySys, entryKalorid;
    private DatePicker dp;
    private TimePicker tp;
    private Image img;

    public LounasookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new LounasookDatabase(dbPath);

        Title = AppResources.Lunch;

        dp = new DatePicker { Date = DateTime.Now };
        tp = new TimePicker { Time = TimeSpan.FromHours(13) };

        entryRoa = CreateEntry(AppResources.FoodName);
        entryValgud = CreateEntry(AppResources.Proteins);
        entryRasvad = CreateEntry(AppResources.Fats);
        entrySys = CreateEntry(AppResources.Carbs);
        entryKalorid = CreateEntry(AppResources.Calories);

        entryValgud.TextChanged += OnMacrosChanged;
        entryRasvad.TextChanged += OnMacrosChanged;
        entrySys.TextChanged += OnMacrosChanged;

        img = new Image
        {
            HeightRequest = 200,
            Aspect = Aspect.AspectFill
        };

        var fotoFrame = new Frame
        {
            CornerRadius = 15,
            Padding = 0,
            IsClippedToBounds = true,
            Content = img
        };

        var btnPick = CreateButton(AppResources.Choose, Colors.Blue, Btn_valifoto_Clicked);
        var btnPhoto = CreateButton(AppResources.Camera, Colors.Blue, Btn_pildista_Clicked);
        var btnSave = CreateButton(AppResources.Save, Colors.Green, Btn_salvesta_Clicked);

        var buttonRow = new HorizontalStackLayout
        {
            Spacing = 10,
            Children = { btnPick, btnPhoto, btnSave }
        };

        var mainStack = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 20
        };

        mainStack.Children.Add(CreateCard(AppResources.GeneralInfo,
            dp, tp, entryRoa, entryValgud, entryRasvad, entrySys, entryKalorid));

        mainStack.Children.Add(CreateCard(AppResources.Photo,
            fotoFrame, buttonRow));

        Content = new ScrollView { Content = mainStack };
    }

    // ================= CALCULATOR =================
    private void OnMacrosChanged(object sender, TextChangedEventArgs e)
    {
        int valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        int rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        int sys = int.TryParse(entrySys.Text, out var s) ? s : 0;

        int kalorid = (valgud * 4) + (sys * 4) + (rasvad * 9);

        entryKalorid.Text = kalorid.ToString();
    }

    // ================= UI =================

    private Entry CreateEntry(string placeholder)
    {
        return new Entry
        {
            Placeholder = placeholder,
            HeightRequest = 50,
            Margin = new Thickness(0, 5)
        };
    }

    private Button CreateButton(string text, Color color, EventHandler clicked)
    {
        var btn = new Button
        {
            Text = text,
            BackgroundColor = color,
            TextColor = Colors.White,
            CornerRadius = 10
        };

        btn.Clicked += clicked;
        return btn;
    }

    // исправлено (без object)
    private Frame CreateCard(string title, params MauiView[] views)
    {
        var stack = new VerticalStackLayout { Spacing = 10 };

        stack.Children.Add(new Label
        {
            Text = title,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        });

        foreach (var v in views)
        {
            stack.Children.Add(v);
        }

        return new Frame
        {
            CornerRadius = 20,
            Padding = 15,
            HasShadow = true,
            Content = stack
        };
    }

    // ================= FOTO =================

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.Default.PickPhotoAsync();
        await SalvestaFoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var foto = await MediaPicker.Default.CapturePhotoAsync();
            await SalvestaFoto(foto);
        }
    }

    private async Task SalvestaFoto(FileResult foto)
    {
        if (foto == null) return;

        using var stream = await foto.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        fotoBytes = ms.ToArray();

        img.Source = ImageSource.FromStream(() => new MemoryStream(fotoBytes));
    }

    // ================= SAVE =================

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryRoa.Text))
        {
            await DisplayAlert(AppResources.Error, AppResources.EnterFoodName, AppResources.OK);
            return;
        }

        selectedItem ??= new LounasookClass();

        selectedItem.Roa_nimi = entryRoa.Text;
        selectedItem.Valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        selectedItem.Rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        selectedItem.Susivesikud = int.TryParse(entrySys.Text, out var s) ? s : 0;
        selectedItem.Kalorid = int.TryParse(entryKalorid.Text, out var k) ? k : 0;
        selectedItem.Kuupaev = dp.Date;
        selectedItem.Kallaaeg = tp.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveLounasook(selectedItem);

        ClearForm();
    }

    private void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;

        entryRoa.Text = "";
        entryValgud.Text = "";
        entryRasvad.Text = "";
        entrySys.Text = "";
        entryKalorid.Text = "";

        dp.Date = DateTime.Now;
        tp.Time = TimeSpan.FromHours(13);
        img.Source = null;
    }
}