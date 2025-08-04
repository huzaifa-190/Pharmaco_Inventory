using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management_Dashboard.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<string> SalesLabels { get; set; } = new();
        public List<decimal> SalesValues { get; set; } = new();

        public List<string> StockProductNames { get; set; } = new();
        public List<int> StockQuantities { get; set; } = new();

        public List<decimal> ProfitSummaryValues { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Load daily sales data initially
            var dailyData = await _context.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            SalesLabels = dailyData.Select(d => d.Date.ToShortDateString()).ToList();
            SalesValues = dailyData.Select(d => d.Revenue).ToList();

            // Stock Levels
            var stockData = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();

            StockProductNames = stockData.Select(p => p.Name).ToList();
            StockQuantities = stockData.Select(p => p.StockQuantity).ToList();

            // Profit Summary
            decimal totalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice);
            decimal totalCost = await _context.Products.SumAsync(p => p.UnitPrice * p.StockQuantity);
            decimal totalProfit = totalRevenue - totalCost;

            ProfitSummaryValues = new List<decimal> { totalRevenue, totalCost, totalProfit };
        }

        // API endpoint to fetch sales data dynamically based on period
        public async Task<IActionResult> OnGetGetSalesDataAsync(string period)
        {
            if (string.IsNullOrEmpty(period))
            {
                period = "daily";
            }

            var dailyData = await _context.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            List<string> labels;
            List<decimal> values;

            switch (period.ToLower())
            {
                case "weekly":
                    var weeklyGroups = dailyData
                        .GroupBy(d =>
                        {
                            var cal = CultureInfo.CurrentCulture.Calendar;
                            var dfi = DateTimeFormatInfo.CurrentInfo;
                            var week = cal.GetWeekOfYear(d.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                            return new { d.Date.Year, Week = week };
                        })
                        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Week);

                    labels = weeklyGroups
                        .Select(g => $"W{g.Key.Week} {g.Key.Year}")
                        .ToList();

                    values = weeklyGroups
                        .Select(g => g.Sum(x => x.Revenue))
                        .ToList();
                    break;

                case "monthly":
                    var monthlyGroups = dailyData
                        .GroupBy(d => new { d.Date.Year, d.Date.Month })
                        .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                    labels = monthlyGroups
                        .Select(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}")
                        .ToList();

                    values = monthlyGroups
                        .Select(g => g.Sum(x => x.Revenue))
                        .ToList();
                    break;

                case "daily":
                default:
                    labels = dailyData.Select(d => d.Date.ToShortDateString()).ToList();
                    values = dailyData.Select(d => d.Revenue).ToList();
                    break;
            }

            return new JsonResult(new { labels, values });
        }
    }
}
