namespace DeliveryPlatform.Business.RabbitMQ
{
    public interface IRabbitMQPublisher
    {
        void PublishOrderMessage(string message);
    }
}