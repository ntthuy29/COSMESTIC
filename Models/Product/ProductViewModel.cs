using Microsoft.AspNetCore.Mvc.Rendering;

namespace COSMESTIC.Models.Product
{
    public class ProductViewModel
    {
        public string TuKhoaTimKiem { get; set; }
        public string DanhMucDuocChon { get; set; }
        public List<ListProductModel> SanPhams { get; set; }
        public List<SelectListItem> DanhMucs { get; set; }
    
        // Thuộc tính phân trang
        public int CurrentPage { get; set; } = 1; // Trang hiện tại
        public int PageSize { get; set; } = 5; // Số sản phẩm mỗi trang
        public int TotalItems { get; set; } // Tổng số sản phẩm
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize); // Tổng số trang
    }
}
