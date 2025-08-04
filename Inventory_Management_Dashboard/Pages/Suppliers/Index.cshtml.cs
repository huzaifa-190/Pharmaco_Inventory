using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Suppliers
{
    public class IndexModel : PageModel
    {
        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

        public IndexModel(Inventory_Management_Dashboard.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Supplier> Supplier { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Supplier = await _context.Suppliers.ToListAsync();
        }
    }
}
