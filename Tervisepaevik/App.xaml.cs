using Tervisepaevik.Services;
using Microsoft.Maui.Storage;

namespace Tervisepaevik
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TervitatavPage());

            if (!Preferences.Get("notifications_started", false))
            {
                NotificationService.StartWaterReminders();
                Preferences.Set("notifications_started", true);
            }
        }
    }
}