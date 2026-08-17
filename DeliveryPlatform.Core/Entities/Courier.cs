namespace DeliveryPlatform.Core.Entities
{
    public class Courier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public double CurrentLatitude { get; set; }
        public double CurrentLongtitude { get; set; }

        // Bir kuryenin birden fazla siparişi olabilir
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}