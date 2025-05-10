using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace COSMESTIC.Models.Data
{
    [Table("Order")]
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orderID { get; set; }
        [Required]
        [ForeignKey("userID")]
        public int userID { get; set; }
        public decimal totalAmount { get; set; }
        public DateTime orderDate {get;set;}


        public DateTime? endDate { get; set; }


        public string status { get; set; }
        public virtual Users users { get; set; }
        public virtual Invoice invoice { get; set; }
        public virtual ICollection<orderDetail> orderDetails { get; set; }
        public virtual ICollection <Revenue> revenues { get; set; }
        // Thêm thuộc tính DeliveryIFMT để lưu thông tin giao hàng'
        public int? DeliveryID { get; set; }

        public virtual DeliveryIFMT Delivery { get; set; } // Liên kết với bảng DeliveryIFMT

    }
}
