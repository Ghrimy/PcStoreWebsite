using System.ComponentModel.DataAnnotations;

namespace PCStore_Shared.Models.Validation;

public class ValidateQuantityAttribute(int minValue, int maxValue) : ValidationAttribute
{
    private readonly int _maxValue = maxValue;
    private readonly int _minValue = minValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not int quantity) return new ValidationResult("Value is not an integer.");
        if (quantity < _minValue || quantity > _maxValue)
            return new ValidationResult($"Quantity must be between {_minValue} and {_maxValue}.");
        return ValidationResult.Success;
    }
}