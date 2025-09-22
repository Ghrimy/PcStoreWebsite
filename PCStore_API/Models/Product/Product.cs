using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PCStore_Shared;

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