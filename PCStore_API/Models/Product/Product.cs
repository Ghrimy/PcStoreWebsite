using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Models.Product;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    
    [DataType(DataType.ImageUrl)]
    public string? ProductImage { get; set; }
    
    [DataType(DataType.Currency)]
    [Precision(18, 2)]
    public decimal ProductPrice { get; set; }
    
    public int ProductStock { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public string? ProductBrand { get; set; }
    
}