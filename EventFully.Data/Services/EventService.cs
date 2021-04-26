using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        public EventService(
            IEventRepository eventRepository
            )
        {
            _eventRepository = eventRepository;
        }

        public async Task<List<Event>> GetCurrentEvents()
        {
            return await _eventRepository.GetCurrentEvents();
        }

        public async Task<Event> GetEventById(int eventId)
        {
            return await _eventRepository.GetEventById(eventId);
        }

        public async Task<List<State>> GetStates()
        {
            return await _eventRepository.GetStates();
        }

        public async Task<Event> SaveEvent(Event item)
        {
            return await _eventRepository.SaveEvent(item);
        }

        public async Task<List<Event>> GetUserEvents(string userId)
        {
            return await _eventRepository.GetUserEvents(userId);
        }

        public async Task<Sponsor> GetSponsorById(int id)
        {
            return await _eventRepository.GetSponsorById(id);
        }

        public async Task<Sponsor> SaveSponsor(Sponsor sponsor)
        {
            return await _eventRepository.SaveSponsor(sponsor);
        }

        public async Task<bool> DeleteSponsor(int sponsorId)
        {
            return await _eventRepository.DeleteSponsor(sponsorId);
        }

        public async Task<List<Sponsor>> GetSponsorsByEvent(int id)
        {
            return await _eventRepository.GetSponsorsByEvent(id);
        }

        public async Task<int> GetSponsorCountByEventId(int eventId)
        {
            return await _eventRepository.GetSponsorCountByEventId(eventId);
        }

        public async Task<UserEventRole> SaveUserEventRole(UserEventRole role)
        {
            return await _eventRepository.SaveUserEventRole(role);
        }

        public async Task<Order> SaveOrder(Order item,List<OrderItem> items)
        {
            try
            {
                var order = await _eventRepository.SaveOrder(item);
                items.ForEach(i => i.OrderId = order.Id);
                var orderItems = await _eventRepository.SaveOrderItems(items);

                return order;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _eventRepository.GetOrderById(orderId);
        }

        public async Task<bool> AreThereAdminsRemaining(string userId, int eventId)
        {
            return await _eventRepository.AreThereAdminsRemaining(userId, eventId);
        }

        public async Task<UserEventRole> GetUserEventRoleById(int id)
        {
            return await _eventRepository.GetUserEventRoleById(id);
        }

        public async Task<bool> DeleteUserEventRole(UserEventRole role)
        {
            return await _eventRepository.DeleteUserEventRole(role);
        }

        public async Task<UserEventInvitation> SaveUserEventInvitation(UserEventInvitation invitation)
        {
            return await _eventRepository.SaveUserEventInvitation(invitation);
        }

        public async Task<UserEventInvitation> GetUserEventInvitation(string email, int eventId)
        {
            return await _eventRepository.GetUserEventInvitation(email, eventId);
        }

        public async Task<UserEventInvitation> GetUserEventInvitationByToken(string token)
        {
            return await _eventRepository.GetUserEventInvitationByToken(token);
        }

        public async Task<Order> GetOrderByEventId(int eventId)
        {
            return await _eventRepository.GetOrderByEventId(eventId);
        }
    }
}
