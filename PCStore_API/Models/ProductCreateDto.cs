using System.ComponentModel.DataAnnotations;
using PCStore_Shared;

namespace PCStore_API.Models;

public class ProductCreateDto
{

    [Required] public string? ProductName { get; set; }
    [Required] public string? ProductDescription { get; set; }
    [Required] public string? ProductImage { get; set; }
    [Required] public decimal ProductPrice { get; set; }
    [Required] public int ProductStock { get; set; }
    [Required] public ProductCategory ProductCategory { get; set; }
    [Required] public string? ProductBrand { get; set; }
}