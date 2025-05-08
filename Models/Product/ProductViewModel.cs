using Microsoft.AspNetCore.Mvc.Rendering;

namespace COSMESTIC.Models.Product
{
    public class ProductViewModel
    {
        public string TuKhoaTimKiem { get; set; }
        public string DanhMucDuocChon { get; set; }
        public List<ListProductModel> SanPhams { get; set; }
        public List<SelectListItem> DanhMucs { get; set; }
    }
}
