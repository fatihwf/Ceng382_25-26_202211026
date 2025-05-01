using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Oturumu temizle
            HttpContext.Session.Clear();

            // İlgili çerezleri sil
            if (Request.Cookies.ContainsKey("username"))
            {
                Response.Cookies.Delete("username");
            }
            if (Request.Cookies.ContainsKey("token"))
            {
                Response.Cookies.Delete("token");
            }
            if (Request.Cookies.ContainsKey("session_id"))
            {
                Response.Cookies.Delete("session_id");
            }

            // Giriş sayfasına yönlendir
            return RedirectToPage("/Index");
        }
    }
}
