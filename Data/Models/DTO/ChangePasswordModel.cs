using System.ComponentModel.DataAnnotations;

namespace Data.Models.DTO
{
    public class ChangePasswordModel
    {
        
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
