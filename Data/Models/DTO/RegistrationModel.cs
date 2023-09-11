using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.DTO
{
    public class RegistrationModel
    {

        
        public string? UserName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public int Gender { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public bool  Status { get; set; }
        
        public byte[]? Avatar { get; set; }
        
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Minimum length 6 and must contain  1 Uppercase,1 lowercase, 1 special character and 1 digit")]
        public string? Password { get; set; }
        
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }
        [Required]
        public int ClassID { get; set; }
    }
}
