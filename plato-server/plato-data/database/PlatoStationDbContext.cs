using Microsoft.EntityFrameworkCore;
using plato.data.Models;

namespace plato.data.database
{
    internal class PlatoStationDbContext : DbContext
    {
        public PlatoStationDbContext(DbContextOptions<PlatoStationDbContext> o) : base(o) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<TemperatureProfile> TemperatureProfiles { get; set; }
        public DbSet<TemperatureSchedule> TemperatureSchedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });

            modelBuilder.Entity<TemperatureSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.Start).IsRequired();
                entity.Property(e => e.End).IsRequired();
                entity.Property(e => e.Profile).IsRequired();
                entity.HasOne(p => p.Profile).WithMany(s => s.Schedules);
            });

            modelBuilder.Entity<TemperatureProfile>(entity =>
           {
               entity.HasKey(e => e.Id);
               entity.Property(p => p.Name).IsRequired();
           });
        }
    }
}
