using AutoMapper;
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
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepo _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public PromotionService(IPromotionRepo repo, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Promotion> AddPromotion(PromotionRequest Request)
        {
            var promotion = _mapper.Map<Promotion>(Request);
            promotion.CreateDate = DateTime.Now;
            await _repo.Add(promotion);
            return promotion;
           
        }

        public async Task DeletePromotion(Guid Id)
        {
           await GetPromotionById(Id);
           await _repo.DeleteAsync(Id);
        }

        public async Task<IEnumerable<Promotion>> GetPromotion()
        {
            var promotions = await _repo.GetAllAsync();
            return promotions;
        }

        public async Task<Promotion> GetPromotionByCode(Guid customerId, string code)
        {
            var promotion = await _repo.GetPromotionByCode(code);

            // check limit 
            var limit = await _unitOfWork.PromotionUsedRepo.CountPromotionUsedAsync(promotion.Id);
            if(promotion.Limit == limit)
            {
                throw new ArgumentException("Khuyến mãi này đã được sử dụng hết");
            }

            // check used
            var checkExist = await _unitOfWork.PromotionUsedRepo.CheckExsit (promotion.Id, customerId);
            if(checkExist)
            {
                throw new ArgumentException("Bạn đã sử dụng mã khuyến mãi này rồi");
            }


            if (promotion == null)
            {
                throw new ArgumentException("Không tìm thấy promotion");
            }
            return promotion;
        }

        public async Task<Promotion> GetPromotionById(Guid Id)
        {
            var promotion = await _repo.GetByIdAsync(Id);
            if(promotion == null)
            {
                throw new Exception("không tìm thấy promotion");
            }
            return promotion;
        }

        public async Task<Promotion> UpdatePromotion(Guid Id, PromotionRequest Request)
        {
            var existingPromotion = await GetPromotionById(Id);
            
            existingPromotion.Name = Request.Name;
            existingPromotion.Condition = Request.Condition;
            existingPromotion.DiscountPercent = Request.DiscountPercent;
            existingPromotion.StartDate = Request.StartDate;
            existingPromotion.EndDate = Request.EndDate;
            existingPromotion.UpdateDate = DateTime.Now;

            await _repo.Update(existingPromotion);
            return existingPromotion;
        }
    }
}
