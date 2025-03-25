using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.CustomActionFilters;
using OnlineShopping.Services.DTOs.Order;
using OnlineShopping.Services.Interfaces;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders([FromQuery] int? customerId = null)
        {
            return await _orderService.GetAllOrders(customerId);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderById(id);

            if (order == null) return NotFound();
            
            return order;
        }
        
        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<OrderDTO>> CreateOrder(AddOrderDTO addOrderDTO)
        {
            var order = await _orderService.CreateOrder(addOrderDTO);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        
        [HttpPut("{id}/status")]
        [ValidateModel]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var order = await _orderService.UpdateOrderStatus(id, updateOrderStatusDTO);

            if (order == null) return NotFound();
            
            return Ok(order);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);

            if (!result) return NotFound();
            
            return NoContent();
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _orderService.ExportOrdersToExcel();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "orders.xlsx");
        }
    }
}