using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    public class ProductReView
    {
        [Key]
        public int reviewID { get; set; }
        [ForeignKey("userID")]
        public int userID { get; set; }
        [ForeignKey("productID")]
        public int productID { get; set; }
        [Range(1, 5)]
        public int rating { get; set; }
        [StringLength(1000)]
        public string comment { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Users user { get; set; }
        public virtual Products product { get; set; }
    }
}
