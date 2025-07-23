using AutoMapper;
using ShoeStore.API.DTOs;
using ShoeStore.API.Models;

namespace ShoeStore.API.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity));

            CreateMap<CreateProductDTO, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));

            CreateMap<UpdateProductDTO, Product>()
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));

            // Product Variant mappings
            CreateMap<ProductVariant, ProductVariantResponseDTO>();
            CreateMap<CreateProductVariantDTO, ProductVariant>();

            // Category mappings
            CreateMap<Category, CategoryResponseDTO>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count(p => p.IsActive)));

            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();

            // Brand mappings
            CreateMap<Brand, BrandResponseDTO>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count(p => p.IsActive)));

            CreateMap<CreateBrandDTO, Brand>();
            CreateMap<UpdateBrandDTO, Brand>();

            // Order mappings
            CreateMap<Order, OrderResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.Phone));

            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow.AddHours(7)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<CreateOrderDetailDTO, OrderDetail>();

            CreateMap<OrderDetail, OrderDetailResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Price * src.Quantity));

            // Cart mappings
            CreateMap<Cart, CartResponseDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name));

            CreateMap<CartItem, CartItemResponseDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Product.Price * src.Quantity));

            // User Account mappings
            CreateMap<UserAccount, UserAccountResponseDTO>();
            CreateMap<CreateUserAccountDTO, UserAccount>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)); // Note: Should hash password in service

            CreateMap<UpdateUserAccountDTO, UserAccount>();

            // Customer mappings
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserAccount.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserAccount.Username))
                .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.Orders.Count));

            CreateMap<CreateCustomerDTO, Customer>();
            CreateMap<UpdateCustomerDTO, Customer>();
        }
    }
} 