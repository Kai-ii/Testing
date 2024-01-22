using ASassignment.Model;
using ASassignment.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace ASassignment.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] { ".pdf", ".docx" };
                var fileExtension = System.IO.Path.GetExtension(RModel.Resume.FileName);
                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("", "Invalid file format. Please upload .pdf or .docx.");
                    return Page();
                }

                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: RModel.Password.Trim()!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                var user = new ApplicationUser()
                {
                    UserName = RModel.Email.Trim(),
                    Email = RModel.Email.Trim(),
                    Firstname = RModel.FName.Trim(),
                    Lastname = RModel.FName.Trim(),
                    Gender = RModel.Gender,
                    NRIC = protector.Protect(RModel.NRIC).Trim(),
                    Password = hashed,
                    Confirmpassword = RModel.ConfirmPassword.Trim(),
                    DOB = RModel.DOB.Date,
                    Resume = RModel.Resume.FileName.Trim(),
                    Who = RModel.Who.Trim(),
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }
    }
}
