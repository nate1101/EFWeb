using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class AgendaRepository : IAgendaRepository
    {
        private EventfullyDBContext _dbContext;

        public AgendaRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AgendaItem>> GetAgendaItems(int eventId)
        {
            try
            {
                return await _dbContext.AgendaItem.Where(i => i.EventId == eventId).OrderBy(i=>i.StartDate).ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AgendaItem> GetAgendaItem(int agendaItemId)
        {
            try
            {
                return await _dbContext.AgendaItem
                    //.Include(i=>i.Location)
                    .Include(i=>i.AgendaItemSpeakers)
                    .Where(i=>i.Id == agendaItemId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AgendaItem>> GetMyAgendaItems(int eventId, string userId)
        {
            try
            {
                return await _dbContext.AgendaItem.Where(i => i.EventId == eventId).Where(i=>i.UserAgendaItems.Any(x=>x.UserId == userId)).OrderBy(i => i.StartDate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddUserAgendaItem(UserAgendaItem item)
        {
            try
            {
                await _dbContext.AddAsync(item);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> RemoveUserAgendaItem(int agendaItemId, string userId)
        {
            try
            {
                var item = await _dbContext.UserAgendaItem.Where(i => i.AgendaItemId == agendaItemId).Where(i => i.UserId == userId).FirstOrDefaultAsync();
                _dbContext.Remove(item);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> RemoveUserAgendaItems(int agendaItemId)
        {
            try
            {
                var items = await _dbContext.UserAgendaItem.Where(i => i.AgendaItemId == agendaItemId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Track>> GetTracks(int eventId)
        {
            try
            {
                return await _dbContext.Track.Where(i => i.EventId == eventId).OrderBy(i => i.TrackName).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AgendaItem> SaveAgendaItem(AgendaItem item)
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

        public async Task<Track> SaveTrack(Track item)
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

        public async Task<bool> RemoveAgendaItem(int agendaItemId)
        {
            try
            {
                var items = await _dbContext.AgendaItem.Where(i => i.Id == agendaItemId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAgendaItemTracks(int agendaItemId)
        {
            try
            {
                var items = await _dbContext.TrackAgendaItem.Where(i => i.AgendaItemId == agendaItemId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddAgendaItemTracks(List<TrackAgendaItem> items)
        {
            try
            {
                await _dbContext.AddRangeAsync(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Track> GetTrackById(int trackId)
        {
            try
            {
                return await _dbContext.Track.FindAsync(trackId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveTrackFromAgendaItems(int trackId)
        {
            try
            {
                var items = await _dbContext.TrackAgendaItem.Where(i => i.TrackId == trackId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteTrack(int trackId)
        {
            try
            {
                var item = await _dbContext.Track.FindAsync(trackId);
                _dbContext.Remove(item);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetAgendaItemCountByEventId(int eventId)
        {
            try
            {
                return await _dbContext.AgendaItem.Where(i => i.EventId == eventId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetTrackCountByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Track.Where(i => i.EventId == eventId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SavePushReminder(PushReminder push)
        {
            try
            {
                if (push.Id == 0)
                    await _dbContext.AddAsync(push);
                else
                    _dbContext.Update(push);

                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
