using EventFully.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFully.Functions.Models
{
    public class EventBxDataContext : DbContext
    {
        //public EventBxDataContext(DbContextOptions<EventBxDataContext> options)
        //    : base(options)
        //{ }
        //public EventBxDataContext() : base("name=efcs") { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<Exhibitor> Exhibitor
        {
            get;
            set;
        }
    }
}
