using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tervisepaevik.Resources.Localization;
using Tervisepaevik.View;

namespace Tervisepaevik
{
    public partial class NewPage1 : ContentPage
    {
        public class MenuItemModel
        {
            public string Key { get; set; }
            public string Text { get; set; }
            public string Image { get; set; }
        }

        public List<MenuItemModel> valikud = new()
        {
            new MenuItemModel { Key = "welcome", Text = AppResources.Welcome, Image = "tervitus.png" },
            new MenuItemModel { Key = "food", Text = AppResources.Food, Image = "menuu.png" },
            new MenuItemModel { Key = "workout", Text = AppResources.Workouts, Image = "gym.png" },
            new MenuItemModel { Key = "feeling", Text = AppResources.HowIfeel, Image = "enesetunne.png" },
            new MenuItemModel { Key = "water", Text = AppResources.WaterMonitoring, Image = "veejalgimine.png" },
            new MenuItemModel { Key = "breathing", Text = AppResources.Breathing, Image = "kopsud.png" }
        };

        public NewPage1()
        {
            Title = AppResources.HealthDiary;

            ScrollView sv = new ScrollView();

            VerticalStackLayout vsl = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
            };

            for (int i = 0; i < valikud.Count; i++)
            {
                var frame = new Frame
                {
                    CornerRadius = 20,
                    HasShadow = false,
                    Padding = 10
                };

                var imgButton = new ImageButton
                {
                    Source = valikud[i].Image,
                    HeightRequest = 55,
                    WidthRequest = 55,
                    Aspect = Aspect.AspectFit,
                    BackgroundColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.Center
                };

                var label = new Label
                {
                    Text = valikud[i].Text,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };

                int index = i;

                imgButton.Clicked += async (s, e) =>
                {
                    var key = valikud[index].Key;

                    switch (key)
                    {
                        case "welcome":
                            await Navigation.PushAsync(new TervitatavPage());
                            break;

                        case "food":
                            Application.Current.MainPage = new Flyout_Page();
                            break;

                        case "workout":
                            await Navigation.PushAsync(new TreeningudFotoPage());
                            break;

                        case "feeling":
                            await Navigation.PushAsync(new EnesetunnePage());
                            break;

                        case "water":
                            await Navigation.PushAsync(new VeejalgiminePage());
                            break;

                        case "breathing":
                            await ShowBreathingPopup();
                            break;
                    }
                };

                frame.Content = new VerticalStackLayout
                {
                    Children = { imgButton, label }
                };

                vsl.Children.Add(frame);
            }

            sv.Content = vsl;
            Content = sv;
        }

        // 🔥 ИСПРАВЛЕННЫЙ POPUP
        private async Task ShowBreathingPopup()
        {
            var popupPage = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.7),
                Padding = 20
            };

            var timerLabel = new Label
            {
                Text = "30",
                FontSize = 28,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            var kopsudImage = new Image
            {
                Source = "kopsud.png",
                WidthRequest = 220,
                HeightRequest = 220,
                Aspect = Aspect.AspectFit
            };

            // 🌬 АНИМАЦИЯ ДЫХАНИЯ
            kopsudImage.Loaded += async (s, e) =>
            {
                while (true)
                {
                    await kopsudImage.ScaleTo(1.2, 3000, Easing.SinInOut);
                    await kopsudImage.ScaleTo(1.0, 3000, Easing.SinInOut);
                }
            };

            // ✅ КНОПКА SULGE (ИСПРАВЛЕНА)
            var btn_sule = new Button
            {
                Text = AppResources.Close, // или "Sulge"
                BackgroundColor = Color.FromArgb("#6C4CF1"),
                TextColor = Colors.White,
                CornerRadius = 15,
                WidthRequest = 160,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            btn_sule.Clicked += async (s, e) =>
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            };

            popupPage.Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 15,
                Children =
                {
                    new Label
                    {
                        Text = AppResources.BreathingExercise,
                        FontSize = 22,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = AppResources.BreathingInstruction,
                        FontSize = 14,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(10, 0)
                    },
                    timerLabel,
                    kopsudImage,
                    btn_sule
                }
            };

            int secondsRemaining = 30;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    secondsRemaining--;

                    timerLabel.Text = secondsRemaining > 0
                        ? secondsRemaining.ToString()
                        : AppResources.Ready;
                });

                return secondsRemaining > 0;
            });

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
        }
    }
}