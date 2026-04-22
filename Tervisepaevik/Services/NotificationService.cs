using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.LocalNotification;

namespace Tervisepaevik.Services;

public static class NotificationService
{
    public static void ShowWaterReminder(int seconds = 5)
    {
        var request = new NotificationRequest
        {
            NotificationId = 100,
            Title = "💧 Вода",
            Description = "Попей воды!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(seconds)
            }
        };

        LocalNotificationCenter.Current.Show(request);
    }

    public static void StartWaterReminders()
    {
        for (int i = 1; i <= 10; i++) // 10 напоминаний
        {
            var request = new NotificationRequest
            {
                NotificationId = 100 + i,
                Title = "💧 Вода",
                Description = "Попей воды!",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddMinutes(30 * i)
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}