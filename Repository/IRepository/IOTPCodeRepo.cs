using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IOTPCodeRepo : IRepository<OTPCode> 
    {
        Task<OTPCode> GetValidCodeAsync(User user, string code);
    }
}
