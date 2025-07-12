using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Data;
using ShoppingWeb.DTOs.Feedback;
using ShoppingWeb.Exceptions;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;

namespace ShoppingWeb.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ShoppingWebContext _context;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(ShoppingWebContext context, ILogger<FeedbackService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<FeedbackResponseDTO>> GetAllAsync()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(f => new FeedbackResponseDTO
            {
                FeedbackId = f.FeedbackId,
                UserId = f.UserId,
                Username = f.User.Username,
                Content = f.Content,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            });
        }

        public async Task<FeedbackResponseDTO> GetByIdAsync(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (feedback == null)
            {
                throw new NotFoundException("Feedback not found");
            }

            return new FeedbackResponseDTO
            {
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId,
                Username = feedback.User.Username,
                Content = feedback.Content,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt
            };
        }

        public async Task<FeedbackResponseDTO> CreateAsync(int userId, FeedbackRequestDTO dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var feedback = new Feedback
            {
                UserId = userId,
                Content = dto.Content,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return new FeedbackResponseDTO
            {
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId,
                Username = user.Username,
                Content = feedback.Content,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                UpdatedAt = feedback.UpdatedAt
            };
        }

        public async Task UpdateStatusAsync(int id, string newStatus)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                throw new NotFoundException("Feedback not found");
            }

            feedback.Status = newStatus;
            feedback.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
