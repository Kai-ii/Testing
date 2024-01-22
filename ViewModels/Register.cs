using System.ComponentModel.DataAnnotations;

namespace ASassignment.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "NRIC is required")]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        //[RegularExpression(@"^\w+[\+\. \w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$", ErrorMessage = "Password must be minimum 12 character and have lowercase, uppercase, numbers, and special characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd:MM:yyyy}")]
        [CustomValidation(typeof(Register), "ValidateDateOfBirth")]
        public DateTime DOB { get; set; }
        public static ValidationResult ValidateDateOfBirth(DateTime dob, ValidationContext context)
        {
            if (dob > DateTime.Now)
            {
                return new ValidationResult("Date of Birth cannot be a future date.");
            }
            return ValidationResult.Success;
        }

        [Required(ErrorMessage = "Resume is required")]
        [DataType(DataType.Upload)]
        public IFormFile Resume { get; set; }

        [Required(ErrorMessage = "Who Am I is required")]
        [RegularExpression(@"^[\s\S]*$", ErrorMessage = "Invalid characters")]
        public string Who { get; set; }
    }
}
