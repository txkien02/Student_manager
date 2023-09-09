using System.ComponentModel.DataAnnotations;

namespace Data.Models.DTO
{
    public class ClassModel
    {
        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

       
        public int TotalStudents { get; set; }
    }
}
