using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    /// <summary>
    /// Speaker Model
    /// </summary>
    public class Speaker
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Bio { get; set; }
        public string ProfilePic { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedByUserId { get; set; }
        public virtual Event Event { get; set; }
    }

    public class SpeakerListView
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Bio { get; set; }
        public string ProfilePic { get; set; }
        public int SessionsAssigned { get; set; }
        public int EventId { get; set; }
    }
}
