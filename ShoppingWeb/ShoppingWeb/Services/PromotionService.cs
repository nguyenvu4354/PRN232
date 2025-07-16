using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Product;
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

        public async Task<PagedResultDTO<PromotionResponseDTO>> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Promotions
                .OrderByDescending(p => p.StartDate);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PromotionResponseDTO
                {
                    PromotionId = p.PromotionId,
                    Title = p.Title,
                    Description = p.Description,
                    DiscountPercentage = p.DiscountPercentage,
                    DiscountAmount = p.DiscountAmount,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return new PagedResultDTO<PromotionResponseDTO>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
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
        public async Task AddProductsToPromotionAsync(ProductPromotionRequestDTO request)
        {
            var promotion = await _context.Promotions
                .Include(p => p.ProductPromotions)
                .FirstOrDefaultAsync(p => p.PromotionId == request.PromotionId);

            if (promotion == null)
                throw new NotFoundException("Promotion not found");

            foreach (var productId in request.ProductIds)
            {
                if (!promotion.ProductPromotions.Any(pp => pp.ProductId == productId))
                {
                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        var pp = new ProductPromotion
                        {
                            ProductId = productId,
                            PromotionId = request.PromotionId,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.ProductPromotions.Add(pp);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task<List<ProductResponseDTO>> GetProductsByPromotionIdAsync(int promotionId)
        {
            var products = await _context.ProductPromotions
                .Where(pp => pp.PromotionId == promotionId)
                .Select(pp => pp.Product)
                .ToListAsync();

            return products.Select(p => new ProductResponseDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList();
        }

        public async Task<PromotionStatusDTO> UpdateStatusAsync(int promotionId, bool isActive)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId);
            if (promotion == null)
                throw new NotFoundException($"Promotion with ID {promotionId} not found.");

            promotion.IsActive = isActive;
            promotion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new PromotionStatusDTO
            {
                IsActive = promotion.IsActive
            };
        }

    }
}