using AutoMapper;
using Repository.Models;
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
        }
    }
}
