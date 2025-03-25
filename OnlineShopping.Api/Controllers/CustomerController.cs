using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.CustomActionFilters;
using OnlineShopping.Services.DTOs.Customer;
using OnlineShopping.Services.Interfaces;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            return await _customerService.GetAllCustomers();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomerById(id);

            if (customer == null) return NotFound();
            

            return customer;
        }
        
        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<CustomerDTO>> CreateCustomer(AddCustomerDTO addCustomerDTO)
        {
            var customer = await _customerService.CreateCustomer(addCustomerDTO);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }
        
        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateCustomer(int id, UpdateCustomerDTO updateCustomerDTO)
        {
            var customer = await _customerService.UpdateCustomer(id, updateCustomerDTO);

            if (customer == null) return NotFound();
            

            return Ok(customer);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomer(id);

            if (!result) return NotFound();
            
            return NoContent();
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel()
        {
            var excelBytes = await _customerService.ExportCustomersToExcel();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "customers.xlsx");
        }
    }
}