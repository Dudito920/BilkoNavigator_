//айлът трябва да е ТОЧНО ТАКЪВ:

using Microsoft.EntityFrameworkCore;
using BilkoNavigator_.Models;

namespace BilkoNavigator_.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Herb> Herbs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<HerbFinding> HerbFindings { get; set; }
        public DbSet<HerbImage> HerbImages { get; set; }
    }
}
