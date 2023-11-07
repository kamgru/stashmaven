namespace StashMaven.WebApi;

public class StashMavenResult<T>
{
    public T? Data { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }

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
