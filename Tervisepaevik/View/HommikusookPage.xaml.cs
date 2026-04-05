using Microsoft.Maui.Controls;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class HommikusookPage : ContentPage
{
    private byte[] fotoBytes;
    private HommikusookDatabase database;
    private HommikusookClass selectedItem;

    private Entry entryRoa, entryValgud, entryRasvad, entrySys, entryKalorid;
    private DatePicker dp;
    private TimePicker tp;
    private Image img;

    public HommikusookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new HommikusookDatabase(dbPath);

        Title = "Hommikusöök";
        BackgroundColor = Color.FromArgb("#F5F5F5");

        dp = new DatePicker { Date = DateTime.Now };
        tp = new TimePicker { Time = TimeSpan.FromHours(8) };

        entryRoa = CreateEntry("Roa nimi");
        entryValgud = CreateEntry("Valgud (g)");
        entryRasvad = CreateEntry("Rasvad (g)");
        entrySys = CreateEntry("Süsivesikud (g)");
        entryKalorid = CreateEntry("Kalorid (kcal)");

        // 🔥 КАЛЬКУЛЯТОР — подключаем события
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

        var btnPick = CreateButton("Vali", "#03A9F4", Btn_valifoto_Clicked);
        var btnPhoto = CreateButton("Kaamera", "#2196F3", Btn_pildista_Clicked);
        var btnSave = CreateButton("Salvesta", "#4CAF50", Btn_salvesta_Clicked);

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

        mainStack.Children.Add(CreateCard("Üldandmed",
            dp, tp, entryRoa, entryValgud, entryRasvad, entrySys, entryKalorid));

        mainStack.Children.Add(CreateCard("Foto",
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
            BackgroundColor = Colors.White,
            HeightRequest = 50,
            Margin = new Thickness(0, 5)
        };
    }

    private Button CreateButton(string text, string color, EventHandler action)
    {
        var btn = new Button
        {
            Text = text,
            BackgroundColor = Color.FromArgb(color),
            TextColor = Colors.White,
            CornerRadius = 15,
            HeightRequest = 45
        };

        btn.Clicked += action;
        return btn;
    }

    private Frame CreateCard(string title, params object[] views)
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
            if (v is VisualElement element)
                stack.Children.Add(element);
        }

        return new Frame
        {
            CornerRadius = 20,
            Padding = 15,
            BackgroundColor = Colors.White,
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

    private void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryRoa.Text)) return;

        selectedItem ??= new HommikusookClass();

        selectedItem.Roa_nimi = entryRoa.Text;
        selectedItem.Valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        selectedItem.Rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        selectedItem.Susivesikud = int.TryParse(entrySys.Text, out var s) ? s : 0;
        selectedItem.Kalorid = int.TryParse(entryKalorid.Text, out var k) ? k : 0;
        selectedItem.Kuupaev = dp.Date;
        selectedItem.Kallaaeg = tp.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveHommikusook(selectedItem);

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
        tp.Time = TimeSpan.FromHours(8);
        img.Source = null;
    }
} 