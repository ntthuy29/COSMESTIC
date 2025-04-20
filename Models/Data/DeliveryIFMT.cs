using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("DeliveryIFMT")]
    public class DeliveryIFMT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int deliveryID { get; set; }
       
        public string deliveryAddress { get; set; }
        public string deliveryPhone { get; set; }
        public string deliveryName { get; set; }

        public int userID { get; set; }
        public virtual Users user { get; set; }
    }
}
