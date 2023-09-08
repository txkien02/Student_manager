using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Student_manager.Models.Domain
{
    public class SubjectInfo
    {
        [Key]
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public string Studentid { get;set; }


    }
}
