using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class ExhibitorRepository : IExhibitorRepository
    {
        private EventfullyDBContext _dbContext;

        public ExhibitorRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Exhibitor> GetExhibitorById(int id)
        {
            try
            {
                return await _dbContext.Exhibitor.FindAsync(id);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Exhibitor>> GetExhibitorsByEventId(int eventId)
        {
            try
            {
                return await (from ee in _dbContext.Exhibitor
                              //join e in _dbContext.Exhibitor on ee.ExhibitorId equals e.Id
                              where ee.EventId == eventId
                              select ee).Distinct().OrderBy(i => i.ExhibitorName).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Exhibitor> SaveExhibitor(Exhibitor exhibitor)
        {
            try
            {
                if (exhibitor.Id > 0)
                    _dbContext.Update(exhibitor);
                else
                {
                    await _dbContext.AddAsync(exhibitor);
                }

                await _dbContext.SaveChangesAsync();

                return exhibitor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteExhibitor(int exhibitorId)
        {
            try
            {
                var exhibitor = await GetExhibitorById(exhibitorId);
                _dbContext.Remove(exhibitor);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> GetExhibitorCountByEventId(int eventId)
        {
            try
            {
                return await _dbContext.Exhibitor.Where(i => i.EventId == eventId).CountAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
