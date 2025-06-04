using System.ComponentModel.DataAnnotations;

namespace COSMESTIC.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string fullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{9,11}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[@*]).{9,}$", ErrorMessage = "Mật khẩu phải từ 9 ký tự, có chữ in hoa và ký tự đặc biệt (@ hoặc *)")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
