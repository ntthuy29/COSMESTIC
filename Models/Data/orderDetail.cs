using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("OrderDetail")]
    public class orderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int orderDetailID { get; set; }
        [ForeignKey("orderID")]
        public int orderID { get; set; }
        [ForeignKey("productID")]
        public int productID { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public virtual Orders orders { get; set; }
        public virtual Products products { get; set; }
    }
}
