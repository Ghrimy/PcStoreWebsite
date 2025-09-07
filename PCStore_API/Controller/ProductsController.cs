using Microsoft.AspNetCore.Mvc;
using PCStore_API.Data;
using PCStore_Shared;

namespace PCStore_API.Controller;


[ApiController]
[Route("api/[controller]")]

public class ProductsController(PcStoreDbContext context) : ControllerBase
{
    [HttpGet]
    public List<Product?> Get() => context.Products.ToList();
    
    [HttpGet("{id:int}")]
    public Product? Get(int id) => context.Products.Find(id);
    
    [HttpPost]
    public void Post([FromBody] Product product)
    {
        context.Products.Add(product);
        context.SaveChanges();
    }
    
}