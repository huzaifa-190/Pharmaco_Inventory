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
            if (id == null) return NotFound();

            User = await _context.Users.FindAsync(id);
            if (User == null || User.Role != "Admin") return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userToDelete = await _context.Users.FindAsync(User.UserId);
            if (userToDelete == null || userToDelete.Role != "Admin") return NotFound();

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
