using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.Product;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Services.ProductServices;

public class ProductService(PcStoreDbContext context, ILogger<ProductService> logger) : IProductService
{

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await context.Products.Select(p => p.ToDto()).ToListAsync();
        logger.LogInformation("Products found: {Count}", products.Count);
        if (products.Count == 0)
            throw new NotFoundException("No products found");

        return products;
    }

    public async Task<ProductDto> GetProductByIdAsync(int productId)
    {
        var products = await context.Products.Where(p => p.ProductId == productId).Select(p => p.ToDto())
            .FirstOrDefaultAsync();
        if (products == null)
            throw new NotFoundException("No product found");

        return products;
    }
    
    public async Task<List<ProductDto>> SearchProductsAsync(ProductCategory? category, string? brand, bool hasDiscount,
        decimal? minPrice,
        decimal? maxPrice, bool inStockOnly = false, string? sortBy = null, string sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = context.Products.AsQueryable();

        //Filters the products based on the given parameters
        if (category.HasValue)
            query = query.Where(p => p.ProductCategory == category);

        if (hasDiscount)
            query = query.Where(p => p.ProductDiscount > 0);

        if (!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.ProductBrand.ToLower() == brand.ToLower());


        if (minPrice.HasValue)
            query = query.Where(p => p.ProductPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.ProductPrice <= maxPrice.Value);

        if (pageSize <= 0) pageSize = 10;
        if (pageNumber < 1) pageNumber = 1;
        
        if (inStockOnly)
            query = query.Where(p => p.ProductStock > 0);

        var totalCount = await query.CountAsync();

        //Sorts the products based on the given parameters price or name        
        switch (sortBy?.ToLower())
        {
            case "price":
                query = sortOrder == "desc"
                    ? query.OrderByDescending(p => p.ProductPrice)
                    : query.OrderBy(p => p.ProductPrice);
                break;
            case "name":
                query = sortOrder == "desc"
                    ? query.OrderByDescending(p => p.ProductName)
                    : query.OrderBy(p => p.ProductName);
                break;
            default:
                query = sortOrder == "asc"
                    ? query.OrderBy(p => p.ProductName)
                    : query.OrderByDescending(p => p.ProductName);
                break;
        }

        //Converts the products to the front end Dto
        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.ToDto())
            .ToListAsync();
        


        //returns products if they exist
        return products.Count == 0 ? [] : products;
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateDto product)
    {
        //Creates a new product with the given parameters
        var createProduct = new Product
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


        context.Products.Add(createProduct);
        logger.LogInformation("Created new product with id {ProductId}", createProduct.ProductId);
        await context.SaveChangesAsync();
        return createProduct.ToDto();
    }

    public async Task<ProductDto> UpdateProductAsync(int productId, ProductUpdateDto product)
    {
        
        //Checks if the product exists
        var existingProduct = await context.Products.FindAsync(productId);
        if (existingProduct == null)
            throw new NotFoundException("Product not found");
        
        logger.LogInformation("Updating product {ProductId}", productId);
        existingProduct.UpdateFromDto(product);

        //Logs the changes made to the product
        var entry = context.Entry(existingProduct);
        foreach (var prop in entry.Properties)
            if (prop.IsModified)
                logger.LogInformation("Property {Property} changed from {Old} to {New}",
                    prop.Metadata.Name,
                    prop.OriginalValue,
                    prop.CurrentValue);

        await context.SaveChangesAsync();
        return existingProduct.ToDto();
    }

    public async Task DeleteProductAsync(int productId)
    {
        //Deletes the product with the given id
        var product = await context.Products.FindAsync(productId);
        if (product == null)
            throw new NotFoundException("Product not found");

        //Removes the product and logs the deletion
        context.Products.Remove(product);
        logger.LogInformation("Deleted product with id {ProductId}", productId);
        await context.SaveChangesAsync();
    }
}