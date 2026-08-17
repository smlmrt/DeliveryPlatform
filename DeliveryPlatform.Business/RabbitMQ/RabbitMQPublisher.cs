using RabbitMQ.Client;
using System.Text;

namespace DeliveryPlatform.Business.RabbitMQ
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        public void PublishOrderMessage(string message)
        {
            // RabbitMQ sunucusuna bağlanıyoruz (Şimdilik localhost'ta çalışacağını varsayıyoruz)
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // "order_queue" adında bir kuyruk tanımlıyoruz. Durable=true ile sunucu çökse bile mesajlar silinmez!
            channel.QueueDeclare(queue: "order_queue",
                                 durable: true, 
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            // Mesajı kuyruğa fırlatıyoruz
            channel.BasicPublish(exchange: "",
                                 routingKey: "order_queue",
                                 basicProperties: null,
                                 body: body);
        }
    }
}