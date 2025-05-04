using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data {
    public class AppDbContext(DbContextOptions<AppDbContext>options):DbContext(options) {

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Car> Cars { get; set; }


    }
}
