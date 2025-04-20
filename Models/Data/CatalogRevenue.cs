using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("CatalogRevenue")]
    public class CatalogRevenue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int catalogRevenueID { get; set; }
        [ForeignKey("revenueID")]
        public int revenueID { get; set; }
        [ForeignKey("catalogID")]
        public int catalogID { get; set; }
       public virtual Catalogs catalog { get; set; } 
        public virtual Revenue revenue { get; set; }
    }
}
