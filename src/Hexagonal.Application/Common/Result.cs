namespace Hexagonal.Application.Common;

/// <summary>
/// Résultat d'une opération applicative (succès ou erreur).
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public string? ErrorCode { get; }

    protected Result(bool isSuccess, string? errorMessage = null, string? errorCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true);
    public static Result Failure(string message, string? code = null) => new(false, message, code);
}

/// <summary>
/// Résultat avec valeur de retour.
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true)
    {
        Value = value;
    }

    private Result(string message, string? code = null) : base(false, message, code)
    {
        Value = default;
    }

    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Failure(string message, string? code = null) => new(message, code);

    public T GetValueOrThrow() =>
        IsSuccess && Value != null ? Value : throw new InvalidOperationException(ErrorMessage ?? "Operation failed.");
}
