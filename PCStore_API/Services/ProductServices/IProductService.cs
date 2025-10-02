using PCStore_Shared.Models.Product;

namespace PCStore_API.Services.ProductServices;

public interface IProductService
{
    public Task<List<ProductDto>> GetAllProductsAsync();
    public Task<ProductDto> GetProductByIdAsync(int productId);

    public Task<List<ProductDto>> SearchProductsAsync(ProductCategory? category, string? brand, bool hasDiscount,
        decimal? minPrice, decimal? maxPrice, bool inStockOnly = false,
        string? sortBy = null, string sortOrder = "asc", int pageNumber = 1, int pageSize = 10);

    public Task<ProductDto> CreateProductAsync(ProductCreateDto product);
    public Task<ProductDto> UpdateProductAsync(int productId, ProductUpdateDto product);
    public Task DeleteProductAsync(int productId);
}