using OnlineShopping.Datahub.Models.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopping.Datahub.Repository
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomers();
        Task<Customer> GetCustomerById(int id);
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task<bool> DeleteCustomer(int id);
        Task<bool> IsEmailUnique(string email, int? excludeId = null);
        Task<bool> IsPhoneNumberUnique(string phoneNumber, int? excludeId = null);
    }
}