using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface INotificationService
    {
        Task<PushSubscription> SavePushSubscription(PushSubscription push);
        Task<List<PushReminderView>> GetActivePushReminders();
        Task<bool> IsNotificationActive(string token);
        Task<PushSubscription> GetPushSubscription(string token);
    }
}
