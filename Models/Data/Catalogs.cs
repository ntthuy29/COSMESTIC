using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Catalog")]
    public class Catalogs
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int catalogID { get; set; }
        public string catalogName { get; set; }
        public string catalogDescription { get; set; }
        public ICollection<Products> products { get; set; }
        public ICollection<CatalogRevenue> catalogRevenues { get; set; }
    }
}
