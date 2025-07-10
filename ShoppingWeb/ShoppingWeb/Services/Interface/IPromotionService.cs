using ShoppingWeb.DTOs.Promotion;

namespace ShoppingWeb.Services.Interface
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionResponseDTO>> GetAllAsync();
        Task<PromotionResponseDTO> GetByIdAsync(int id);
        Task CreateAsync(PromotionRequestDTO request);
        Task UpdateAsync(int id, PromotionRequestDTO request);
        Task DeleteAsync(int id);
    }
}
