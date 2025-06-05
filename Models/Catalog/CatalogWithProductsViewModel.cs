using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CatalogWithProductsViewModel
{
    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    public int catalogID { get; set; }
    [Display(Name = "Tên danh mục")]
    public string catalogName { get; set; }

    [Display(Name = "Mô tả danh mục")]
    public string catalogDescription { get; set; }

    [Display(Name = "Danh sách sản phẩm")]
    public List<ProductItemViewModel> Products { get; set; } = new List<ProductItemViewModel>();
}

public class ProductItemViewModel
{
    public int productID { get; set; }
    [Required]
    public string productName { get; set; }

    public string productDescription { get; set; }

    [Range(0.01, 1000000)]
    public decimal price { get; set; }
    public int quantity { get; set; }

    [Required]
    public IFormFile imageFile { get; set; }
    public string imagePath
    {
        get; set;
    }
}