using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private EventfullyDBContext _dbContext;

        public NotificationRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PushSubscription> SavePushSubscription(PushSubscription push)
        {
            try
            {
                if (push.Id == 0)
                    await _dbContext.AddAsync(push);
                else
                    _dbContext.Update(push);

                await _dbContext.SaveChangesAsync();

                return push;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PushReminderView>> GetActivePushReminders()
        {
            try
            {
                return await _dbContext.PushReminderView.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> IsNotificationActive(string token)
        {
            try
            {
                var result = await _dbContext.PushSubscription.Where(i => i.Token == token).Where(i=>i.ActiveFlag == true).FirstOrDefaultAsync();
                if (result != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PushSubscription> GetPushSubscription(string token)
        {
            try
            {
                return await _dbContext.PushSubscription.Where(i => i.Token == token).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
