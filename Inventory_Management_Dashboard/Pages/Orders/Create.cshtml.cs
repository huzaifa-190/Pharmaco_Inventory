using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        public SelectList ProductList { get; set; } = default!;
        public SelectList UserList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch products and users for dropdowns
            var products = await _context.Products.ToListAsync();
            var users = await _context.Users.ToListAsync();

            ProductList = new SelectList(products, "ProductId", "Name");
            UserList = new SelectList(users, "UserId", "Email");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns if form validation fails
                ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
                UserList = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");

                return Page();
            }

            // Fetch product's unit price from DB using selected ProductId
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == Order.ProductId);

            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Selected product not found.");
                ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
                UserList = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");
                return Page();
            }

            // Calculate TotalPrice = Quantity * UnitPrice
            Order.TotalPrice = Order.Quantity * product.UnitPrice;

            // Save order
            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}





