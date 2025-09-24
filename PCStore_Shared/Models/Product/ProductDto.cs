using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Product;

public enum ProductCategory
{
    PcCase,
    PcRam,
    PcMotherboard,
    PcCpuCooler,
    PcPowerSupply,
    PcGpu,
    PcCpu,
    PcStorage,
    PcCaseFans,
 
}
public class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDescription { get; set; }
    public string? ProductImage { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductBrand { get; set; }
    public ProductCategory ProductCategory { get; set; }
}