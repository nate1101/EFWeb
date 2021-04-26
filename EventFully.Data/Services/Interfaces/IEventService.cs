using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface IEventService
    {
        Task<List<Event>> GetCurrentEvents();
        Task<Event> GetEventById(int eventId);
        Task<List<State>> GetStates();
        Task<Event> SaveEvent(Event item);
        Task<List<Event>> GetUserEvents(string userId);
        Task<Sponsor> GetSponsorById(int id);
        Task<Sponsor> SaveSponsor(Sponsor sponsor);
        Task<bool> DeleteSponsor(int sponsorId);
        Task<List<Sponsor>> GetSponsorsByEvent(int id);
        Task<int> GetSponsorCountByEventId(int eventId);
        Task<UserEventRole> SaveUserEventRole(UserEventRole role);
        Task<Order> SaveOrder(Order item, List<OrderItem> items);
        Task<Order> GetOrderById(int orderId);
        Task<bool> AreThereAdminsRemaining(string userId, int eventId);
        Task<UserEventRole> GetUserEventRoleById(int id);
        Task<bool> DeleteUserEventRole(UserEventRole role);
        Task<UserEventInvitation> SaveUserEventInvitation(UserEventInvitation invitation);
        Task<UserEventInvitation> GetUserEventInvitation(string email, int eventId);
        Task<UserEventInvitation> GetUserEventInvitationByToken(string token);
        Task<Order> GetOrderByEventId(int eventId);
    }
}
