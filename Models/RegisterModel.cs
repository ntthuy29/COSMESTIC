using System.ComponentModel.DataAnnotations;

namespace COSMESTIC.Models
{
    public class RegisterModel
    {
        [Required]
        public string fullName { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Phone]
        public string phoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime dateOfBirth { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
