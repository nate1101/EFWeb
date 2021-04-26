using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    /// <summary>
    /// Exhibitor Model
    /// </summary>
    public class Exhibitor
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string ExhibitorName { get; set; }
        public string BoothLocation { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
        public string ProfilePic { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        //public virtual Event Event { get; set; }
        [NotMapped]
        public string EventName { get; set; }
    }

    public class EventExhibitor
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ExhibitorId { get; set; }
    }

    public class ExhibitorListView
    {
        public int Id { get; set; }
        public string ExhibitorName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Description { get; set; }
        public string ProfilePic { get; set; }
        public int EventId { get; set; }
    }
}