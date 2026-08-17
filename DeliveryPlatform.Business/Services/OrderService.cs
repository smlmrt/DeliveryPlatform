using DeliveryPlatform.Business.Interfaces;
using DeliveryPlatform.Business.RabbitMQ;
using DeliveryPlatform.Core.Entities;
using System.Text.Json;

namespace DeliveryPlatform.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRabbitMQPublisher _rabbitPublisher;

        public OrderService(IRabbitMQPublisher rabbitPublisher)
        {
            _rabbitPublisher = rabbitPublisher;
        }

        public async Task CreateOrderAsync(Order order)
        {
            // İŞTE SİHRİN OLDUĞU YER!
            // Siparişi doğrudan veritabanına yazmak YERİNE, JSON'a çevirip RabbitMQ kuyruğuna atıyoruz.
            var orderJson = JsonSerializer.Serialize(order);
            
            _rabbitPublisher.PublishOrderMessage(orderJson);

            // Veritabanı işlemi yapmadığımız için Task'i manuel tamamlıyoruz
            await Task.CompletedTask; 
        }
    }
}