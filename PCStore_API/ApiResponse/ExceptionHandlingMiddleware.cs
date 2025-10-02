using System.Net;
using System.Text.Json;

namespace PCStore_API.ApiResponse;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    // constructor injection
    public async Task InvokeAsync(HttpContext context)
    {
        // call the next delegate/middleware in the pipeline
        try
        {
            await next(context); // let the pipeline continue
        }
        // catch any exceptions that were thrown
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    // helper method to handle exceptions
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // set the status code
        var statusCode = exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            BusinessRuleException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        // set the response
        response.StatusCode = statusCode;

        // serialize the error to JSON
        var result = JsonSerializer.Serialize(ApiResponse<object>.FailureResponse(exception.Message));
        return response.WriteAsync(result);
    }
}