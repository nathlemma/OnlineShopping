using Microsoft.EntityFrameworkCore;
using OnlineShopping.Datahub.Models.Domain;

namespace OnlineShopping.Datahub.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly OnlineShoppingDbContext _context;

        public ProductRepository(OnlineShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.Where(p => p.IsActive).ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product> CreateProduct(Product product)
        {
            product.CreatedDate = DateTime.Now;
            product.IsActive = true;
            
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            
            return product;
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null || !existingProduct.IsActive) return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || !product.IsActive) return false;

            product.IsActive = false;
            product.ModifiedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsNameUnique(string name, int? excludeId = null)
        {
            if (excludeId.HasValue) return !await _context.Products.AnyAsync(p => p.Name == name && p.Id != excludeId.Value);
                
            return !await _context.Products.AnyAsync(p => p.Name == name);
        }
    }
}