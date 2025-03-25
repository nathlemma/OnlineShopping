using Microsoft.EntityFrameworkCore;
using OnlineShopping.Datahub.Models.Domain;

namespace OnlineShopping.Datahub.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OnlineShoppingDbContext _context;

        public OrderRepository(OnlineShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrders(int? customerId = null)
        {
            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.IsActive);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            return await query.ToListAsync();
        }

        public async Task<Order> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.IsActive);
        }

        public async Task<Order> CreateOrder(Order order)
        {
            order.CreatedDate = DateTime.Now;
            order.OrderDate = DateTime.Now;
            order.IsActive = true;
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);
            
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            
            return await GetOrderById(order.Id);
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
                
            if (existingOrder == null || !existingOrder.IsActive)
                return null;

            existingOrder.Status = order.Status;
            existingOrder.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return await GetOrderById(existingOrder.Id);
        }

        public async Task<bool> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || !order.IsActive)
                return false;

            order.IsActive = false;
            order.ModifiedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}