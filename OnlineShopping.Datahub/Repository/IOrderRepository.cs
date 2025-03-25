using OnlineShopping.Datahub.Models.Domain;

namespace OnlineShopping.Datahub.Repository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrders(int? customerId = null);
        Task<Order> GetOrderById(int id);
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(Order order);
        Task<bool> DeleteOrder(int id);
    }
}