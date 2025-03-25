using OnlineShopping.Services.DTOs.Customer;

namespace OnlineShopping.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetAllCustomers();
        Task<CustomerDTO> GetCustomerById(int id);
        Task<CustomerDTO> CreateCustomer(AddCustomerDTO addCustomerDTO);
        Task<CustomerDTO> UpdateCustomer(int id, UpdateCustomerDTO updateCustomerDTO);
        Task<bool> DeleteCustomer(int id);
        Task<byte[]> ExportCustomersToExcel();
    }
}