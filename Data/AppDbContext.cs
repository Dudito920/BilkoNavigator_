//айлът трябва да е ТОЧНО ТАКЪВ:

using Microsoft.EntityFrameworkCore;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BilkoNavigator_.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Herb> Herbs => Set<Herb>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<HerbFinding> HerbFindings => Set<HerbFinding>();
        public DbSet<HerbImage> HerbImages => Set<HerbImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ⚠️ важно!

            modelBuilder.Entity<Herb>()
                .HasIndex(h => h.LatinName)
                .IsUnique();

            modelBuilder.Entity<Herb>()
                .HasOne(h => h.Image)
                .WithOne(i => i.Herb)
                .HasForeignKey<HerbImage>(i => i.HerbId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
