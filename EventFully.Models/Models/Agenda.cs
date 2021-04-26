using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    /// <summary>
    /// Agenda Item Model
    /// </summary>
    public class AgendaItem
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<AgendaItemSpeaker> AgendaItemSpeakers { get; set; }
        public virtual ICollection<UserAgendaItem> UserAgendaItems { get; set; }
        public virtual ICollection<TrackAgendaItem> TrackAgendaItems { get; set; }
        [NotMapped]
        public bool CurrentSpeakerAssigned { get; set; }
        [NotMapped]
        public string StartTime { get; set; }
        [NotMapped]
        public string EndTime { get; set; }
    }

    public class AgendaItemSpeaker
    {
        public int Id { get; set; }
        public int AgendaItemId { get; set; }
        public int SpeakerId { get; set; }
        //public virtual AgendaItem AgendaItem { get; set; }
        public virtual Speaker Speaker { get; set; }
    }

    public class UserAgendaItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AgendaItemId { get; set; }
        [NotMapped]
        public int ReminderMinutes { get; set; }
        [NotMapped]
        public string Token { get; set; }
    }

    public class Track
    {
        public int Id { get; set; }
        public string TrackName { get; set; }
        public int EventId { get; set; }
        public string Description { get; set; }
        public string HexColor { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        [NotMapped]
        public bool Selected { get; set; }
        //public virtual ICollection<TrackAgendaItem> TrackAgendaItems { get; set; }
    }

    public class TrackListView
    {
        public int Id { get; set; }
        public string TrackName { get; set; }
        public int EventId { get; set; }
        public string Description { get; set; }
        public string HexColor { get; set; }
        public int Sessions { get; set; }
    }

    public class TrackAgendaItem
    {
        public int Id { get; set; }
        public int TrackId { get; set; }
        public int AgendaItemId { get; set; }
        public virtual Track Track { get; set; }
    }

    public class AgendaItemView
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public string ScheduledDate { get; set; }
        public string ScheduledTime { get; set; }
        public int LocationId { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
    }

    public class AgendaItemCalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int EventId{ get; set; }
        public bool AllDay { get; set; }
        public ICollection<TrackAgendaItem> TrackAgendaItems { get; set; }
    }

    //public class AgendaItemTrack
    //{
    //    public int Id { get; set; }
    //    public int AgendaItemId { get; set; }
    //    public int TrackId { get; set; }
    //    public virtual Track Speaker { get; set; }
    //}
}
