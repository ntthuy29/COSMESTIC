namespace COSMESTIC.Models.Product
{
    public class DetailProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string CatalogName { get; set; }
        public int TotalQuantity { get; set; } // Số lượng tổng (giả sử nhập từ một nguồn khác)
        public int SoldQuantity { get; set; } // Số lượng đã bán (tính từ OrderDetails)
        public int RemainingQuantity { get; set; } // Số lượng còn lại (tính toán)
        public decimal Revenue { get; set; }
    }
}
