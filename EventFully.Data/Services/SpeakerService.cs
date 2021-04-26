using EventFully.Models;
using EventFully.Repositories.Interfaces;
using EventFully.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services
{
    public class SpeakerService : ISpeakerService
    {
        private readonly ISpeakerRepository _speakerRepository;
        public SpeakerService(
            ISpeakerRepository speakerRepository
            )
        {
            _speakerRepository = speakerRepository;
        }

        public async Task<Speaker> GetSpeakerById(int speakerId)
        {
            return await _speakerRepository.GetSpeakerById(speakerId);
        }

        public async Task<List<Speaker>> GetSpeakersByEventId(int eventId)
        {
            return await _speakerRepository.GetSpeakersByEventId(eventId);
        }

        public async Task<int> GetSpeakerCountByEventId(int eventId)
        {
            return await _speakerRepository.GetSpeakerCountByEventId(eventId);
        }

        public async Task<Speaker> SaveSpeaker(Speaker speaker)
        {
            return await _speakerRepository.SaveSpeaker(speaker);
        }

        public async Task<AgendaItemSpeaker> AddAgendaItemToSpeaker(AgendaItemSpeaker item)
        {
            return await _speakerRepository.AddAgendaItemToSpeaker(item);
        }

        public async Task<bool> RemoveAgendaItemToSpeaker(int agendaItemId, int speakerId)
        {
            return await _speakerRepository.RemoveAgendaItemToSpeaker(agendaItemId, speakerId);
        }

        public async Task<bool> DeleteSpeaker(int speakerId)
        {
            return await _speakerRepository.DeleteSpeaker(speakerId);
        }

        public async Task<bool> RemoveAgendaItemSpeakers(int agendaItemId)
        {
            return await _speakerRepository.RemoveAgendaItemSpeakers(agendaItemId);
        }

        public async Task<bool> AddAgendaItemSpeakers(List<AgendaItemSpeaker> items)
        {
            return await _speakerRepository.AddAgendaItemSpeakers(items);
        }
    }
}
