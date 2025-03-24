using Microsoft.EntityFrameworkCore;
using OnlineShopping.Datahub.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopping.Datahub.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OnlineShoppingDbContext _context;

        public CustomerRepository(OnlineShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _context.Customers.Where(c => c.IsActive).ToListAsync();
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            customer.CreatedDate = DateTime.Now;
            customer.IsActive = true;
            
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.Id);
            if (existingCustomer == null || !existingCustomer.IsActive)
                return null;

            existingCustomer.Name = customer.Name;
            existingCustomer.Email = customer.Email;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Address = customer.Address;
            existingCustomer.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || !customer.IsActive)
                return false;

            customer.IsActive = false;
            customer.ModifiedDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailUnique(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return !await _context.Customers.AnyAsync(c => c.Email == email && c.Id != excludeId.Value);
                
            return !await _context.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> IsPhoneNumberUnique(string phoneNumber, int? excludeId = null)
        {
            if (excludeId.HasValue)
                return !await _context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber && c.Id != excludeId.Value);
                
            return !await _context.Customers.AnyAsync(c => c.PhoneNumber == phoneNumber);
        }
    }
}