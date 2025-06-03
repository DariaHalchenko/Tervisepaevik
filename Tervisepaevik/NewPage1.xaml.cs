using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Tervisepaevik.View;

namespace Tervisepaevik
{
    public partial class NewPage1 : ContentPage
    {
        public List<ContentPage> lehed = new List<ContentPage>()
        {
            new TervitatavPage(),
            new TreeningudPage(),
            new EnesetunnePage(),
            new VeejalgiminePage()
        };

        public List<(string Tekst, string Pilt)> valikud = new List<(string, string)>
        {
            ("Tervitus", "tervitus.png"),
            ("Söök", "menuu.png"),
            ("Treeningud", "gym.png"),
            ("Enesetunne", "enesetunne.png"),
            ("Vee jälgimine", "veejalgimine.png"),
            ("Hingamine", "kopsud.png") 
        };

        public NewPage1()
        {
            Title = "Tervise Päevik";

            ScrollView sv = new ScrollView();
            VerticalStackLayout vsl = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
                BackgroundColor = Color.FromArgb("#FFF0F5")
            };

            for (int i = 0; i < valikud.Count; i++)
            {
                var frame = new Frame
                {
                    BorderColor = Colors.LightGray,
                    CornerRadius = 20,
                    HasShadow = true,
                    BackgroundColor = Colors.White,
                    Padding = 10
                };

                var imgButton = new ImageButton
                {
                    Source = valikud[i].Pilt,
                    HeightRequest = 55, 
                    WidthRequest = 55,  
                    Aspect = Aspect.AspectFit,
                    BackgroundColor = Colors.Transparent,
                    HorizontalOptions = LayoutOptions.Center,
                    CornerRadius = 10 
                };

                var label = new Label
                {
                    Text = valikud[i].Tekst,
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.DarkMagenta,
                    HorizontalOptions = LayoutOptions.Center
                };

                int index = i; 
                imgButton.Clicked += async (s, e) =>
                {
                    if (valikud[index].Tekst == "Menüü")
                    {
                        Application.Current.MainPage = new Flyout_Page();
                    }
                    else if (valikud[index].Tekst == "Hingamine")
                    {
                        await ShowBreathingPopup();
                    }
                    else
                    {
                        int realIndex = index > 1 ? index - 1 : index;
                        await Navigation.PushAsync(lehed[realIndex]);
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
                FontSize = 24,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            var kopsudImage = new Image
            {
                Source = "kopsud.png",
                WidthRequest = 200,
                HeightRequest = 200,
                Aspect = Aspect.AspectFit
            };

            kopsudImage.Loaded += async (s, e) =>
            {
                while (true)
                {
                    await kopsudImage.ScaleTo(1.2, 3000, Easing.SinInOut);
                    await kopsudImage.ScaleTo(1.0, 3000, Easing.SinInOut);
                }
            };

            var btn_sule = new Button
            {
                Text = "Sulge",
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
                Children =
                {
                    new Label
                    {
                        Text = "Hingamise harjutus",
                        FontSize = 20,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Hingake sisse, kui pilt muutub suuremaks,\nhingake välja, kui pilt muutub väiksemaks.",
                        FontSize = 14,
                        TextColor = Colors.White,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Margin = new Thickness(10, 10, 10, 0)
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
                    timerLabel.Text = secondsRemaining > 0 ? secondsRemaining.ToString() : "Valmis!";
                });
                return secondsRemaining > 0;
            });

            await Application.Current.MainPage.Navigation.PushModalAsync(popupPage);
        }
    }
}
