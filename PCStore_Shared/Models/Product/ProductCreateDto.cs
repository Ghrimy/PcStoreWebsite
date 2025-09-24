using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Product;

public class ProductCreateDto
{
    [Required]
    public string? ProductName { get; set; }
    
    [Required]
    public string? ProductDescription { get; set; }
    
    [Required]
    [DataType(DataType.ImageUrl)]
    public string? ProductImage { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal ProductPrice { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Stock must be greater than 0")]
    public int ProductStock { get; set; }
    
    [Required]
    public ProductCategory ProductCategory { get; set; }
    
    [Required]
    public string? ProductBrand { get; set; }
}