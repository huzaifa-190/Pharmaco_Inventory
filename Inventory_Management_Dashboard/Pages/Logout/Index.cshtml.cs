using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Inventory_Management_Dashboard.Pages.Logout
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Clear all session data
            HttpContext.Session.Clear();

            // Optionally add a logout toast
            TempData["ToastMessage"] = "Logged out successfully!";
            TempData["ToastType"] = "success";

            // Redirect to login page
            return RedirectToPage("/Login/Index");
        }
    }
}
