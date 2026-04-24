using Tervisepaevik.Database;
using Tervisepaevik.Models;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.View;

public partial class VahepalaDetailPage : ContentPage
{
    private VahepalaClass item;
    private VahepalaDatabase database;

    private Entry entryName;
    private Label kaloridLabel;

    private int valgud, rasvad, sys;

    public VahepalaDetailPage(VahepalaClass selected)
    {
        item = selected;

        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Tervisepaevik.db");
        database = new VahepalaDatabase(dbPath);

        Title = AppResources.Detail;

        valgud = item.Valgud;
        rasvad = item.Rasvad;
        sys = item.Susivesikud;

        var image = new Image
        {
            Source = ImageSource.FromStream(() => new MemoryStream(item.Toidu_foto)),
            HeightRequest = 250,
            Aspect = Aspect.AspectFill
        };

        entryName = new Entry
        {
            Text = item.Roa_nimi,
            Placeholder = AppResources.FoodName
        };

        var valgudLayout = CreateMacroRow(AppResources.Proteins, valgud, (v) =>
        {
            valgud = v;
            UpdateCalories();
        });

        var rasvadLayout = CreateMacroRow(AppResources.Fats, rasvad, (v) =>
        {
            rasvad = v;
            UpdateCalories();
        });

        var sysLayout = CreateMacroRow(AppResources.Carbs, sys, (v) =>
        {
            sys = v;
            UpdateCalories();
        });

        var infoLabel = new Label
        {
            Text = AppResources.SnackInfo,
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

        UpdateCalories();

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

                    valgudLayout,
                    rasvadLayout,
                    sysLayout,

                    infoLabel,
                    caloriesRow,

                    saveBtn,
                    deleteBtn
                }
            }
        };
    }

    private HorizontalStackLayout CreateMacroRow(string title, int initialValue, Action<int> onChanged)
    {
        int value = initialValue;

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
            Text = $"{value} g",
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
            (kalorid >= 200 && kalorid <= 400)
            ? Colors.Green
            : Colors.Red;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        item.Roa_nimi = entryName.Text;

        item.Valgud = valgud;
        item.Rasvad = rasvad;
        item.Susivesikud = sys;
        item.Kalorid = (valgud * 4) + (sys * 4) + (rasvad * 9);

        database.SaveVahepala(item);

        await DisplayAlert(AppResources.OK, AppResources.Saved, AppResources.OK);
    }
}