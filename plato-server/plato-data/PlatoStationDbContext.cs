using Microsoft.EntityFrameworkCore;
using plato_data.Models;

namespace plato_data
{
    public class PlatoStationDbContext: DbContext
    {
        public PlatoStationDbContext(DbContextOptions<PlatoStationDbContext> o) : base(o) { }
        public DbSet<User> Users { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
            });
        }
    }
}
