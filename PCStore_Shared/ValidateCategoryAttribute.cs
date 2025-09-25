using System.ComponentModel.DataAnnotations;
using PCStore_Shared.Models.Product;

namespace PCStore_Shared;

public class ValidateCategoryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is ProductCategory c && Enum.IsDefined(typeof(ProductCategory), c))
            return ValidationResult.Success;

        return new ValidationResult("Invalid product category.");
    }
}
