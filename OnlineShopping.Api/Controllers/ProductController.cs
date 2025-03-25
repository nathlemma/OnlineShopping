using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.CustomActionFilters;
using OnlineShopping.Services.DTOs.Product;
using OnlineShopping.Services.Interfaces;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            return await _productService.GetAllProducts();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null) return NotFound();

            return product;
        }
        
        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<ProductDTO>> CreateProduct(AddProductDTO addProductDTO)
        {
            var product = await _productService.CreateProduct(addProductDTO);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDTO updateProductDTO)
        {
            var product = await _productService.UpdateProduct(id, updateProductDTO);
            if (product == null) return NotFound();
            
            return Ok(product);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);

            if (!result) return NotFound();

            return NoContent();
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _productService.ExportProductsToExcel();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "products.xlsx");
        }
    }
}