using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.ShoppingCart.Validation;

public class ValidateQuantityAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value is > 0 ? ValidationResult.Success : new ValidationResult("Invalid quantity amount.");
    }
}