using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<int>> GetUserEventRoles(int eventId, string userId);
    }
}
