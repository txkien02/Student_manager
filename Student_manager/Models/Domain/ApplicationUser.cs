using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Student_manager.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        
        public string Name { get; set; }     
        
        public DateTime DOB { get; set; }

        public int Gender { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }

        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[]? Avatar { get; set; } // Sử dụng byte[] để lưu trữ hình ảnh
        
        public int ClassID { get; set; }

    }
}
