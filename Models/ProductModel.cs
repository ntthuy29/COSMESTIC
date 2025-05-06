namespace COSMESTIC.Models
{
    public class ProductModel
    {
        public string productName { get; set; }
        public string productDescription { get; set; }
        public decimal price { get; set; }

        public string imagePath { get; set; }//upload file ảnh chứ nỏ phải url
       
    }
}
