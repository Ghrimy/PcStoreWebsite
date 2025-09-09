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
    // Gets all products
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
            ProductStock = p.ProductStock,
            ProductBrand = p.ProductBrand,
        }).ToListAsync();
        
        return Ok(products);

    }

    // Gets product by id
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
                ProductStock = p.ProductStock,
                ProductBrand = p.ProductBrand,

            }).FirstOrDefaultAsync();
        
        if (product == null) return NotFound();

        return Ok(product);

    }

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
            ProductStock = p.ProductStock,
            ProductBrand = p.ProductBrand,
        }).ToListAsync();
        
        if(products.Count == 0) return NotFound("No products found with the given filters");
        
        return Ok(products);

    }

    
    [HttpPost]
    public void Post([FromBody] Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
    }
    
}