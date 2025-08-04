using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;
using Inventory_Management_Dashboard.Helpers;

namespace Inventory_Management_Dashboard.Pages.Products
{
    public class IndexModel : AuthenticatedPageModel
    {
        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

        public IndexModel(Inventory_Management_Dashboard.Data.AppDbContext context)
        {
            _context = context;
        }
        
        public IList<Product> Product { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var adminCheck = CheckAdminLoggedIn();
            if (adminCheck != null) return adminCheck;

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier).ToListAsync();

            return Page();

        }
    }
}
