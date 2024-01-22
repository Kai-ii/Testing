using ASassignment.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ASassignment.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor contextAccessor;
        public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contextAccessor)
        {
            this.signInManager = signInManager;
            this.contextAccessor = contextAccessor;
        }
        public void OnGet() { }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            await signInManager.SignOutAsync();
            contextAccessor.HttpContext.Session.Clear();
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {
            return RedirectToPage("Index");
        }
    }
}
