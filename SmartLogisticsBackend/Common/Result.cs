
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultErrorType ErrorType { get; }

    protected Result(bool isSuccess, string? error = null, 
                     ResultErrorType errorType = default)
    {
        IsSuccess = isSuccess;
        Error     = error;
        ErrorType = errorType;
    }

    public static Result Success()
        => new(true);

    public static Result Failure(string error, 
                                 ResultErrorType errorType = ResultErrorType.Invalid)
        => new(false, error, errorType);
}
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true)
        => Value = value;

    private Result(string error, ResultErrorType errorType)
        : base(false, error, errorType) { }

    public static Result<T> Success(T value)
        => new(value);

    public static Result<T> Failure(string error,
        ResultErrorType errorType = ResultErrorType.Invalid)
        => new(error, errorType);
    
    public static Result<T> Conflict(string error)
        => new(error, ResultErrorType.Conflict);

    public static Result<T> NotFound(string error)
        => new(error, ResultErrorType.NotFound);

    public static Result<T> Forbidden(string error)
        => new(error, ResultErrorType.Forbidden);
}

public enum ResultErrorType
{
    Conflict,
    NotFound,
    Forbidden,
    Invalid
}