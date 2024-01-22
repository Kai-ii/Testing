using Microsoft.AspNetCore.Identity;

namespace ASassignment.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Confirmpassword { get; set; }
        public DateTime DOB { get; set; }
        public string Resume { get; set; }
        public string Who { get; set; }

    }
}
