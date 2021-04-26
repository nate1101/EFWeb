using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _agendaRepository;
        public AgendaService(
            IAgendaRepository agendaRepository
            )
        {
            _agendaRepository = agendaRepository;
        }

        public async Task<List<AgendaItem>> GetAgendaItems(int eventId)
        {
            return await _agendaRepository.GetAgendaItems(eventId);
        }

        public async Task<AgendaItem> GetAgendaItem(int agendaItemId)
        {
            return await _agendaRepository.GetAgendaItem(agendaItemId);
        }

        public async Task<List<AgendaItem>> GetMyAgendaItems(int eventId, string userId)
        {
            return await _agendaRepository.GetMyAgendaItems(eventId, userId);
        }

        public async Task<AgendaItem> AddUserAgendaItem(UserAgendaItem item)
        {
            var success = await _agendaRepository.AddUserAgendaItem(item);
            if (success)
            {
                // set reminder
                if (item.ReminderMinutes > 0) {
                    await _agendaRepository.SavePushReminder(new PushReminder()
                    {
                        CreatedDate = DateTime.Now,
                        AgendaItemId = item.AgendaItemId,
                        ModifiedDate = DateTime.Now,
                        ReminderMinutes = item.ReminderMinutes,
                        Token = item.Token,
                        UserId = item.UserId
                    });
                }
                return await _agendaRepository.GetAgendaItem(item.AgendaItemId);
            }
            else
                return new AgendaItem();
        }

        public async Task<AgendaItem> RemoveUserAgendaItem(int agendaItemId, string userId)
        {
            var success = await _agendaRepository.RemoveUserAgendaItem(agendaItemId, userId);
            if (success)
                return await _agendaRepository.GetAgendaItem(agendaItemId);
            else
                return new AgendaItem();
        }

        public async Task<List<Track>> GetTracks(int eventId)
        {
            return await _agendaRepository.GetTracks(eventId);
        }

        public async Task<AgendaItem> SaveAgendaItem(AgendaItem item)
        {
            return await _agendaRepository.SaveAgendaItem(item);
        }

        public async Task<bool> RemoveAgendaItemTracks(int agendaItemId)
        {
            return await _agendaRepository.RemoveAgendaItemTracks(agendaItemId);
        }

        public async Task<bool> AddAgendaItemTracks(List<TrackAgendaItem> items)
        {
            return await _agendaRepository.AddAgendaItemTracks(items);
        }

        public async Task<Track> SaveTrack(Track item)
        {
            return await _agendaRepository.SaveTrack(item);
        }

        public async Task<bool> RemoveAgendaItem(int agendaItemId)
        {
            return await _agendaRepository.RemoveAgendaItem(agendaItemId);
        }

        public async Task<bool> RemoveUserAgendaItems(int agendaItemId)
        {
            return await _agendaRepository.RemoveUserAgendaItems(agendaItemId);
        }

        public async Task<Track> GetTrackById(int trackId)
        {
            return await _agendaRepository.GetTrackById(trackId);
        }

        public async Task<bool> RemoveTrackFromAgendaItems(int trackId)
        {
            return await _agendaRepository.RemoveTrackFromAgendaItems(trackId);
        }

        public async Task<bool> DeleteTrack(int trackId)
        {
            try
            {
                await _agendaRepository.DeleteTrack(trackId);
                await _agendaRepository.RemoveTrackFromAgendaItems(trackId);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<int> GetAgendaItemCountByEventId(int eventId)
        {
            return await _agendaRepository.GetAgendaItemCountByEventId(eventId);
        }

        public async Task<int> GetTrackCountByEventId(int eventId)
        {
            return await _agendaRepository.GetTrackCountByEventId(eventId);
        }
    }
}
