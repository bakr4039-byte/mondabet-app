namespace Mondabet.Shared.Domain;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess(Value!) : onFailure(Error!);
}

public record Error(string Code, string Message)
{
    public static Error NotFound(string entity, Guid id) =>
        new("NotFound", $"{entity} with Id '{id}' was not found.");

    public static Error Validation(string message) =>
        new("Validation", message);

    public static Error Unauthorized(string message = "Unauthorized") =>
        new("Unauthorized", message);

    public static Error Forbidden(string message = "Forbidden") =>
        new("Forbidden", message);

    public static Error Conflict(string message) =>
        new("Conflict", message);
}
