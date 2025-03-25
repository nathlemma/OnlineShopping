using OnlineShopping.Services.DTOs.Order;

namespace OnlineShopping.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetAllOrders(int? customerId = null);
        Task<OrderDTO> GetOrderById(int id);
        Task<OrderDTO> CreateOrder(AddOrderDTO addOrderDTO);
        Task<OrderDTO> UpdateOrderStatus(int id, UpdateOrderStatusDTO updateOrderStatusDTO);
        Task<bool> DeleteOrder(int id);
        Task<byte[]> ExportOrdersToExcel();
    }
}