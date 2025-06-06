using AutoMapper;
using Azure.Core;
using Repository.IRepository;
using Repository.Models;
using Repository.Models.DTOs.Request;
using Repository.UnitOfWork;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderSlotLimitService : IOrderSlotLimitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderSlotLimitService(IUnitOfWork unitOfWork, IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderSlotLimit> AddOrderSlotLimit(OrderSlotLimitRequest OrderSlotLimit)
        {
            var orderSlot = _mapper.Map<OrderSlotLimit>( OrderSlotLimit );
            await _unitOfWork.OrderSlotLimitRepo.AddAsync( orderSlot );
            await _unitOfWork.SaveAsync();
            return orderSlot;
        }

        public async Task DeleteOrderSlotLimit(Guid Id)
        {
            var order = await GetOrderSlotLimitById(Id);
            await _unitOfWork.OrderSlotLimitRepo.DeleteAsync(Id);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<OrderSlotLimit>> GetOrderSlotLimit()
        {
            var orderSlots = await _unitOfWork.OrderSlotLimitRepo.GetAllAsync();
            return orderSlots;
        }

        public async Task<OrderSlotLimit> GetOrderSlotLimitById(Guid Id)
        {
            var orderSlot = await _unitOfWork.OrderSlotLimitRepo.GetByIdAsync(Id);
            if (orderSlot == null)
            {
                throw new Exception("không tìm thấy orderslot");
            }
            return orderSlot;
        }

        public async Task<OrderSlotLimit> UpdateOrderSlotLimit(Guid Id, OrderSlotLimitRequest request)
        {
            var existinOrderSlot = await GetOrderSlotLimitById(Id);
            _mapper.Map(request, existinOrderSlot);
            await _unitOfWork.OrderSlotLimitRepo.UpdateAsync(existinOrderSlot);
            await _unitOfWork.SaveAsync();
            return existinOrderSlot;
        }
    }
    }
      