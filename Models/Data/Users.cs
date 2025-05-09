

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("User")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userID { get; set; }

        public string fullName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string role { get;set; }
        public string status { get; set; }
        public  virtual Account account { get; set; }
        public virtual ICollection< DeliveryIFMT >deliverys { get; set; }
        public virtual ICollection<Orders> orders { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual ICollection<ProductReView> ProductReviews { get; set; }
    }
}
