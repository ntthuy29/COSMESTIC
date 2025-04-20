using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Data
{
    [Table("Account")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int accountID { get; set; }
     [Required]
     public string username { get; set; }
        [Required]
       public string password { get; set; }
        [ForeignKey("userID")]
        public int userID { get; set; }
        public virtual Users user { get; set; }

    }
}
