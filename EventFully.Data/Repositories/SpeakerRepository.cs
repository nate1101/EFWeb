using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private EventfullyDBContext _dbContext;

        public SpeakerRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Speaker> GetSpeakerById(int speakerId)
        {
            try
            {
                return await _dbContext.Speaker.FindAsync(speakerId);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Speaker>> GetSpeakersByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Speaker.Where(i => i.EventId == eventId).OrderBy(i => i.FullName).OrderBy(i=>i.LastName).ThenBy(i=>i.FirstName).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetSpeakerCountByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Speaker.Where(i=>i.EventId == eventId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Speaker> SaveSpeaker(Speaker speaker)
        {
            try
            {
                if (speaker.Id > 0)
                    _dbContext.Update(speaker);
                else
                    await _dbContext.AddAsync(speaker);

                await _dbContext.SaveChangesAsync();

                return speaker;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AgendaItemSpeaker> AddAgendaItemToSpeaker(AgendaItemSpeaker item)
        {
            try
            {
                await _dbContext.AddAsync(item);
                await _dbContext.SaveChangesAsync();

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddAgendaItemSpeakers(List<AgendaItemSpeaker> items)
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

        public async Task<bool> RemoveAgendaItemSpeakers(int agendaItemId)
        {
            try
            {
                var items = await _dbContext.AgendaItemSpeaker.Where(i => i.AgendaItemId == agendaItemId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAgendaItemToSpeaker(int agendaItemId, int speakerId)
        {
            try
            {
                var items = await _dbContext.AgendaItemSpeaker.Where(i => i.AgendaItemId == agendaItemId).Where(i => i.SpeakerId == speakerId).ToListAsync();
                _dbContext.RemoveRange(items);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteSpeaker(int speakerId)
        {
            try
            {
                var speaker = await GetSpeakerById(speakerId);
                var items = await _dbContext.AgendaItemSpeaker.Where(i => i.SpeakerId == speakerId).ToListAsync();
                _dbContext.RemoveRange(items);
                _dbContext.Remove(speaker);
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
