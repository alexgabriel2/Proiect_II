namespace backend.Models {
    public class CarAddDTO {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Milleage { get; set; }
        public int Price { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string Transmission { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
    public class CarCardDTO {
        
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Milleage { get; set; }
        public int Price { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
