using AutoMapper;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using OnlineShopping.Datahub.Models.Domain;
using OnlineShopping.Datahub.Repository;
using OnlineShopping.Services.Interfaces;

using OnlineShopping.Services.DTOs.Customer;

namespace OnlineShopping.Services.BusinessLogic
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<List<CustomerDTO>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllCustomers();
            return _mapper.Map<List<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO> GetCustomerById(int id)
        {
            var customer = await _customerRepository.GetCustomerById(id);
            if (customer == null)
                return null;
                
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> CreateCustomer(AddCustomerDTO addCustomerDTO)
        {
            // Check if email and phone are unique
            if (!await _customerRepository.IsEmailUnique(addCustomerDTO.Email))
                throw new Exception("Email is already in use.");
                
            if (!await _customerRepository.IsPhoneNumberUnique(addCustomerDTO.PhoneNumber))
                throw new Exception("Phone number is already in use.");
            
            var customer = _mapper.Map<Customer>(addCustomerDTO);
            var createdCustomer = await _customerRepository.CreateCustomer(customer);
            
            return _mapper.Map<CustomerDTO>(createdCustomer);
        }

        public async Task<CustomerDTO> UpdateCustomer(int id, UpdateCustomerDTO updateCustomerDTO)
        {
            var existingCustomer = await _customerRepository.GetCustomerById(id);
            if (existingCustomer == null)
                return null;
                
            // Check if email and phone are unique (excluding current customer)
            if (!await _customerRepository.IsEmailUnique(updateCustomerDTO.Email, id))
                throw new Exception("Email is already in use.");
                
            if (!await _customerRepository.IsPhoneNumberUnique(updateCustomerDTO.PhoneNumber, id))
                throw new Exception("Phone number is already in use.");
            
            _mapper.Map(updateCustomerDTO, existingCustomer);
            var updatedCustomer = await _customerRepository.UpdateCustomer(existingCustomer);
            
            return _mapper.Map<CustomerDTO>(updatedCustomer);
        }

        public async Task<bool> DeleteCustomer(int id)
        {
            return await _customerRepository.DeleteCustomer(id);
        }

        public async Task<byte[]> ExportCustomersToExcel()
        {
            var customers = await _customerRepository.GetAllCustomers();
            
            // Create workbook and sheet
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("Customers");
            
            // Create header row
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("ID");
            headerRow.CreateCell(1).SetCellValue("Name");
            headerRow.CreateCell(2).SetCellValue("Email");
            headerRow.CreateCell(3).SetCellValue("Phone Number");
            headerRow.CreateCell(4).SetCellValue("Address");
            headerRow.CreateCell(5).SetCellValue("Created Date");
            
            // Add data rows
            int rowIndex = 1;
            foreach (var customer in customers)
            {
                var row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(customer.Id);
                row.CreateCell(1).SetCellValue(customer.Name);
                row.CreateCell(2).SetCellValue(customer.Email);
                row.CreateCell(3).SetCellValue(customer.PhoneNumber);
                row.CreateCell(4).SetCellValue(customer.Address);
                row.CreateCell(5).SetCellValue(customer.CreatedDate.ToString("yyyy-MM-dd"));
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