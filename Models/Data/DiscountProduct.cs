using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("DiscountProduct")]
    public class DiscountProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int discountProductID { get; set; }
        [ForeignKey("discountID")]
        public int discountID { get; set; }
        [ForeignKey("productID")]
        public int productID { get; set; }
        public virtual Discount discount { get; set; }
        public virtual Products products { get; set; }
    }
}
