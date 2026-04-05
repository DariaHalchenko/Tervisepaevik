using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class OhtusookDetailPage : ContentPage
{
    private OhtusookClass item;
    private OhtusookDatabase database;

    private Image image;
    private Entry entryName, entryValgud, entryRasvad, entrySys, entryKalorid;

    public OhtusookDetailPage(OhtusookClass selected)
    {
        item = selected;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        Title = "Detail";

        image = new Image
        {
            Source = ImageSource.FromStream(() => new MemoryStream(item.Toidu_foto)),
            HeightRequest = 250,
            Aspect = Aspect.AspectFill
        };

        entryName = CreateEntry(item.Roa_nimi);
        entryValgud = CreateEntry(item.Valgud.ToString());
        entryRasvad = CreateEntry(item.Rasvad.ToString());
        entrySys = CreateEntry(item.Susivesikud.ToString());
        entryKalorid = CreateEntry(item.Kalorid.ToString());

        // 🔥 калькулятор
        entryValgud.TextChanged += OnMacrosChanged;
        entryRasvad.TextChanged += OnMacrosChanged;
        entrySys.TextChanged += OnMacrosChanged;

        var saveBtn = new Button
        {
            Text = "Salvesta",
            BackgroundColor = Colors.Green,
            TextColor = Colors.White
        };

        saveBtn.Clicked += Save_Clicked;

        var deleteBtn = new Button
        {
            Text = "Kustuta",
            BackgroundColor = Colors.Red,
            TextColor = Colors.White
        };

        deleteBtn.Clicked += async (s, e) =>
        {
            bool confirm = await DisplayAlert("Kustuta", "Kas oled kindel?", "Jah", "Ei");
            if (confirm)
            {
                database.DeleteOhtusook(item.Ohtusook_id);
                await Navigation.PopAsync();
            }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    image,
                    entryName,
                    entryValgud,
                    entryRasvad,
                    entrySys,
                    entryKalorid,
                    saveBtn,
                    deleteBtn
                }
            }
        };
    }

    private Entry CreateEntry(string text)
    {
        return new Entry
        {
            Text = text
        };
    }

    private void OnMacrosChanged(object sender, TextChangedEventArgs e)
    {
        int valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        int rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        int sys = int.TryParse(entrySys.Text, out var s) ? s : 0;

        int kalorid = (valgud * 4) + (sys * 4) + (rasvad * 9);

        entryKalorid.Text = kalorid.ToString();
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
        item.Roa_nimi = entryName.Text;

        item.Valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        item.Rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        item.Susivesikud = int.TryParse(entrySys.Text, out var s) ? s : 0;

        item.Kalorid = (item.Valgud * 4) + (item.Susivesikud * 4) + (item.Rasvad * 9);

        database.SaveOhtusook(item);

        DisplayAlert("OK", "Salvestatud", "OK");
    }
}