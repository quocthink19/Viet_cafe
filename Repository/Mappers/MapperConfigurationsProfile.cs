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
            .ForMember(dest => dest.Milk, opt => opt.MapFrom(src => src.Milk.ToString()))
            .ForMember(dest => dest.Ice, opt => opt.MapFrom(src => src.Ice.ToString()))
            .ForMember(dest => dest.Sugar, opt => opt.MapFrom(src => src.Sugar.ToString()))
            .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature.ToString()))
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size.Name))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.CustomizeToppings, opt => opt.MapFrom(src => src.CustomizeToppings));

            CreateMap<CustomizeTopping, CustomizeToppingResponse>()
                .ForMember(dest => dest.Topping, opt => opt.MapFrom(src => src.Topping.Name));
            CreateMap<Cart, CartResponse>();
            CreateMap<CartItem, CartItemResponse>();

            CreateMap<PromotionRequest, Promotion>();

            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomizeTopping, CustomizeToppingResponse>()
          .ForMember(dest => dest.Topping, opt => opt.MapFrom(src => src.Topping.Name));
        }
    }
    }

