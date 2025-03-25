using OnlineShopping.Datahub.Models.Domain;

namespace OnlineShopping.Datahub.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task<bool> DeleteProduct(int id);
        Task<bool> IsNameUnique(string name, int? excludeId = null);
    }
}