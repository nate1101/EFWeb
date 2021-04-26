using EventFully.Models;
using EventFully.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Repositories
{
    public class UserRepository : IUserRepository
    {
        private EventfullyDBContext _dbContext;

        public UserRepository(EventfullyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<int>> GetUserEventRoles(int eventId, string userId)
        {
            try
            {
                return await _dbContext.UserEventRole.Where(i => i.EventId == eventId).Where(i => i.UserId == userId).Select(i => i.RoleId).ToListAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
