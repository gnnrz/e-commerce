using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Service;
using CleanArchitecture.Application.Strategies;
using CleanArchitecture.Domain.Interface;
using CleanArchitecture.Web.DTOs;

namespace CleanArchitecture.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;        

        public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService; 
        }

        [HttpPost("/create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] List<OrderItem> items)
        {
            var result = await _orderService.CreateOrderAsync(items);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, result.Value);
        }

        [HttpGet("/gettAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("/getOrderById")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var maybeOrder = await _orderService.GetOrderByIdAsync(id);

            if (maybeOrder.HasNoValue)
                return NotFound("Pedido não encontrado.");

            return Ok(maybeOrder.Value);
        }

        [HttpPut("/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrderAsync(id);

            if (result.IsFailure)
                return NotFound(result.Error);

            return NoContent();
        }


        [HttpPut("/process-payment")]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentRequestDto paymentRequest, bool usePolly = false)
        {
            IPaymentStrategy? paymentStrategy = paymentRequest.PaymentMethod switch
            {
                "pix" => new PixPaymentStrategy(),
                "card" => new CardPaymentStrategy(),
                _ => null
            };

            if (paymentStrategy == null)
                return BadRequest("Método de pagamento inválido.");

            var result = await _orderService.ProcessPaymentAsync(id, paymentStrategy, usePolly);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok("Pagamento processado com sucesso.");
        }
    }
}
