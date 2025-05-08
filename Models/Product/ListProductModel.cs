using System.ComponentModel.DataAnnotations.Schema;

namespace COSMESTIC.Models.Product
{
    public class ListProductModel
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public decimal price { get; set; }
        public string imagePath { get; set; }//upload file ảnh chứ nỏ phải url
        public string catalog { get; set; }// đổi từ int idcatalog
    }
}
