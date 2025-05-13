using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("DeliveryIFMT")]
    public class DeliveryIFMT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int deliveryID { get; set; }
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống.")]
        public string deliveryAddress { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string deliveryPhone { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string deliveryName { get; set; }

        public int userID { get; set; }
        public virtual Users user { get; set; }
    }
}
