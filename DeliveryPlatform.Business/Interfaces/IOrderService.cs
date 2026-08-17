using DeliveryPlatform.Core.Entities;

namespace DeliveryPlatform.Business.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(Order order);
    }
}