using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    /// <summary>
    /// Event Model
    /// </summary>
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime EndDate { get; set; }
        public string EventThumb { get; set; }
        public string EventBanner { get; set; }
        public string VenueName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string AddressLabel { get; set; }
        public string AddressLabelValue { get; set; }
        public string TimeZone { get; set; }
        public int TimeZoneOffsetSeconds { get; set; }
        public bool Published { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string PublishedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        public virtual ICollection<UserEventRole> UserEventRoles { get; set; }
        [NotMapped]
        public Order EventOrder { get; set; }
    }

    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public string StateAbbreviation { get; set; }
    }
}
