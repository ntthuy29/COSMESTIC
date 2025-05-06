using System.ComponentModel.DataAnnotations;

namespace COSMESTIC.Models.Discount
{
    public class CreateModel
    {
        [Required]
        public int discountID { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public decimal value { get; set; }
        [Required]
        public DateTime startDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        [Required]
        public bool isActive { get; set; }
    }
}
