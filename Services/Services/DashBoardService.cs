using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class DashBoardService : IDashBoardService 
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashBoardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
