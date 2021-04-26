using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(
            IUserRepository userRepository
            )
        {
            _userRepository = userRepository;
        }

        public async Task<List<int>> GetUserEventRoles(int eventId, string userId)
        {
            return await _userRepository.GetUserEventRoles(eventId, userId);
        }
    }
}
