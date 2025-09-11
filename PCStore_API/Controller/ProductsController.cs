using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCStore_API.Data;
using PCStore_API.Models;
using PCStore_Shared;

namespace PCStore_API.Controller;


[ApiController]
[Route("api/[controller]")]

public class ProductsController(PcStoreDbContext context) : ControllerBase
{
    /// <summary>
    /// Front end Dto
    /// Gets all products and search functions
    /// Uses productDto that only has the basic information for customer
    /// </summary>
    
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await context.Products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductDescription = p.ProductDescription,
            ProductImage = p.ProductImage,
            ProductPrice = p.ProductPrice,
            ProductBrand = p.ProductBrand,
        }).ToListAsync();
        
        return Ok(products);

    }

    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> FindProduct(int id)
    {
        var product = await context.Products
            .Where(p => p.ProductId == id)
            .Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ProductImage = p.ProductImage,
                ProductPrice = p.ProductPrice,
                ProductBrand = p.ProductBrand,

            }).FirstOrDefaultAsync();
        
        if (product == null) return NotFound();

        return Ok(product);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="category"></param>
    /// <param name="brand"></param>
    /// <param name="minPrice"></param>
    /// <param name="maxPrice"></param>
    /// <param name="inStockOnly"></param>
    /// <param name="sortBy"></param>
    /// <param name="sortOrder"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("filter")]
    public async Task<ActionResult<List<ProductDto>>> FilterProduct(
        ProductCategory? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        bool inStockOnly = false,
        string? sortBy = null,
        string sortOrder = "asc",
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = context.Products.AsQueryable();
        
        if(category.HasValue)
            query = query.Where(p => p.ProductCategory == category);
        
        if(!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.ProductBrand.ToUpper() == brand.ToUpper());

        
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
        }
        
        if (minPrice.HasValue)
            query = query.Where(p => p.ProductPrice >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.ProductPrice <= maxPrice.Value);

        if (pageNumber >= 0)
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        
        if (inStockOnly)
            query = query.Where(p => p.ProductStock > 0);


        var products = await query.Select(p => new ProductDto()
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            ProductDescription = p.ProductDescription,
            ProductImage = p.ProductImage,
            ProductPrice = p.ProductPrice,
            ProductBrand = p.ProductBrand,
        }).ToListAsync();
        
        if(products.Count == 0) return NotFound("No products found with the given filters");
        
        return Ok(products);

    }

    /// <summary>
    /// Api endpoints for authorized users
    /// updates, creates and deletes products
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("/products/{id:int}")]
    public async Task<ActionResult<ProductUpdateDto>>  UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateDto dto)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found");
        
        if (dto.ProductName != null) product.ProductName = dto.ProductName;
        if (dto.ProductDescription != null) product.ProductDescription = dto.ProductDescription;
        if (dto.ProductImage != null) product.ProductImage = dto.ProductImage;
        if (dto.ProductBrand != null) product.ProductBrand = dto.ProductBrand;
        if (dto.ProductCategory.HasValue) product.ProductCategory = dto.ProductCategory.Value;
        if (dto.ProductStock.HasValue) product.ProductStock = dto.ProductStock.Value;
        if (dto.ProductPrice.HasValue) product.ProductPrice = dto.ProductPrice.Value;


        await context.SaveChangesAsync();
        return Ok(product);
    }
    
    [Authorize(Roles = "Employee,Admin")]
    [HttpPost("/products")]
    public async Task<ActionResult<ProductCreateDto>> CreateProduct([FromBody] ProductCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var createProduct = new Product()
        {
            ProductName = dto.ProductName,
            ProductDescription = dto.ProductDescription,
            ProductImage = dto.ProductImage,
            ProductPrice = dto.ProductPrice,
            ProductBrand = dto.ProductBrand,
            ProductCategory = dto.ProductCategory,
            ProductStock = dto.ProductStock,
        };
        
        context.Products.Add(createProduct);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(FindProduct), new { id = createProduct.ProductId }, createProduct);

    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpDelete("/products/{id:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();
        
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return NoContent();
    }
}