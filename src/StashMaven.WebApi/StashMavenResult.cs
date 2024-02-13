namespace StashMaven.WebApi;


public class StashMavenResult
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }

    public static StashMavenResult Success()
    {
        return new StashMavenResult
        {
            IsSuccess = true,
            Message = string.Empty
        };
    }

    public static StashMavenResult Error(
        string message)
    {
        return new StashMavenResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

public class StashMavenResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static StashMavenResult<T> Success(
        T data)
    {
        return new StashMavenResult<T>
        {
            Data = data,
            IsSuccess = true,
            Message = string.Empty
        };
    }

    public static StashMavenResult<T> Error(
        string message)
    {
        return new StashMavenResult<T>

        {
            Data = default,
            IsSuccess = false,
            Message = message
        };
    }
}
