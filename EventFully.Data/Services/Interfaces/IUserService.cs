using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<int>> GetUserEventRoles(int eventId, string userId);
    }
}
