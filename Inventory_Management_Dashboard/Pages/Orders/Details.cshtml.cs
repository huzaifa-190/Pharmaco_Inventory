using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order = await _context.Orders
                .Include(o => o.Product)  
                .Include(o => o.User)  
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (Order == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
