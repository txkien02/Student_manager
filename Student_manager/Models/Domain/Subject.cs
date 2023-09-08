using System.ComponentModel.DataAnnotations;

namespace Student_manager.Models.Domain
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int TotalSubject { get; set; }
        public int MaxTotalSubject { get; set; }
    }
}
