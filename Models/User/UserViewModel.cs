namespace COSMESTIC.Models.User
{
    public class UserViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; } // Định dạng chuỗi để hiển thị
        public string Role { get; set; }
        public string Status { get; set; }
    }
}
