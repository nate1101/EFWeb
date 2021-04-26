using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface ISpeakerService
    {
        Task<Speaker> GetSpeakerById(int speakerId);
        Task<List<Speaker>> GetSpeakersByEventId(int eventId);
        Task<int> GetSpeakerCountByEventId(int eventId);
        Task<Speaker> SaveSpeaker(Speaker speaker);
        Task<AgendaItemSpeaker> AddAgendaItemToSpeaker(AgendaItemSpeaker item);
        Task<bool> RemoveAgendaItemToSpeaker(int agendaItemId, int speakerId);
        Task<bool> DeleteSpeaker(int speakerId);
        Task<bool> RemoveAgendaItemSpeakers(int agendaItemId);
        Task<bool> AddAgendaItemSpeakers(List<AgendaItemSpeaker> items);
    }
}
