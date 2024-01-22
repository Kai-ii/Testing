using ASassignment.Model;
using ASassignment.ViewModels;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ASassignment.Pages
{
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

        private readonly IHttpContextAccessor contxt;
        //private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly AuthDbContext authDbContext;
		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, AuthDbContext authDbContext)
        {
            this.signInManager = signInManager;
            contxt = httpContextAccessor;
            this.authDbContext = authDbContext;
        }

        public void OnGet()
		{
           
        }
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
                if (!await ValidateCaptcha())
                {
                    ModelState.AddModelError("", "Captcha verification failed");
                    return Page();
                }
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
					LModel.RememberMe, lockoutOnFailure: true);
				if (identityResult.Succeeded)
				{
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, LModel.Email),
                    };
                    var i = new ClaimsIdentity(claims, "MyCookieAuth");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    //return RedirectToPage("Index");
                    var user = await authDbContext.Users.FirstOrDefaultAsync(x => x.Email == LModel.Email);
                    if (user != null)
                    {
                        var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                        var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                        contxt.HttpContext.Session.SetString("a", user.Firstname);
                        contxt.HttpContext.Session.SetString("b", user.Lastname);
                        contxt.HttpContext.Session.SetString("c", user.Gender);
                        contxt.HttpContext.Session.SetString("d", protector.Unprotect(user.NRIC));
                        contxt.HttpContext.Session.SetString("e", user.Email);
                        contxt.HttpContext.Session.SetString("f", user.Password);
                        contxt.HttpContext.Session.SetString("g", user.Confirmpassword);
                        contxt.HttpContext.Session.SetString("h", user.DOB.ToString());
                        contxt.HttpContext.Session.SetString("i", user.Resume);
                        contxt.HttpContext.Session.SetString("j", user.Who);
                    }

                    return RedirectToPage("Index");
				}
                if (identityResult.IsLockedOut)
                {
                    return RedirectToPage("error/403");
                }
            }
			return Page();
		}
        private async Task<bool> ValidateCaptcha()
        {
            var captchaSecretKey = "6Ldcl1MpAAAAALZ9DIjH2gtB8uo7IITcA7rtLpvz";
            var captchaResponse = HttpContext.Request.Form["g-recaptcha-response"];
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={captchaSecretKey}&response={captchaResponse}");
                Console.WriteLine($"Captcha Response: {response}");

                var captchaResult = JsonSerializer.Deserialize<ReCaptchaResponse>(response);
                return Convert.ToBoolean(captchaResult.Success);
            }
        }
    }
}
