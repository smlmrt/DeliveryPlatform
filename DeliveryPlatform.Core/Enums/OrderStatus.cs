namespace DeliveryPlatform.Core.Enums
{
    public enum OrderStatus
    {
        Pending = 1, // Sipariş RabbitMQ kuyruğunda / Restoran onayı bekliyor
        Preparing = 2, // Restoran onayladı, hazırlanıyor
        OnTheWay = 3, // Kurye yola çıktı (SignalR takibi başlar)
        Delivered = 4  // Teslim edildi
    }
}