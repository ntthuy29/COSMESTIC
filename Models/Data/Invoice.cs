using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        public int invoiceID { get; set; }
        [ForeignKey("orderID")]
        public int orderID { get; set; }
        public virtual Orders orders { get; set; }
    }
}
