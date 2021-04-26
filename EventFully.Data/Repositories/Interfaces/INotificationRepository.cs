using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<PushSubscription> SavePushSubscription(PushSubscription push);
        Task<List<PushReminderView>> GetActivePushReminders();
        Task<bool> IsNotificationActive(string token);
        Task<PushSubscription> GetPushSubscription(string token);
    }
}
