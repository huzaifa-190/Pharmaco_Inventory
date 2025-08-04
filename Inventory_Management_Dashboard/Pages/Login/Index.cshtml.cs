using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly Inventory_Management_Dashboard.Data.AppDbContext _context;

        public IndexModel(Inventory_Management_Dashboard.Data.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel LoginForm { get; set; } = new LoginViewModel();

        [TempData]
        public string? ErrorMessage { get; set; }
        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.Users.ToListAsync();
        }

     
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // First, check if a user with this email exists
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == LoginForm.Email);

            if (user == null)
            {
                TempData["ToastMessage"] = "No account found with this email.";
                TempData["ToastType"] = "error";
                return Page();
            }

            // Now check if the password matches
            if (user.Password != LoginForm.Password)
            {
                TempData["ToastMessage"] = "Incorrect password.";
                TempData["ToastType"] = "error";
                return Page();
            }

            // Check if the user is an admin
            if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ToastMessage"] = "You do not have access to this system.";
                TempData["ToastType"] = "error";
                return Page();
            }

            // Set session for logged in admin
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.FullName);

            //TempData["ToastMessage"] = "Login successful!";
            //TempData["ToastType"] = "success";

            return RedirectToPage("/Dashboard/Index");
        }




    }
}
