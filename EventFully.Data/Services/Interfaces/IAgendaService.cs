﻿using EventFully.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EventFully.Services.Interfaces
{
    public interface IAgendaService
    {
        Task<List<AgendaItem>> GetAgendaItems(int eventId);
        Task<AgendaItem> GetAgendaItem(int agendaItemId);
        Task<List<AgendaItem>> GetMyAgendaItems(int eventId, string userId);
        Task<AgendaItem> AddUserAgendaItem(UserAgendaItem item);
        Task<AgendaItem> RemoveUserAgendaItem(int agendaItemId, string userId);
        Task<List<Track>> GetTracks(int eventId);
        Task<AgendaItem> SaveAgendaItem(AgendaItem item);
        Task<bool> RemoveAgendaItemTracks(int agendaItemId);
        Task<bool> AddAgendaItemTracks(List<TrackAgendaItem> items);
        Task<Track> SaveTrack(Track item);
        Task<bool> RemoveAgendaItem(int agendaItemId);
        Task<bool> RemoveUserAgendaItems(int agendaItemId);
        Task<Track> GetTrackById(int trackId);
        Task<bool> RemoveTrackFromAgendaItems(int trackId);
        Task<bool> DeleteTrack(int trackId);
        Task<int> GetAgendaItemCountByEventId(int eventId);
        Task<int> GetTrackCountByEventId(int eventId);
    }
}
