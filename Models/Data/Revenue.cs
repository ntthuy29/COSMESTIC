using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Revenue")]
    public class Revenue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int revenueID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal totalRevenue { get; set; }
        public decimal totalProduct { get; set; }
        public string note { get; set; }
        [ForeignKey("orderID")]
        public int orderID { get; set; }
        public virtual Orders orders { get; set; }
    }
}
