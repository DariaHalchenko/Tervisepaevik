using Microsoft.Maui.Controls;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;
using MauiView = Microsoft.Maui.Controls.View;

namespace Tervisepaevik.View;

public partial class TreeningudPage : ContentPage
{
    private byte[] fotoBytes;
    private TreeningudDatabase database;
    private TreeningudClass selectedItem;

    private Entry entryNimi, entryTuup, entryKirjeldus, entryLink, entryKalorid;
    private DatePicker dp;
    private TimePicker tp;
    private Image img;

    public TreeningudPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new TreeningudDatabase(dbPath);

        Title = $"🏋️ {AppResources.Workouts}";

        dp = new DatePicker { Date = DateTime.Now };
        tp = new TimePicker { Time = TimeSpan.FromHours(8) };

        entryNimi = CreateEntry(AppResources.WorkoutName);
        entryTuup = CreateEntry(AppResources.Type);
        entryKirjeldus = CreateEntry(AppResources.Description);
        entryLink = CreateEntry(AppResources.VideoLink);
        entryKalorid = CreateEntry(AppResources.Calories);

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

        var btnPick = CreateButton(AppResources.ChoosePhoto, "#03A9F4", Btn_valifoto_Clicked);
        var btnPhoto = CreateButton(AppResources.Camera, "#2196F3", Btn_pildista_Clicked);
        var btnSave = CreateButton(AppResources.Save, "#4CAF50", Btn_salvesta_Clicked);

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

        mainStack.Children.Add(CreateCard($"📅 {AppResources.Time}", dp, tp));

        mainStack.Children.Add(CreateCard($"🏃 {AppResources.Workout1}",
            entryNimi,
            entryTuup,
            entryKirjeldus,
            entryLink,
            entryKalorid));

        mainStack.Children.Add(CreateCard($"📸 {AppResources.Photo}", fotoFrame, buttonRow));

        Content = new ScrollView { Content = mainStack };
    }

    private Entry CreateEntry(string placeholder)
    {
        return new Entry
        {
            Placeholder = placeholder,
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
            CornerRadius = 15,
            HeightRequest = 45,
            TextColor = Colors.White
        };

        btn.Clicked += action;
        return btn;
    }

    private Frame CreateCard(string title, params MauiView[] views)
    {
        var stack = new VerticalStackLayout
        {
            Spacing = 10
        };

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

    private async void Btn_valifoto_Clicked(object sender, EventArgs e)
    {
        var foto = await MediaPicker.Default.PickPhotoAsync();
        await LoadPhoto(foto);
    }

    private async void Btn_pildista_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var foto = await MediaPicker.Default.CapturePhotoAsync();
            await LoadPhoto(foto);
        }
    }

    private async Task LoadPhoto(FileResult foto)
    {
        if (foto == null) return;

        using var stream = await foto.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);

        fotoBytes = ms.ToArray();

        img.Source = ImageSource.FromStream(() => new MemoryStream(fotoBytes));
    }

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryNimi.Text))
        {
            await DisplayAlert(AppResources.Error, AppResources.EnterWorkoutName, AppResources.OK);
            return;
        }

        selectedItem ??= new TreeningudClass();

        selectedItem.Treeningu_nimi = entryNimi.Text;
        selectedItem.Treeningu_tuup = entryTuup.Text;
        selectedItem.Kirjeldus = entryKirjeldus.Text;
        selectedItem.Link = entryLink.Text;
        selectedItem.Kulutud_kalorid = int.TryParse(entryKalorid.Text, out var k) ? k : 0;
        selectedItem.Kuupaev = dp.Date;
        selectedItem.Kallaaeg = tp.Time;

        if (fotoBytes != null)
            selectedItem.Treeningu_foto = fotoBytes;

        database.SaveTreeningud(selectedItem);

        ClearForm();
    }

    private void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;

        entryNimi.Text = "";
        entryTuup.Text = "";
        entryKirjeldus.Text = "";
        entryLink.Text = "";
        entryKalorid.Text = "";

        dp.Date = DateTime.Now;
        tp.Time = TimeSpan.FromHours(8);
        img.Source = null;
    }
}