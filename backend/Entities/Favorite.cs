namespace backend.Entities {
    public class Favorite {
        public Guid UserId { get; set; }
        public Guid CarId { get; set; }

        // Navigation properties (optional, but recommended)
        public User User { get; set; }
        public Car Car { get; set; }

    }
}
