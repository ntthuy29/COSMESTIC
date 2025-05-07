using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("CartItem")]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cartItemID { get; set; }
        [ForeignKey("cartID")]
        public int cartID { get; set; }
        [ForeignKey("productID")]
        public int productID { get; set; }
        public int quantity { get; set; }
        public decimal unitprice { get; set; }
        public virtual ShoppingCart cart { get; set; }
        public virtual Products products { get; set; }
    }

}
