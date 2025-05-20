using Repository.Models;
using Repository.Models.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IServices
{
    public interface ISizeService
    {
        Task<Size> AddSize(SizeRequest size);
        Task DeleteSize(Guid Id);
        Task<Size> UpdateSize(Guid Id, SizeRequest size);
        Task<IEnumerable<Size>> GetSize();
        Task<Size> GetSizeById(Guid Id);
    }
}
