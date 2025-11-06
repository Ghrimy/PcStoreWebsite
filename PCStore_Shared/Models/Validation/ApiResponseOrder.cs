using PCStore_Shared.Models.Order;

namespace PCStore_Shared.Models.Validation;

public class ApiResponseOrder<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<OrderDto> Data { get; set; }
    public object Errors { get; set; }
}