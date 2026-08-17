using Microsoft.AspNetCore.SignalR;

namespace DeliveryPlatform.Web.Hubs
{
    public class CourierHub : Hub
    {
        // Kurye mobil uygulamasından (veya simülasyondan) tetiklenecek olan metot
        public async Task SendCourierLocation(int orderId, double latitude, double longitude)
        {
            // "ReceiveLocation" dinleyicisine sahip olan tüm kullanıcılara (Müşterilere) 
            // kuryenin anlık konumunu saniyesinde fırlatıyoruz (WebSockets)
            await Clients.All.SendAsync("ReceiveLocation", orderId, latitude, longitude);
        }
    }
}