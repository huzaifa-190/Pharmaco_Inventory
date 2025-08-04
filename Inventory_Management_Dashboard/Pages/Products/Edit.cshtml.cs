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

namespace Inventory_Management_Dashboard.Pages.Products
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        // Dropdown lists for Category and Supplier
        public SelectList CategoryOptions { get; set; }
        public SelectList SupplierOptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Product = await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Supplier)
                        .FirstOrDefaultAsync(m => m.ProductId == id);

            if (Product == null)
                return NotFound();

            // Populate dropdown options
            CategoryOptions = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            SupplierOptions = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Email");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns when form is invalid
                CategoryOptions = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
                SupplierOptions = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Email");
                return Page();
            }

            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.ProductId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
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

//namespace Inventory_Management_Dashboard.Pages.Products
//{
//    public class EditModel : PageModel
//    {
//        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

//        public EditModel(Inventory_Management_Dashboard.Data.AppDbContext context)
//        {
//            _context = context;
//        }

//        [BindProperty]
//        public Product Product { get; set; } = default!;

//        public async Task<IActionResult> OnGetAsync(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var product =  await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
//            if (product == null)
//            {
//                return NotFound();
//            }
//            Product = product;
//           ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
//           ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Email");
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

//            _context.Attach(Product).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!ProductExists(Product.ProductId))
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

//        private bool ProductExists(int id)
//        {
//            return _context.Products.Any(e => e.ProductId == id);
//        }
//    }
//}
