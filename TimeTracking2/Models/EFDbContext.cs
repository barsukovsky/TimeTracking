using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TimeTracking2.Models
{
    /// <summary>
    /// Даёт возможность выполнять запросы к БД
    /// </summary>
    public class EFDbContext : DbContext
    {
        public EFDbContext() : base("EFDbContext") { }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<Report> Reports { get; set; }
    }
}