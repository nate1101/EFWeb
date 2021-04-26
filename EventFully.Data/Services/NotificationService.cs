using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(
            INotificationRepository notificationRepository
            )
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<PushSubscription> SavePushSubscription(PushSubscription push)
        {
            return await _notificationRepository.SavePushSubscription(push);
        }

        public async Task<List<PushReminderView>> GetActivePushReminders()
        {
            return await _notificationRepository.GetActivePushReminders();
        }

        public async Task<bool> IsNotificationActive(string token)
        {
            return await _notificationRepository.IsNotificationActive(token);
        }

        public async Task<PushSubscription> GetPushSubscription(string token)
        {
            return await _notificationRepository.GetPushSubscription(token);
        }
    }
}
