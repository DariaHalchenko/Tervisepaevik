using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisepaevik.Resources.Localization;

namespace Tervisepaevik.Services;

public static class NotificationService
{
    public static async Task RequestPermission()
    {
        await LocalNotificationCenter.Current.RequestNotificationPermission();
    }
    public static void ShowWaterReminder(int seconds = 5)
    {
        var request = new NotificationRequest
        {
            NotificationId = 100,
            Title = AppResources.WaterTitle,
            Description = AppResources.WaterDescription,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(seconds)
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }

    public static void StartWaterReminders()
    {
        for (int i = 1; i <= 10; i++) 
        {
            var request = new NotificationRequest
            {
                NotificationId = 100 + i,
                Title = AppResources.WaterTitle,
                Description = AppResources.WaterDescription,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMinutes(30 * i)
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}