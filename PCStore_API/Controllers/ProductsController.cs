using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Extensions;
using PCStore_API.Models.Product;
using PCStore_Shared.Models;
using PCStore_Shared.Models.Product;


namespace PCStore_API.Controllers;


[ApiController]
[Route("api/[controller]")]

public class ProductsController(PcStoreDbContext context, ILogger<ProductsController> logger) : ControllerBase
{
    /// <summary>
    /// Front end Dto
    /// Gets all products and search functions
    /// Uses productDto that only has the basic information for customer
    /// </summary>
    //[Authorize(Roles = "User")]
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        //Gets all products and converts them to the front end Dto
        var products = await context.Products.Select(p => p.ToDto()).ToListAsync();
        
        logger.LogInformation("Products found: {Count}", products.Count);
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products, "Products found"));

    }

    [Authorize(Roles = "User")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> FindProduct(int id)
    { 
        //Gets the product and converts it to the front end Dto
        var product = await context.Products
            .Where(p => p.ProductId == id)
            .Select(p => p.ToDto()).FirstOrDefaultAsync();

        //Checks if the product exists
        if (product == null)
        {
            logger.LogInformation("Product not found with id {ProductId}", id);
            return NotFound(ApiResponse<ProductDto>.FailureResponse("Product not found", new List<string> { "No product with given id" }));

        }

        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product found"));

    }

    /// <summary>
    /// Filters the products
    /// Uses the queryable extension methods to filter the products
    /// Uses the productDto that only has the basic information for customer
    /// </summary>

    [Authorize(Roles = "User")]
    [HttpGet("filter")]
    public async Task<ActionResult<List<ProductDto>>> FilterProduct(
        ProductCategory? category,
        string? brand,
        bool HasDiscount,
        decimal? minPrice,
        decimal? maxPrice,
        bool inStockOnly = false,
        string? sortBy = null,
        string sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = context.Products.AsQueryable();
        
        //Filters the products based on the given parameters
        if(category.HasValue)
            query = query.Where(p => p.ProductCategory == category);

        if (HasDiscount)
            query = query.Where(p => p.ProductDiscount > 0);
        
        if(!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.ProductBrand.Equals(brand, StringComparison.OrdinalIgnoreCase));
        
        if (minPrice.HasValue)
            query = query.Where(p => p.ProductPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.ProductPrice <= maxPrice.Value);

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


        //Checks if the products exist
        if (products.Count == 0)
        {
            var appliedFilters = new List<string>();
            if (category.HasValue) appliedFilters.Add($"Category = {category.Value}");
            if (!string.IsNullOrEmpty(brand)) appliedFilters.Add($"Brand = {brand}");
            if (minPrice.HasValue) appliedFilters.Add($"MinPrice = {minPrice.Value}");
            if (maxPrice.HasValue) appliedFilters.Add($"MaxPrice = {maxPrice.Value}");
            if (inStockOnly) appliedFilters.Add("InStockOnly = true");

            logger.LogInformation("No products found. Filters used: {Filters}", string.Join(", ", appliedFilters));;
            return NotFound(ApiResponse<ProductDto>
                .FailureResponse("No products found", new List<string>{ "No products found with the given filters" }));

        }
        
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(
            products,
            pageNumber,
            pageSize,
            totalCount,
            "Products retrieved successfully"
        ));

    }

    /// <summary>
    /// Api endpoints for authorized users
    /// updates, creates and deletes products
    /// </summary>

    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<ProductUpdateDto>>  UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateDto dto)
    {
        //Checks if the values are valid and fits the model
        if (!ModelState.IsValid)
        {
            logger.LogInformation("Model is not valid");
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ProductDto>.FailureResponse("Model is invalid", errors));
        }
        
        //Checks if the product exists
        var product = await context.Products.FindAsync(id);
        if (product == null)
        {
            logger.LogInformation("Product not found with id {ProductId}", id);
            return NotFound(ApiResponse<ProductDto>.FailureResponse("No product found", new List<string>{ "Filter returned zero results" }));
        }

        //Updates the product with the given parameters
        product.UpdateFromDto(dto);
        
        //Logs the changes made to the product
        var entry = context.Entry(product);
        foreach (var prop in entry.Properties)
        {
            if (prop.IsModified)
            {
                logger.LogInformation("Property {Property} changed from {Old} to {New}",
                    prop.Metadata.Name,
                    prop.OriginalValue,
                    prop.CurrentValue);
            }
        }

        await context.SaveChangesAsync();
        return Ok(ApiResponse<ProductUpdateDto>.SuccessResponse(product.ToUpdateDto(), "Product updated"));
    }
    
    [Authorize(Roles = "Employee,Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductCreateDto>> CreateProduct([FromBody] ProductCreateDto dto)
    {
        //Creates a new product with the given parameters, checks if the values are valid and fits the model
        if (!ModelState.IsValid)
        {
            logger.LogInformation("Model is not valid");
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ProductDto>.FailureResponse("Model is invalid", errors));
        }

        //Creates a new product with the given parameters
        var createProduct = new Product()
        {
            ProductName = dto.ProductName,
            ProductDescription = dto.ProductDescription,
            ProductImage = dto.ProductImage,
            ProductPrice = dto.ProductPrice,
            ProductBrand = dto.ProductBrand,
            ProductCategory = dto.ProductCategory,
            ProductStock = dto.ProductStock,
            ProductDiscount = dto.ProductDiscount,
        };

        
        context.Products.Add(createProduct);
        logger.LogInformation("Created new product with id {ProductId}", createProduct.ProductId);
        await context.SaveChangesAsync();
        
        createProduct.ToCreateDto();
        return CreatedAtAction(nameof(FindProduct), new { id = createProduct.ProductId },
            ApiResponse<ProductDto>.SuccessResponse(createProduct.ToDto(), "Product created successfully"));
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        //Deletes the product with the given id
        var product = await context.Products.FindAsync(id);
        if (product == null) 
            return NotFound(ApiResponse<ProductDto>.FailureResponse("Product not found", new List<string> { "No product with given id" }));
        
        //Removes the product and logs the deletion
        context.Products.Remove(product);
        logger.LogInformation("Deleted product with id {ProductId}", id);
        await context.SaveChangesAsync();
        return Ok(ApiResponse<ProductDto>.SuccessResponse(null, "Product deleted successfully"));
    }
}