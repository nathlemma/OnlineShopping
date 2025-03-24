using AutoMapper;
using OnlineShopping.Datahub.Models.Domain;
using OnlineShopping.Services.DTOs.Customer;
using OnlineShopping.Services.DTOs.Product;
using OnlineShopping.Services.DTOs.Order;

namespace OnlineShopping.Services.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Customer, CustomerDTO>();
            CreateMap<AddCustomerDTO, Customer>();
            CreateMap<UpdateCustomerDTO, Customer>();
            
            CreateMap<Product, ProductDTO>();
            CreateMap<AddProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();
            
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));
            
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            
            CreateMap<AddOrderDTO, Order>();
            CreateMap<AddOrderItemDTO, OrderItem>();
        }
    }
}