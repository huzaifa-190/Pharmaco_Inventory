using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventory_Management_Dashboard.Helpers
{
    public class AuthenticatedPageModel : PageModel
    {
        // Check if user is logged in (session check)
        public IActionResult CheckUserLoggedIn()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToPage("/Login/Index");
            }
            return null;
        }

        // Optional: Check if admin
        public IActionResult CheckAdminLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || !string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToPage("/Login/Index");
            }
            return null;
        }
    }
   }
