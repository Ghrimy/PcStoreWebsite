using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Product;

public class ProductCreateDto
{
    [Required] public string? ProductName { get; set; }

    [Required] public string? ProductDescription { get; set; }

    [Required] public string? ProductImage { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal ProductPrice { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public decimal ProductDiscount { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than 0")]
    public int ProductStock { get; set; }

    [Required] public ProductCategory ProductCategory { get; set; }

    [Required] public string? ProductBrand { get; set; }
}