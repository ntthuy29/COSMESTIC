using COSMESTIC.Models.Order;

namespace COSMESTIC.Models.User
{
    public class UserDetailViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? totalItems { get; set; }
        public decimal totalAmount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public List<OrderViewModel> Orders { get; set; }
    }
}
