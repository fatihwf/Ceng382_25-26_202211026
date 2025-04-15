using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using App.Models;

namespace App.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Enter username and password";
                return Page();
            }

            // users.json dosyasının yolunu belirle
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");

            if (!System.IO.File.Exists(filePath))
            {
                ErrorMessage = "User file not found.";
                return Page();
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
            List<User> users = JsonSerializer.Deserialize<List<User>>(jsonData);

            // Girilen bilgilerin uygun, aktif bir kullanıcı ile eşleşip eşleşmediğini kontrol et
            var user = users.FirstOrDefault(u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)
                                    && u.Password == Password && u.IsActive);

            if(user == null)
            {
                ErrorMessage = "Username or password is wrong.";
                return Page();
            }

            // Başarılı giriş durumunda; basit bir token üret (örneğin Guid kullanarak)
            string token = Guid.NewGuid().ToString();

            // Oturuma verileri depola
            HttpContext.Session.SetString("username", Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // Çerez için seçenekler (30 dakika geçerlilik, HttpOnly, Secure, SameSite.Strict)
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            // Başarılı giriş sonrası, önceki hafta oluşturduğunuz tablo sayfasına yönlendir (Örneğin, /Table)
            return RedirectToPage("/Table");
        }
    }
}
