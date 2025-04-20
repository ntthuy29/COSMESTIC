using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int discountID { get; set; }
        public decimal value { get; set; }
        public decimal discountAmount { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string discountType { get; set; }
        public bool isActive { get; set; }
        // Navigation property
        public virtual ICollection<Orders> Orders
        {
            get; set;
        }
    }
}
