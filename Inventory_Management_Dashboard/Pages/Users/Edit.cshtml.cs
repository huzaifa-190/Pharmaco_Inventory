using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_Dashboard.Data;
using Inventory_Management_Dashboard.Models;

namespace Inventory_Management_Dashboard.Pages.Users
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
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
            if (!ModelState.IsValid) return Page();

            var existingUser = await _context.Users.FindAsync(User.UserId);
            if (existingUser == null || existingUser.Role != "Admin") return NotFound();

            existingUser.FullName = User.FullName;
            existingUser.Email = User.Email;
            existingUser.Password = User.Password;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
