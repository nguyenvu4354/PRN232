using ShoppingWeb.DTOs.Common;
using ShoppingWeb.DTOs.Product;
using ShoppingWeb.DTOs.Promotion;

namespace ShoppingWeb.Services.Interface
{
    public interface IPromotionService
    {
        Task<PagedResultDTO<PromotionResponseDTO>> GetPagedAsync(int page, int pageSize);
        Task<PromotionResponseDTO> GetByIdAsync(int id);
        Task CreateAsync(PromotionRequestDTO request);
        Task UpdateAsync(int id, PromotionRequestDTO request);
        Task DeleteAsync(int id);
        Task AddProductsToPromotionAsync(ProductPromotionRequestDTO request);
        Task<List<ProductResponseDTO>> GetProductsByPromotionIdAsync(int promotionId);
        
        Task<PromotionStatusDTO> UpdateStatusAsync(int promotionId, bool isActive);


    }
}
