using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCStore_API.ApiResponse;
using PCStore_API.Services.ProductServices;
using PCStore_Shared.Models.Product;

namespace PCStore_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    //[Authorize(Roles = "User")]
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        //Gets all products and converts them to the front end Dto
        var products = await productService.GetAllProductsAsync();
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products, "Products retrieved successfully"));
    }

    //[Authorize(Roles = "User")]
    [HttpGet("product/{id:int}")]
    public async Task<ActionResult<ProductDto>> FindProduct(int id)
    {
        //Gets the product and converts it to the front end Dto
        var product = await productService.GetProductByIdAsync(id);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product found"));
    }

    //[Authorize(Roles = "User")]
    [HttpGet("filter")]
    public async Task<ActionResult<List<ProductDto>>> FilterProduct(ProductCategory? category, string? brand,
        bool hasDiscount, decimal? minPrice, decimal? maxPrice,
        bool inStockOnly = false, string? sortBy = null, string sortOrder = "asc", int pageNumber = 1,
        int pageSize = 10)
    {
        //Filters the products based on the given parameters
        var products = await productService.SearchProductsAsync(category, brand, hasDiscount, minPrice, maxPrice,
            inStockOnly, sortBy, sortOrder, pageNumber, pageSize);
        return Ok(ApiResponse<List<ProductDto>>.SuccessResponse(products, pageNumber, pageSize, products.Count,
            "Products retrieved successfully"));
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<ProductUpdateDto>> UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateDto dto)
    {
        //Checks if the values are valid and fits the model
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ProductDto>.FailureResponse("Model is invalid", errors));
        }

        var updateProduct = await productService.UpdateProductAsync(id, dto);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(updateProduct, "Product updated successfully"));
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductCreateDto>> CreateProduct([FromBody] ProductCreateDto dto)
    {
        //Creates a new product with the given parameters, checks if the values are valid and fits the model
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<ProductDto>.FailureResponse("Model is invalid", errors));
        }

        var createProduct = await productService.CreateProductAsync(dto);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(createProduct, "Product created successfully"));
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        await productService.DeleteProductAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(true, "Product deleted successfully"));
    }
}