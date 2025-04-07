using Microsoft.EntityFrameworkCore;

namespace WorkdayCalendarApi.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<HolidayEntity> Holidays { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly setting table names
            modelBuilder.Entity<HolidayEntity>().ToTable("Holidays");
            modelBuilder.Entity<HolidayEntity>()
            .HasIndex(h => h.PublicId)
            .IsUnique();

        }
    }

}
