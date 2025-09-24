using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Product;

public class ProductUpdateDto
{
    public string? ProductName { get; set; }

    public string? ProductDescription { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? ProductImage { get; set; }
        
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? ProductPrice { get; set; }
        
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than 0")]
    public int? ProductStock { get; set; }

    public ProductCategory? ProductCategory { get; set; }

    public string? ProductBrand { get; set; }
}