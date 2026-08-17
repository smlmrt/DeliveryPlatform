namespace DeliveryPlatform.Core.Entities
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Bir restoranın birden fazla siparişi olabilir
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}