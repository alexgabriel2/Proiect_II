using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data {
    public class AppDbContext(DbContextOptions<AppDbContext>options):DbContext(options) {

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Car> Cars { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.CarId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Car)
                .WithMany()
                .HasForeignKey(f => f.CarId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
