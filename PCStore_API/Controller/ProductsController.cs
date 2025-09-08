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
    public async Task<ActionResult<List<ProductDto>>> FilterProduct(ProductCategory? category, string? brand)
    {
        var query = context.Products.AsQueryable();
        
        if(category.HasValue)
            query = query.Where(p => p.ProductCategory == category);
        
        if(!string.IsNullOrEmpty(brand))
            query = query.Where(p => p.ProductBrand == brand.ToUpper());

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