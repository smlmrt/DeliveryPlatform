using DeliveryPlatform.Core.Enums;

namespace DeliveryPlatform.Core.Entities
{
    public class Order : BaseEntity
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }

        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }


        // Sipariş ilk verildiğinde kurye belli olmadığından "int?" nullable yapıyoruz
        public int? CourierId { get; set; }
        public Courier? Courier { get; set; }

    }
}