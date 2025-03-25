using AutoMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using OnlineShopping.Datahub.Models.Domain;
using OnlineShopping.Datahub.Repository;
using OnlineShopping.Services.Interfaces;
using OnlineShopping.Services.DTOs.Product;

namespace OnlineShopping.Services.BusinessLogic
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return null;
                
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<ProductDTO> CreateProduct(AddProductDTO addProductDTO)
        {
            // Check if name is unique
            if (!await _productRepository.IsNameUnique(addProductDTO.Name))
                throw new Exception("Product name already exists.");
            
            var product = _mapper.Map<Product>(addProductDTO);
            var createdProduct = await _productRepository.CreateProduct(product);
            
            return _mapper.Map<ProductDTO>(createdProduct);
        }

        public async Task<ProductDTO> UpdateProduct(int id, UpdateProductDTO updateProductDTO)
        {
            var existingProduct = await _productRepository.GetProductById(id);
            if (existingProduct == null)
                return null;
                
            // Check if name is unique (excluding current product)
            if (!await _productRepository.IsNameUnique(updateProductDTO.Name, id))
                throw new Exception("Product name already exists.");
            
            _mapper.Map(updateProductDTO, existingProduct);
            var updatedProduct = await _productRepository.UpdateProduct(existingProduct);
            
            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            return await _productRepository.DeleteProduct(id);
        }

        public async Task<byte[]> ExportProductsToExcel()
        {
            var products = await _productRepository.GetAllProducts();
            
            // Create workbook and sheet
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Products");
            
            // Create header row
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("ID");
            headerRow.CreateCell(1).SetCellValue("Name");
            headerRow.CreateCell(2).SetCellValue("Description");
            headerRow.CreateCell(3).SetCellValue("Price");
            headerRow.CreateCell(4).SetCellValue("Stock Quantity");
            headerRow.CreateCell(5).SetCellValue("Created Date");
            
            // Add data rows
            int rowIndex = 1;
            foreach (var product in products)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(product.Id);
                row.CreateCell(1).SetCellValue(product.Name);
                row.CreateCell(2).SetCellValue(product.Description ?? "");
                row.CreateCell(3).SetCellValue((double)product.Price);
                row.CreateCell(4).SetCellValue(product.StockQuantity);
                row.CreateCell(5).SetCellValue(product.CreatedDate.ToString("yyyy-MM-dd"));
            }
            
            // Auto-size columns
            for (int i = 0; i < 6; i++)
            {
                sheet.AutoSizeColumn(i);
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