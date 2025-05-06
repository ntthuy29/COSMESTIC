using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Product")]
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int productID { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string imagePath { get; set; }//upload file ảnh chứ nỏ phải url
        [ForeignKey("catalogID")]
        public int catalogID { get; set; }
        public virtual Catalogs catalog { get; set; }
        public virtual ICollection<orderDetail> orderDetails { get; set; }
        public virtual ICollection <CartItem> cartItems { get; set; }
        public virtual ICollection <DiscountProduct> discountProducts { get; set; }


    }
}
