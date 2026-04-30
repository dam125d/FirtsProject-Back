namespace Intap.FirstProject.Application.Abstractions.Results;

public class Result
{
    protected Result(bool isSuccess, ErrorResult error)
    {
        if (isSuccess && error != ErrorResult.None)
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && error == ErrorResult.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorResult Error { get; }

    public static Result Success() => new(true, ErrorResult.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, ErrorResult.None);
    public static Result Failure(ErrorResult error) => new(false, error);
    public static Result<TValue> Failure<TValue>(ErrorResult error) => new(default, false, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, ErrorResult error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failure result.");
}
