using AutoMapper;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Response;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class TopUpService : ITopUpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TopUpService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async  Task<TopUpResponse> GetTopUpById(long Id)
        {
            var topUp = await _unitOfWork.TopUpRepo.GetTopUpById(Id);
            if(topUp == null)
            {
                throw new ArgumentException("không tìm thấy đơn nạp tiền");
            }
            var res = _mapper.Map<TopUpResponse>(topUp);
            return res;
        }
    }
}
