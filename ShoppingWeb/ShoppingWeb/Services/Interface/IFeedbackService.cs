using ShoppingWeb.DTOs.Feedback;

namespace ShoppingWeb.Services.Interface
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackResponseDTO>> GetAllAsync();
        Task<FeedbackResponseDTO> GetByIdAsync(int id);
        Task<FeedbackResponseDTO> CreateAsync(int userId, FeedbackRequestDTO dto);
        Task UpdateStatusAsync(int id, string newStatus);
    }
}
