using Microsoft.EntityFrameworkCore;
using ShoppingWeb.DTOs.Statistics;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
using System.Globalization;

namespace ShoppingWeb.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ShoppingWebContext _context;

        public StatisticsService(ShoppingWebContext context)
        {
            _context = context;
        }

        public async Task<StatisticsResponseDTO> GetSummaryStatisticsAsync()
        {
            var totalOrders = await _context.Carts.CountAsync(c => !c.IsCart);
            var totalRevenue = await _context.Carts
                .Where(c => !c.IsCart)
                .SumAsync(c => c.TotalAmount);

            var totalProductsSold = await _context.OrderDetails
                .SumAsync(od => od.Quantity);

            var bestSellingProduct = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .OrderByDescending(g => g.Sum(x => x.Quantity))
                .Select(g => g.First().Product.ProductName)
                .FirstOrDefaultAsync() ?? "N/A";

            var totalUsers = await _context.Users.CountAsync();

            return new StatisticsResponseDTO
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProductsSold = totalProductsSold,
                BestSellingProduct = bestSellingProduct,
                TotalUsers = totalUsers
            };
        }
        public async Task<List<WeeklyProductSalesDTO>> GetWeeklyProductSalesAsync()
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.CreatedAt != null)
                .ToListAsync();

            var weeklySales = orderDetails
                .GroupBy(od =>
                {
                    var date = od.CreatedAt!.Value;
                    var calendar = CultureInfo.InvariantCulture.Calendar;
                    var week = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return $"{date.Year}-W{week:D2}";
                })
                .Select(g => new WeeklyProductSalesDTO
                {
                    Week = g.Key,
                    TotalProductsSold = g.Sum(x => x.Quantity)
                })
                .OrderBy(x => x.Week)
                .ToList();

            return weeklySales;
        }
    }
}