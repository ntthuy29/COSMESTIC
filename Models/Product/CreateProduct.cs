using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace COSMESTIC.Models.Product
{
    public class CreateProduct
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int DanhMucDuocChon { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string productName { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả sản phẩm")]
        public string productDescription { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0.01, 1000000, ErrorMessage = "Giá phải từ 0.01 đến 1,000,000")]
        [Display(Name = "Giá")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        [Display(Name = "Hình ảnh")]
        public IFormFile imageFile { get; set; }

        [Display(Name = "Danh sách danh mục")]
        public List<SelectListItem> DanhMucs { get; set; } = new List<SelectListItem>();
    }
}

