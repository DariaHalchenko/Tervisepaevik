using Microsoft.Maui.Controls;
using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

using MauiView = Microsoft.Maui.Controls.View;

namespace Tervisepaevik.View;

public partial class OhtusookPage : ContentPage
{
    private byte[] fotoBytes;
    private OhtusookDatabase database;
    private OhtusookClass selectedItem;

    private Entry entryRoa;
    private Label kaloridLabel;

    private DatePicker dp;
    private TimePicker tp;
    private Image img;

    private int valgud = 0, rasvad = 0, sys = 0;

    public OhtusookPage()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new OhtusookDatabase(dbPath);

        Title = AppResources.Dinner;

        dp = new DatePicker { Date = DateTime.Now };
        tp = new TimePicker { Time = TimeSpan.FromHours(19) };

        entryRoa = CreateEntry(AppResources.FoodName);

        var valgudLayout = CreateMacroRow(AppResources.Proteins, (v) =>
        {
            valgud = v;
            UpdateCalories();
        });

        var rasvadLayout = CreateMacroRow(AppResources.Fats, (v) =>
        {
            rasvad = v;
            UpdateCalories();
        });

        var sysLayout = CreateMacroRow(AppResources.Carbs, (v) =>
        {
            sys = v;
            UpdateCalories();
        });

        var infoLabel = new Label
        {
            Text = AppResources.DinnerInfo,
            FontSize = 12,
            TextColor = Colors.Gray
        };

        kaloridLabel = new Label
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };

        var caloriesRow = new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label
                {
                    Text = AppResources.Calories,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center
                },
                kaloridLabel
            }
        };

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
            dp, tp, entryRoa,
            valgudLayout, rasvadLayout, sysLayout,
            infoLabel,
            caloriesRow
        ));

        mainStack.Children.Add(CreateCard(AppResources.Photo,
            fotoFrame, buttonRow));

        Content = new ScrollView { Content = mainStack };

        UpdateCalories();
    }

    private HorizontalStackLayout CreateMacroRow(string title, Action<int> onChanged)
    {
        int value = 0;

        var minusBtn = new Button
        {
            Text = "-",
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            BackgroundColor = Colors.IndianRed,
            TextColor = Colors.White
        };

        var plusBtn = new Button
        {
            Text = "+",
            WidthRequest = 44,
            HeightRequest = 44,
            CornerRadius = 22,
            BackgroundColor = Colors.SeaGreen,
            TextColor = Colors.White
        };

        var valueLabel = new Label
        {
            Text = "0 g",
            VerticalOptions = LayoutOptions.Center
        };

        minusBtn.Clicked += (s, e) =>
        {
            value = Math.Max(0, value - 1);
            valueLabel.Text = $"{value} g";
            onChanged(value);
        };

        plusBtn.Clicked += (s, e) =>
        {
            value++;
            valueLabel.Text = $"{value} g";
            onChanged(value);
        };

        return new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                new Label
                {
                    Text = title,
                    WidthRequest = 120,
                    VerticalOptions = LayoutOptions.Center
                },
                minusBtn,
                valueLabel,
                plusBtn
            }
        };
    }

    private void UpdateCalories()
    {
        int kalorid = (valgud * 4) + (sys * 4) + (rasvad * 9);
        kaloridLabel.Text = $"{kalorid} kcal";

        kaloridLabel.TextColor =
            (kalorid >= 400 && kalorid <= 700)
            ? Colors.Green
            : Colors.Red;
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
            stack.Children.Add(v);

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

    private async void Btn_salvesta_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryRoa.Text))
        {
            await DisplayAlert(AppResources.Error, AppResources.EnterFoodName, AppResources.OK);
            return;
        }

        selectedItem ??= new OhtusookClass();

        selectedItem.Roa_nimi = entryRoa.Text;
        selectedItem.Valgud = valgud;
        selectedItem.Rasvad = rasvad;
        selectedItem.Susivesikud = sys;
        selectedItem.Kalorid = (valgud * 4) + (sys * 4) + (rasvad * 9);
        selectedItem.Kuupaev = dp.Date;
        selectedItem.Kallaaeg = tp.Time;

        if (fotoBytes != null)
            selectedItem.Toidu_foto = fotoBytes;

        database.SaveOhtusook(selectedItem);

        ClearForm();
    }

    private void ClearForm()
    {
        selectedItem = null;
        fotoBytes = null;

        entryRoa.Text = "";

        valgud = rasvad = sys = 0;

        dp.Date = DateTime.Now;
        tp.Time = TimeSpan.FromHours(19);
        img.Source = null;

        UpdateCalories();
    }
}