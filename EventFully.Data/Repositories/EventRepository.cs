using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EventFully.Constant;

namespace EventFully.Repositories
{
    public class EventRepository: IEventRepository
    {
        private EventfullyDBContext _dbContext;

        public EventRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Event>> GetCurrentEvents()
        {
            try
            {
                return await _dbContext.Event.Where(i => i.StartDate >= DateTime.Today.AddDays(-365)).Where(i=>i.Published).OrderBy(i=>i.StartDate).ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Event>> GetUserEvents(string userId)
        {
            try
            {
                return await _dbContext.Event.Where(i => i.UserEventRoles.Any(x=>x.UserId == userId)).Distinct().OrderByDescending(i=>i.StartDate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Event> GetEventById(int eventId)
        {
            try
            {
                return await _dbContext.Event.FindAsync(eventId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Event>> GetEventsByRole()
        {
            try
            {
                return await _dbContext.Event.Where(i => i.StartDate >= DateTime.Today).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<State>> GetStates()
        {
            return await _dbContext.State.ToListAsync();
        }

        public async Task<Event> SaveEvent(Event item)
        {
            try
            {
                if (item.Id > 0)
                    _dbContext.Update(item);
                else
                    await _dbContext.AddAsync(item);

                await _dbContext.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> SaveOrder(Order item)
        {
            try
            {
                if (item.Id > 0)
                    _dbContext.Update(item);
                else
                    await _dbContext.AddAsync(item);

                await _dbContext.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<OrderItem>> SaveOrderItems(List<OrderItem> items)
        {
            try
            {
                await _dbContext.AddRangeAsync(items);

                await _dbContext.SaveChangesAsync();

                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserEventRole> SaveUserEventRole(UserEventRole role)
        {
            try
            {
                if (role.Id > 0)
                    _dbContext.Update(role);
                else
                    await _dbContext.AddAsync(role);

                await _dbContext.SaveChangesAsync();

                return role;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Sponsor>> GetSponsorsByEvent(int id)
        {
            try
            {
                return await _dbContext.Sponsor.Where(i=>i.EventId == id).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Sponsor> GetSponsorById(int id)
        {
            try
            {
                return await _dbContext.Sponsor.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Sponsor> SaveSponsor(Sponsor sponsor)
        {
            try
            {
                if (sponsor.Id > 0)
                    _dbContext.Update(sponsor);
                else
                    await _dbContext.AddAsync(sponsor);

                await _dbContext.SaveChangesAsync();

                return sponsor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteSponsor(int sponsorId)
        {
            try
            {
                var sponsor = await GetSponsorById(sponsorId);
                _dbContext.Remove(sponsor);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetSponsorCountByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Sponsor.Where(i => i.EventId == eventId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            try
            {
                return await _dbContext.Order.FindAsync(orderId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> GetOrderByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Order.Where(i=>i.EventId == eventId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AreThereAdminsRemaining(string userId, int eventId)
        {
            try
            {
                var admins = await _dbContext.UserEventRole.Where(i => i.EventId == eventId).Where(i => i.RoleId == SecurityRole.Administrator).Where(i => i.UserId != userId).ToListAsync();
                if (admins.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserEventRole> GetUserEventRoleById(int id)
        {
            try
            {
                return await _dbContext.UserEventRole.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteUserEventRole(UserEventRole role)
        {
            try
            {
                _dbContext.Remove(role);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserEventInvitation> GetUserEventInvitation(string email, int eventId)
        {
            try
            {
                return await _dbContext.UserEventInvitation.Where(i => i.EventId == eventId).Where(i => i.EmailAddress == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserEventInvitation> SaveUserEventInvitation(UserEventInvitation invitation)
        {
            try
            {
                if (invitation.Id > 0)
                    _dbContext.Update(invitation);
                else
                    await _dbContext.AddAsync(invitation);

                await _dbContext.SaveChangesAsync();

                return invitation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserEventInvitation> GetUserEventInvitationByToken(string token)
        {
            try
            {
                return await _dbContext.UserEventInvitation.Where(i => i.Token == token).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
