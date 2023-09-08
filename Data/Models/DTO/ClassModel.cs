using System.ComponentModel.DataAnnotations;

namespace Data.Models.DTO
{
    public class ClassModel
    {
        [Required]
        public string Name { get; set; }
    }
}
