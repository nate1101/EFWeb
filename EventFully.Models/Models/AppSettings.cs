using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    public class JWTSecurityToken
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

    public class CloudSettings
    {
        public string StorageUrl { get; set; }
        public string SpeakerPhotosContainer { get; set; }
        public string EventsPhotosContainer { get; set; }
        public string ExhibitorPhotosContainer { get; set; }
        public string SponsorPhotosContainer { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string SubscriptionId { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }

    public partial class PushSubscription
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public bool ActiveFlag { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        
    }

    public partial class PushReminder
    {
        public int Id { get; set; }
        public int AgendaItemId { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public int ReminderMinutes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public partial class PushReminderView
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
