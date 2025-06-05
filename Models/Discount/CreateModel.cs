using System.ComponentModel.DataAnnotations;

namespace COSMESTIC.Models.Discount
{
    public class CreateModel
    {
        public int? discountID { get; set; }

        [Required(ErrorMessage = "Tên mã khuyến mãi không được để trống")]
        [StringLength(100, ErrorMessage = "Tên mã khuyến mãi không được vượt quá 100 ký tự")]
        public string name { get; set; }

        [Required(ErrorMessage = "Giá trị không được để trống")]
        [Range(0.01, 100, ErrorMessage = "Giá trị phải nằm trong khoảng từ 0.01 đến 100%")]
        public decimal value { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.DateTime)]
        public DateTime startDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.DateTime)]
        public DateTime endDate { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public bool isActive { get; set; }
    }
}
