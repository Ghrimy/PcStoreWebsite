namespace PCStore_API.ApiResponse;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine(context.Request.Method + " " + context.Request.Path);
        await next(context); // let the pipeline continue
        Console.WriteLine(context.Response.StatusCode + " " + context.Request.Path);
    }
}