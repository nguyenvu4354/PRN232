using Microsoft.EntityFrameworkCore;
using ShoppingWeb.DTOs.Promotion;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly ShoppingWebContext _context;
        private readonly ILogger<PromotionService> _logger;

        public PromotionService(ShoppingWebContext context, ILogger<PromotionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<PromotionResponseDTO>> GetAllAsync()
        {
            return await _context.Promotions.Select(p => new PromotionResponseDTO
            {
                PromotionId = p.PromotionId,
                Title = p.Title,
                Description = p.Description,
                DiscountPercentage = p.DiscountPercentage,
                DiscountAmount = p.DiscountAmount,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = p.IsActive
            }).ToListAsync();
        }

        public async Task<PromotionResponseDTO> GetByIdAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                throw new NotFoundException("Promotion not found");

            return new PromotionResponseDTO
            {
                PromotionId = promotion.PromotionId,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercentage = promotion.DiscountPercentage,
                DiscountAmount = promotion.DiscountAmount,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive
            };
        }

        public async Task CreateAsync(PromotionRequestDTO request)
        {
            var promotion = new Promotion
            {
                Title = request.Title,
                Description = request.Description,
                DiscountPercentage = request.DiscountPercentage,
                DiscountAmount = request.DiscountAmount,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, PromotionRequestDTO request)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                throw new NotFoundException("Promotion not found");

            promotion.Title = request.Title;
            promotion.Description = request.Description;
            promotion.DiscountPercentage = request.DiscountPercentage;
            promotion.DiscountAmount = request.DiscountAmount;
            promotion.StartDate = request.StartDate;
            promotion.EndDate = request.EndDate;
            promotion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
                throw new NotFoundException("Promotion not found");

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }
    }
}