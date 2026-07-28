namespace Blogfolio.Data;

public class Result<T>
{
    private T? _value = default;
    public bool IsSuccess { get; private set; }
    private string? _error;
    public bool IsError => !IsSuccess;

    private Result(bool success, T? value = default, string? err = null)
    {
        _value = value;
        IsSuccess = success;
    }

    public static Result<T> Ok(T value) => new(true, value: value);
    public static Result<T> Err(string err) => new(false, err: err);
}