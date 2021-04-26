using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventFully.Models
{
    public class EventfullyDBContext: DbContext
    {
        public EventfullyDBContext(DbContextOptions<EventfullyDBContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

        public DbSet<PushReminderView> PushReminderView { get; set; }

        public DbSet<PushReminder> PushReminder { get; set; }
        public DbSet<PushSubscription> PushSubscription { get; set; }
        public DbSet<UserEventInvitation> UserEventInvitation { get; set; }
        public DbSet<UserEventRoleView> UserEventRoleView { get; set; }
        public DbSet<OrderSummaryView> OrderSummaryView { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<Sponsor> Sponsor { get; set; }
        public DbSet<TrackListView> TrackListView { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserEventRole> UserEventRole { get; set; }
        public DbSet<ExhibitorListView> ExhibitorListView { get; set; }
        public DbSet<AgendaItemView> AgendaItemView { get; set; }
        public DbSet<SpeakerListView> SpeakerListView { get; set; }
        public DbSet<Exhibitor> Exhibitor { get; set; }
        public DbSet<EventExhibitor> EventExhibitor { get; set; }
        public DbSet<Track> Track { get; set; }
        public DbSet<TrackAgendaItem> TrackAgendaItem { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<AgendaItemSpeaker> AgendaItemSpeaker { get; set; }
        public DbSet<Speaker> Speaker { get; set; }
        public DbSet<AgendaItem> AgendaItem { get; set; }
        //public DbSet<Location> Location { get; set; }
        public DbSet<UserAgendaItem> UserAgendaItem { get; set; }
        public DbSet<State> State { get; set; }
    }

    public class ApplicationUserDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationUserDbContext(DbContextOptions<ApplicationUserDbContext> options) : base(options)
        {

        }
    }
}
