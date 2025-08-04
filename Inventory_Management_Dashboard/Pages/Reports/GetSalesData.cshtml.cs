using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management_Dashboard.Pages.Reports
{
    public class GetSalesDataModel : PageModel
    {
        private readonly AppDbContext _context;

        public GetSalesDataModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JsonResult> OnGetAsync(string period)
        {
            var query = _context.Orders.AsQueryable();
            var labels = new List<string>();
            var values = new List<decimal>();

            if (period == "Daily")
            {
                var dailyData = await query
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new { Date = g.Key.ToShortDateString(), Total = g.Sum(o => o.TotalPrice) })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                labels = dailyData.Select(x => x.Date).ToList();
                values = dailyData.Select(x => x.Total).ToList();
            }
            else if (period == "Weekly")
            {
                var weeklyData = await query
                    .GroupBy(o => EF.Functions.DateDiffWeek(DateTime.MinValue, o.OrderDate))
                    .Select(g => new { Week = "Week " + g.Key, Total = g.Sum(o => o.TotalPrice) })
                    .OrderBy(x => x.Week)
                    .ToListAsync();

                labels = weeklyData.Select(x => x.Week).ToList();
                values = weeklyData.Select(x => x.Total).ToList();
            }
            else if (period == "Monthly")
            {
                var monthlyData = await query
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month:00}", Total = g.Sum(o => o.TotalPrice) })
                    .OrderBy(x => x.Month)
                    .ToListAsync();

                labels = monthlyData.Select(x => x.Month).ToList();
                values = monthlyData.Select(x => x.Total).ToList();
            }

            return new JsonResult(new { labels, values });
        }
    }
}
