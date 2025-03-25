using OnlineShopping.Services.DTOs.Product;

namespace OnlineShopping.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllProducts();
        Task<ProductDTO> GetProductById(int id);
        Task<ProductDTO> CreateProduct(AddProductDTO addProductDTO);
        Task<ProductDTO> UpdateProduct(int id, UpdateProductDTO updateProductDTO);
        Task<bool> DeleteProduct(int id);
        Task<byte[]> ExportProductsToExcel();
    }
}