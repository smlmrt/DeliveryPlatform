using DeliveryPlatform.Core.Entities;
using DeliveryPlatform.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DeliveryPlatform.Worker
{
    public class OrderProcessingWorker : BackgroundService
    {
        private readonly ILogger<OrderProcessingWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // Scoped (DbContext) nesneleri üretmek için fabrika
        private IConnection? _connection;
        private IModel? _channel;

        public OrderProcessingWorker(ILogger<OrderProcessingWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            
            _channel.QueueDeclare(queue: "order_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            
            // "async" keyword'ünü ekleyerek veritabanı işlemlerini bekleyebilir hale getirdik
            consumer.Received += async (ch, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                _logger.LogInformation($"[📦 YENİ SİPARİŞ YAKALANDI] Kuyruktan Okundu.");

                // 1. JSON metnini C# Order nesnesine çevir (Deserialize)
                var order = JsonSerializer.Deserialize<Order>(message);

                if (order != null)
                {
                    // 2. Yeni bir çalışma alanı (Scope) yarat ve veritabanına kaydet
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IRepository<Order>>();
                        
                        await repository.AddAsync(order);
                        await repository.SaveChangesAsync(); // Kayıt işlemi (ID otomatik oluşacak)

                        _logger.LogInformation($"[✅ BAŞARILI] Sipariş veritabanına işlendi! Atanan ID: {order.Id}");
                    }
                }
                
                // Mesajı başarıyla işlediğimizi RabbitMQ'ya bildir
                _channel!.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "order_queue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}