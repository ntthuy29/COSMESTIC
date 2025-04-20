using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        [Key]
        public int cartID { get; set; }
        [ForeignKey("userID")]
        public int userID { get; set; }
        public int totalQuantity { get; set; }
        public decimal totalPrice { get; set; }
        public virtual Users users { get; set; }
        public virtual ICollection<CartItem> cartItems { get; set; }
    }
}
