using PCStore_API.Models.Product;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Extensions;

public static class ProductExtension
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductPrice = product.ProductPrice,
        };
    }

    public static ProductCreateDto ToCreateDto(this Product product)
    {
        return new ProductCreateDto()
        {
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductStock = product.ProductStock,
        };
        
    }

    public static ProductUpdateDto ToUpdateDto(this Product product)
    {
        return new ProductUpdateDto()
        {
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductImage = product.ProductImage,
            ProductPrice = product.ProductPrice,
            ProductBrand = product.ProductBrand,
            ProductCategory = product.ProductCategory,
            ProductStock = product.ProductStock,
        };
    }
}