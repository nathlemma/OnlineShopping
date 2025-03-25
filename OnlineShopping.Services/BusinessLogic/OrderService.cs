using AutoMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using OnlineShopping.Datahub.Models.Domain;
using OnlineShopping.Datahub.Repository;
using OnlineShopping.Services.DTOs.Order;
using OnlineShopping.Services.Interfaces;

namespace OnlineShopping.Services.BusinessLogic
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderDTO>> GetAllOrders(int? customerId = null)
        {
            var orders = await _orderRepository.GetAllOrders(customerId);
            return _mapper.Map<List<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return null;
                
            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO> CreateOrder(AddOrderDTO addOrderDTO)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetCustomerById(addOrderDTO.CustomerId);
            if (customer == null)
                throw new Exception("Customer not found.");
                
            // Create order with items
            var order = new Order
            {
                CustomerId = addOrderDTO.CustomerId,
                OrderItems = new List<OrderItem>(),
                Customer = customer
            };
            
            // Process each order item
            foreach (var itemDto in addOrderDTO.OrderItems)
            {
                // Validate product exists and has sufficient stock
                var product = await _productRepository.GetProductById(itemDto.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {itemDto.ProductId} not found.");
                    
                if (product.StockQuantity < itemDto.Quantity)
                    throw new Exception($"Insufficient stock for product {product.Name}.");
                
                // Create order item
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = product.Price * itemDto.Quantity,
                    Product = product,
                    Order = order
                };
                
                order.OrderItems.Add(orderItem);
                
                // Update product stock
                product.StockQuantity -= itemDto.Quantity;
                await _productRepository.UpdateProduct(product);
            }
            
            // Save order
            var createdOrder = await _orderRepository.CreateOrder(order);
            return _mapper.Map<OrderDTO>(createdOrder);
        }

        public async Task<OrderDTO> UpdateOrderStatus(int id, UpdateOrderStatusDTO updateOrderStatusDTO)
        {
            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
                return null;
            
            order.Status = updateOrderStatusDTO.Status;
            var updatedOrder = await _orderRepository.UpdateOrder(order);
            
            return _mapper.Map<OrderDTO>(updatedOrder);
        }

        public async Task<bool> DeleteOrder(int id)
        {
            return await _orderRepository.DeleteOrder(id);
        }

        public async Task<byte[]> ExportOrdersToExcel()
        {
            var orders = await _orderRepository.GetAllOrders();
            
            // Create workbook and sheets
            var workbook = new XSSFWorkbook();
            var orderSheet = workbook.CreateSheet("Orders");
            var itemSheet = workbook.CreateSheet("Order Items");
            
            // Create header row for Orders
            var headerRow = orderSheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Order ID");
            headerRow.CreateCell(1).SetCellValue("Customer");
            headerRow.CreateCell(2).SetCellValue("Order Date");
            headerRow.CreateCell(3).SetCellValue("Status");
            headerRow.CreateCell(4).SetCellValue("Total Amount");
            headerRow.CreateCell(5).SetCellValue("Items Count");
            
            // Add data rows for Orders
            int rowIndex = 1;
            foreach (var order in orders)
            {
                var row = orderSheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(order.Id);
                row.CreateCell(1).SetCellValue(order.Customer?.Name ?? "Unknown");
                row.CreateCell(2).SetCellValue(order.OrderDate.ToString("yyyy-MM-dd HH:mm"));
                row.CreateCell(3).SetCellValue(order.Status.ToString());
                row.CreateCell(4).SetCellValue((double)order.TotalAmount);
                row.CreateCell(5).SetCellValue(order.OrderItems.Count);
            }
            
            // Auto-size columns for Orders
            for (int i = 0; i < 6; i++)
            {
                orderSheet.AutoSizeColumn(i);
            }
            
            // Create header row for Order Items
            var itemHeaderRow = itemSheet.CreateRow(0);
            itemHeaderRow.CreateCell(0).SetCellValue("Order ID");
            itemHeaderRow.CreateCell(1).SetCellValue("Product");
            itemHeaderRow.CreateCell(2).SetCellValue("Quantity");
            itemHeaderRow.CreateCell(3).SetCellValue("Unit Price");
            itemHeaderRow.CreateCell(4).SetCellValue("Subtotal");
            
            // Add data rows for Order Items
            int itemRowIndex = 1;
            foreach (var order in orders)
            {
                foreach (var item in order.OrderItems)
                {
                    var row = itemSheet.CreateRow(itemRowIndex++);
                    row.CreateCell(0).SetCellValue(order.Id);
                    row.CreateCell(1).SetCellValue(item.Product?.Name ?? "Unknown");
                    row.CreateCell(2).SetCellValue(item.Quantity);
                    row.CreateCell(3).SetCellValue((double)item.UnitPrice);
                    row.CreateCell(4).SetCellValue((double)item.Subtotal);
                }
            }
            
            // Auto-size columns for Order Items
            for (int i = 0; i < 5; i++)
            {
                itemSheet.AutoSizeColumn(i);
            }
            
            // Convert to byte array
            using (var memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}