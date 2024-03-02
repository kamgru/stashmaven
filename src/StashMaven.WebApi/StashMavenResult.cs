namespace StashMaven.WebApi;

using ErrorCode = int;

public static class ErrorCodes
{
    public const ErrorCode PartnerNotFound = 100000;
    public const ErrorCode CustomIdentifierNotUnique = 100001;
    public const ErrorCode PartnerHasShipments = 100002;
    public const ErrorCode OnlyOnePrimaryTaxIdentifier = 100003;
    public const ErrorCode TaxIdentifierTypeNotSupported = 100004;
    public const ErrorCode TaxIdentifierTypeNotUnique = 100005;
    public const ErrorCode TaxIdentifierValueNotUnique = 100006;
    public const ErrorCode CountryCodeNotSupported = 100007;
}

public class StashMavenResult
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public ErrorCode? ErrorCode { get; set; }

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
    
    public static StashMavenResult Error(
        ErrorCode errorCode)
    {
        return new StashMavenResult
        {
            IsSuccess = false,
            Message = string.Empty,
            ErrorCode = errorCode
        };
    }
}

public class StashMavenResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public ErrorCode? ErrorCode { get; set; }

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
    
    public static StashMavenResult<T> Error(
        ErrorCode errorCode)
    {
        return new StashMavenResult<T>
        {
            Data = default,
            IsSuccess = false,
            Message = string.Empty,
            ErrorCode = errorCode
        };
    }
}
