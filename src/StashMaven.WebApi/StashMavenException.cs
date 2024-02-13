namespace StashMaven.WebApi;

public class StashMavenException : Exception
{
    public StashMavenException()
    {
    }

    public StashMavenException(
        string message)
        : base(message)
    {
    }

    public StashMavenException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}
