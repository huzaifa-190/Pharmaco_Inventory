using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_Dashboard.Pages.Dashboard
{
    public class IndexModel : AuthenticatedPageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Properties for Dashboard Stats
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockCount { get; set; }
        public string TopProductName { get; set; } = "N/A";

        public List<string> SalesDates { get; set; } = new();
        public List<decimal> SalesTotals { get; set; } = new();
        public List<string> TopProductNames { get; set; } = new();
        public List<int> TopProductQuantities { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            //  to enforce Admin only
            var adminCheck = CheckAdminLoggedIn();
            if (adminCheck != null) return adminCheck;
            // Total Revenue
            TotalRevenue = await _context.Orders.SumAsync(o => o.TotalPrice);

            // Total Orders
            TotalOrders = await _context.Orders.CountAsync();

            // Low Stock Products
            LowStockCount = await _context.Products.CountAsync(p => p.StockQuantity < 10);

            // Top Product
            var topProduct = await _context.Orders
                .GroupBy(o => o.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(o => o.Quantity) })
                .OrderByDescending(g => g.Quantity)
                .FirstOrDefaultAsync();

            if (topProduct != null)
            {
                var product = await _context.Products.FindAsync(topProduct.ProductId);
                TopProductName = product?.Name ?? "N/A";
            }

            // Sales Trend (last 7 days)
            var startDate = DateTime.UtcNow.AddDays(-6).Date;
            var sales = await _context.Orders
                .Where(o => o.OrderDate >= startDate)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalPrice) })
                .OrderBy(g => g.Date)
                .ToListAsync();

            foreach (var day in Enumerable.Range(0, 7))
            {
                var date = startDate.AddDays(day);
                SalesDates.Add(date.ToString("MMM dd"));
                SalesTotals.Add(sales.FirstOrDefault(s => s.Date == date)?.Total ?? 0);
            }

            // Top 5 Selling Products
            var topProducts = await _context.Orders
                .GroupBy(o => o.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(o => o.Quantity) })
                .OrderByDescending(g => g.Quantity)
                .Take(5)
                .ToListAsync();

            foreach (var item in topProducts)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                TopProductNames.Add(product?.Name ?? "Unknown");
                TopProductQuantities.Add(item.Quantity);
            }
            return Page();
        }
    }
}



//using Inventory_Management_Dashboard.Helpers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;

//namespace Inventory_Management_Dashboard.Pages.Dashboard
//{
//    public class IndexModel : AuthenticatedPageModel
//    {
//        //public void OnGet()
//        //{
//        //}
//        public IActionResult OnGet()
//        {
//            // Centralized login check via base class
//            //var result = CheckUserLoggedIn();
//            //if (result != null) return result;

//             //  to enforce Admin only
//            var adminCheck = CheckAdminLoggedIn();
//            if (adminCheck != null) return adminCheck;

//            // your dashboard code here
//            return Page();
//        }

//    }
//}
