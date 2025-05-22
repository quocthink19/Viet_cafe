using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.Models.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ICustomizeService
    {
        Task<CustomizeResponse> AddCustomize(CustomizeRequest customize);
        Task DeleteCustomize(Guid Id);
        Task<CustomizeResponse> UpdateCustomize(Guid Id, CustomizeRequest CustomizeName);
        Task<IEnumerable<Customize>> GetCustomize();
        Task<Customize> GetCustomizeById(Guid Id);
    }
}
