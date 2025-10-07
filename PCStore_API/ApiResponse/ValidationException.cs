namespace PCStore_API.ApiResponse;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}

public class DbUpdateConcurrencyException : Exception
{
    public DbUpdateConcurrencyException(string message) : base(message)
    {
    }
}