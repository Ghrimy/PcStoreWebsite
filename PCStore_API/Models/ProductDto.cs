namespace PCStore_API.Models;

public class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImage { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductStock { get; set; }
    public string? ProductBrand { get; set; }
}