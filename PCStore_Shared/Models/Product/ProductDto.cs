using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.Product.Validation;

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
    Mouse,
    Keyboard,
    Headphones,
    Monitor,
}

public class ProductDto
{
    [Range(0, int.MaxValue)] public int ProductId { get; set; }

    [Required] public string? ProductName { get; set; }

    [Required] public string? ProductDescription { get; set; }

    [Required] public string? ProductImage { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal ProductPrice { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public decimal ProductDiscount { get; set; }

    public decimal FinalPrice => Math.Round(ProductPrice * (1 - ProductDiscount / 100), 2);

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string? ProductBrand { get; set; }

    [Required] [ValidateCategory] public ProductCategory ProductCategory { get; set; }
}