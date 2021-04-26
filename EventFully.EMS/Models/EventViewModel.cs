using EventFully.Models;
using System;
using System.Collections.Generic;

namespace EventFully.EMS.Models
{
    public class EventViewModel
    {
        public int SpeakerCount { get; set; }
        public int ScheduleCount { get; set; }
        public int ExhibitorCount { get; set; }
        public int TrackCount { get; set; }
        public int SponsorCount { get; set; }
        public Event Event { get; set; }
        public List<Event> Events { get; set; }
        public string StripeToken { get; set; }
        public Order Order { get; set; }
        public string MinStartDate { get; set; }
        public string StripeSessionId { get; set; }
        public List<DateTime> AgendaDates { get; set; }
    }

    public class SpeakerViewModel
    {
        public Speaker Speaker { get; set; }
    }

    public class ExhibitorViewModel
    {
        public Exhibitor Exhibitor { get; set; }
    }

    public class AgendaViewModel
    {
        public AgendaItem AgendaItem { get; set; }
        public IEnumerable<string> SelectedSpeakers { get; set; }
        public IEnumerable<string> SelectedTracks { get; set; }
        public IEnumerable<Track> Tracks { get; set; }

        public string AddTrackName { get; set; }
    }

    public class TrackViewModel
    {
        public Track Track { get; set; }
    }

    public class SponsorViewModel
    {
        public Sponsor Sponsor { get; set; }
    }

    public class UserEventInvitationViewModel
    {
        public UserEventInvitation UserEventInvitation { get; set; }
    }

    public class ImportScheduleViewModel
    {
        public Event Event { get; set; }
        public List<ImportedScheduleItem> ImportedScheduleItems { get; set; }
    }

    public class ImportedScheduleItem
    {
        public int Id { get; set; }
        public DateTime? SessionDate { get; set; }
        public string SessionStartTime { get; set; }
        public string SessionEndTime { get; set; }
        public string SessionTitle { get; set; }
        public string SessionDescription { get; set; }
        public string SessionLocation { get; set; }
        public string SessionTrack { get; set; }
        public string SessionSpeaker { get; set; }
        public string ErrorMessage { get; set; }
    }
}