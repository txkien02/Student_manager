namespace Student_manager.Models.DTO
{
    public class BasicInfoModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }
        public byte[]? Avatar { get; set; }
        public int ClassID { get; set; }
    }
}
