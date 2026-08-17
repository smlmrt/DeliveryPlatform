using DeliveryPlatform.Business.Interfaces;
using DeliveryPlatform.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null)
                return BadRequest("Sipariş verisi geçersiz.");

            // Siparişi doğrudan veritabanına YAZMIYORUZ! RabbitMQ kuyruğuna fırlatıyoruz.
            await _orderService.CreateOrderAsync(order);


            return Accepted(new { message = "Siparişiniz başarıyla alındı ve kuyruğa eklendi!" });
        }
    }
}