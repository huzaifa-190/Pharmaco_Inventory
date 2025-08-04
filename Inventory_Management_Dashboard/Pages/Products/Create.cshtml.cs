using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_Dashboard.Pages.Products
{
    public class CreateModel : PageModel
    {
        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

        public CreateModel(Inventory_Management_Dashboard.Data.AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Product Product { get; set; }

        public SelectList CategoryOptions { get; set; }
        public SelectList SupplierOptions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Fetch categories and suppliers from the database
            CategoryOptions = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
            SupplierOptions = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Email");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // If form is invalid, reload the select lists so the form doesn't break
                CategoryOptions = new SelectList(await _context.Categories.ToListAsync(), "CategoryId", "Name");
                SupplierOptions = new SelectList(await _context.Suppliers.ToListAsync(), "SupplierId", "Email");

                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }




        //public IActionResult OnGet()
        //{
        //ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
        //ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Email");
        //    return Page();
        //}

        //[BindProperty]
        //public Product Product { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        //public async Task<IActionResult> OnPostAsync()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Page();
        //    }

        //    _context.Products.Add(Product);
        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("./Index");
        //}
    }
}
