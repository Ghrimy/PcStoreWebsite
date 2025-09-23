namespace PCStore_Shared.Models;


//Generic response class
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Request successful")
        => new ApiResponse<T> { Success = true, Message = message, Data = data };
    
    public static ApiResponse<T> SuccessResponse(T data, int pageNumber, int pageSize, int totalCount, string message = "Request successful")
        => new ApiResponse<T> { Success = true, Message = message, Data = data, PageNumber = pageNumber, PageSize = pageSize, TotalCount = totalCount };

    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        => new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    
    
}
