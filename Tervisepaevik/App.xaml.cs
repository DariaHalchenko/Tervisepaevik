using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using Tervisepaevik.Services;

namespace Tervisepaevik
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TervitatavPage());

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                NotificationService.StartWaterReminders();
                NotificationService.ShowWaterReminder(15);

                NotificationService.StartWorkoutReminder();
            });
        }
    }
}