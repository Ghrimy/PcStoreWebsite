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
        var products = await context.Products.ToListAsync();
        
        if (products == null) return NotFound();
        
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

    
    [HttpPost]
    public void Post([FromBody] Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
    }
    
}