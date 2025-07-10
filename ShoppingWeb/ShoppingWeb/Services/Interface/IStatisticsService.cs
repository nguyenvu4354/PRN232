using ShoppingWeb.DTOs.Statistics;

namespace ShoppingWeb.Services.Interface
{
    public interface IStatisticsService
    {
        Task<StatisticsResponseDTO> GetSummaryStatisticsAsync();
        Task<List<WeeklyProductSalesDTO>> GetWeeklyProductSalesAsync();

    }
}
