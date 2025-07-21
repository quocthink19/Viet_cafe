using AutoMapper;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Mappers
{
    public class MapperConfigurationsProfile : Profile 
    {
        public MapperConfigurationsProfile()
        {
            CreateMap<Product, ProductResponse>();
            CreateMap<Customize, CustomizeResponse>()
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size.Name))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.CustomizeToppings, opt => opt.MapFrom(src => src.CustomizeToppings));

            CreateMap<Cart, CartResponse>();
            CreateMap<CartItem, CartItemResponse>();
            CreateMap<TopUp, TopUpResponse>()
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<PromotionRequest, Promotion>();

            CreateMap<CartItem, OrderItem>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.productId, opt => opt.MapFrom(src => src.Customize.ProductId));
            
            CreateMap<Customize, CustomerResponse>();
            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<CustomizeTopping, CustomizeToppingResponse>()
          .ForMember(dest => dest.Topping, opt => opt.MapFrom(src => src.Topping.Name))
          .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Topping.Price));

            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName ))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.Payment.ToString()));
            

            CreateMap<OrderSlotLimitRequest,OrderSlotLimit>();
        }
    }
    }

