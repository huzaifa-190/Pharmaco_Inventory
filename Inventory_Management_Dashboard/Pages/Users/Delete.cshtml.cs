using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Users
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "No user ID provided.";
                return RedirectToPage("./Index");
            }

            User = await _context.Users.FindAsync(id);

            if (User == null)
            {
                TempData["ErrorMessage"] = $"User with ID {id} not found.";
                return RedirectToPage("./Index");
            }

            if (!User.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                TempData["ErrorMessage"] = $"User with ID {id} is not an Admin.";
                return RedirectToPage("./Index");
            }

            Console.WriteLine($"[DEBUG] Loaded user for delete: {User.FullName}, Role: {User.Role}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User == null || User.UserId == 0)
            {
                TempData["ErrorMessage"] = "Invalid user data for deletion.";
                return RedirectToPage("./Index");
            }

            var userToDelete = await _context.Users.FindAsync(User.UserId);

            if (userToDelete == null)
            {
                TempData["ErrorMessage"] = $"User with ID {User.UserId} not found.";
                return RedirectToPage("./Index");
            }

            if (!userToDelete.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
            {
                TempData["ErrorMessage"] = $"User with ID {User.UserId} is not an Admin.";
                return RedirectToPage("./Index");
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Admin '{userToDelete.FullName}' deleted successfully.";
            return RedirectToPage("./Index");
        }
    }
}
