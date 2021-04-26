using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    /// <summary>
    /// Sponsor Model
    /// </summary>
    public class Sponsor
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string SponsorName { get; set; }
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
        public string BannerImage { get; set; }
        public string BannerImageUrl { get; set; }
        public string Website { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        //public virtual Event Event { get; set; }
        [NotMapped]
        public string EventName { get; set; }
    }
}