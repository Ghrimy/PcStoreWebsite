using PCStore_API.Models.Product;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Extensions;

public static class ProductExtension
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductPrice = product.ProductPrice,
            ProductDiscount = product.ProductDiscount
        };
    }

    public static ProductCreateDto ToCreateDto(this Product product)
    {
        return new ProductCreateDto
        {
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductStock = product.ProductStock,
            ProductDiscount = product.ProductDiscount
        };
    }

    public static ProductUpdateDto ToUpdateDto(this Product product)
    {
        return new ProductUpdateDto
        {
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductStock = product.ProductStock,
            ProductDiscount = product.ProductDiscount
        };
    }

    public static void UpdateFromDto(this Product product, ProductUpdateDto dto)
    {
        if (dto.ProductName != null) product.ProductName = dto.ProductName;
        if (dto.ProductDescription != null) product.ProductDescription = dto.ProductDescription;
        if (dto.ProductImage != null) product.ProductImage = dto.ProductImage;
        if (dto.ProductBrand != null) product.ProductBrand = dto.ProductBrand;
        if (dto.ProductCategory.HasValue) product.ProductCategory = dto.ProductCategory.Value;
        if (dto.ProductStock.HasValue) product.ProductStock = dto.ProductStock.Value;
        if (dto.ProductPrice.HasValue) product.ProductPrice = dto.ProductPrice.Value;
        if (dto.ProductDiscount.HasValue) product.ProductDiscount = dto.ProductDiscount.Value;
    }
}