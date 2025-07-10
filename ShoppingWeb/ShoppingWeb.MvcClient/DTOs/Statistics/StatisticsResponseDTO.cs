namespace ShoppingWeb.MvcClient.DTOs.Statistics
{
    public class StatisticsResponseDTO
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProductsSold { get; set; }
        public string BestSellingProduct { get; set; } = string.Empty;
        public int TotalUsers { get; set; }
    }
}
