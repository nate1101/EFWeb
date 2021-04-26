using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal OrderTotal { get; set; }
        public string BillingName { get; set; }
        public string BillingAddressLine1 { get; set; }
        public string BillingAddressApt { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressZip { get; set; }
        public string BillingAddressCountry { get; set; }
        public string BillingAddressCountryCode { get; set; }
        public string StripePaymentId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserId { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Decimal ItemAmount { get; set; }
        public Decimal ProcessingFees { get; set; }
    }

    public class OrderSummaryView
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; }
        public DateTime OrderDate { get; set; }
        public Decimal OrderTotal { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string OrderItems { get; set; }
    }
}