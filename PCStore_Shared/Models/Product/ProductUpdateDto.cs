namespace PCStore_Shared.Models.Product;

public class ProductUpdateDto
{
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImage { get; set; }
    public decimal? ProductPrice { get; set; }
    public int? ProductStock { get; set; }
    public ProductCategory? ProductCategory { get; set; }
    public string? ProductBrand { get; set; }
}