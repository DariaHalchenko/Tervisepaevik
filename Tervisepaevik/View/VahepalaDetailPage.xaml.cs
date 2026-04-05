using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class VahepalaDetailPage : ContentPage
{
    private VahepalaClass item;
    private VahepalaDatabase database;

    private Image image;
    private Entry entryName, entryValgud, entryRasvad, entrySys, entryKalorid;

    public VahepalaDetailPage(VahepalaClass selected)
    {
        item = selected;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VahepalaDatabase(dbPath);

        Title = AppResources.Detail;

        image = new Image
        {
            Source = ImageSource.FromStream(() => new MemoryStream(item.Toidu_foto)),
            HeightRequest = 250,
            Aspect = Aspect.AspectFill
        };

        entryName = CreateEntry(item.Roa_nimi, AppResources.FoodName);
        entryValgud = CreateEntry(item.Valgud.ToString(), AppResources.Proteins);
        entryRasvad = CreateEntry(item.Rasvad.ToString(), AppResources.Fats);
        entrySys = CreateEntry(item.Susivesikud.ToString(), AppResources.Carbs);
        entryKalorid = CreateEntry(item.Kalorid.ToString(), AppResources.Calories);

        entryValgud.TextChanged += OnMacrosChanged;
        entryRasvad.TextChanged += OnMacrosChanged;
        entrySys.TextChanged += OnMacrosChanged;

        var saveBtn = new Button
        {
            Text = AppResources.Save,
            BackgroundColor = Colors.Green,
            TextColor = Colors.White
        };

        saveBtn.Clicked += Save_Clicked;

        var deleteBtn = new Button
        {
            Text = AppResources.Delete,
            BackgroundColor = Colors.Red,
            TextColor = Colors.White
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

    private Entry CreateEntry(string text, string placeholder)
    {
        return new Entry
        {
            Text = text,
            Placeholder = placeholder
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

    private async void Save_Clicked(object sender, EventArgs e)
    {
        item.Roa_nimi = entryName.Text;

        item.Valgud = int.TryParse(entryValgud.Text, out var v) ? v : 0;
        item.Rasvad = int.TryParse(entryRasvad.Text, out var r) ? r : 0;
        item.Susivesikud = int.TryParse(entrySys.Text, out var s) ? s : 0;

        item.Kalorid = (item.Valgud * 4) + (item.Susivesikud * 4) + (item.Rasvad * 9);

        database.SaveVahepala(item);

        await DisplayAlert(AppResources.OK, AppResources.Saved, AppResources.OK);
    }
}