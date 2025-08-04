using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_Dashboard.Pages.Orders
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        public SelectList ProductList { get; set; } = default!;
        public SelectList UserList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            Order = await _context.Orders
                .Include(o => o.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (Order == null) return NotFound();

            ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
            UserList = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
                UserList = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");
                return Page();
            }

            // Fetch latest product price for selected ProductId
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == Order.ProductId);

            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Selected product not found.");
                ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "Name");
                UserList = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");
                return Page();
            }

            // Recalculate TotalPrice = Quantity * UnitPrice
            Order.TotalPrice = Order.Quantity * product.UnitPrice;

            _context.Attach(Order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(Order.OrderId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Inventory_Management_Dashboard.Data;
//using Inventory_Management_Dashboard.Models;

//namespace Inventory_Management_Dashboard.Pages.Orders
//{
//    public class EditModel : PageModel
//    {
//        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

//        public EditModel(Inventory_Management_Dashboard.Data.AppDbContext context)
//        {
//            _context = context;
//        }

//        [BindProperty]
//        public Order Order { get; set; } = default!;

//        public async Task<IActionResult> OnGetAsync(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var order =  await _context.Orders.FirstOrDefaultAsync(m => m.OrderId == id);
//            if (order == null)
//            {
//                return NotFound();
//            }
//            Order = order;
//           ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "Name");
//           ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
//            return Page();
//        }

//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more information, see https://aka.ms/RazorPagesCRUD.
//        public async Task<IActionResult> OnPostAsync()
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

//            _context.Attach(Order).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!OrderExists(Order.OrderId))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return RedirectToPage("./Index");
//        }

//        private bool OrderExists(int id)
//        {
//            return _context.Orders.Any(e => e.OrderId == id);
//        }
//    }
//}
