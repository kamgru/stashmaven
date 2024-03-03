namespace StashMaven.WebApi;

using ErrorCode = int;

public static class ErrorCodes
{
    // General
    public const ErrorCode FatalError = 1;
    
    // Partner
    public const ErrorCode PartnerNotFound = 100000;
    public const ErrorCode CustomIdentifierNotUnique = 100001;
    public const ErrorCode PartnerHasShipments = 100002;
    public const ErrorCode OnlyOnePrimaryTaxIdentifier = 100003;
    public const ErrorCode TaxIdentifierTypeNotSupported = 100004;
    public const ErrorCode TaxIdentifierTypeNotUnique = 100005;
    public const ErrorCode TaxIdentifierValueNotUnique = 100006;
    public const ErrorCode CountryCodeNotSupported = 100007;
    
    // Shipment
    public const ErrorCode ShipmentNotFound = 200000;
    public const ErrorCode ShipmentHasNoPartner = 200001;
    public const ErrorCode ShipmentNotPending = 200002;
    public const ErrorCode ShipmentHasNoSourceReference = 200003;
    public const ErrorCode ShipmentHasNoPartnerRefSnapshot = 200004;
    public const ErrorCode ShipmentKindSequenceGeneratorNotFound = 200005;
    public const ErrorCode ConcurrencyResolutionFailed = 200006;
    
}

public class StashMavenResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public ErrorCode? ErrorCode { get; set; }

    public static StashMavenResult Success() =>
        new()
        {
            IsSuccess = true,
            Message = string.Empty
        };

    public static StashMavenResult Error(
        string message) =>
        new()
        {
            IsSuccess = false,
            Message = message
        };

    public static StashMavenResult Error(
        ErrorCode errorCode) =>
        new()
        {
            IsSuccess = false,
            Message = string.Empty,
            ErrorCode = errorCode
        };

    public static StashMavenResult Error(
        ErrorCode errorCode,
        string message) =>
        new()
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode
        };

    public static StashMavenResult Error<T>(
        StashMavenResult<T> result) =>
        new()
        {
            ErrorCode = result.ErrorCode,
            IsSuccess = result.IsSuccess,
            Message = result.Message
        };
}

public class StashMavenResult<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public ErrorCode? ErrorCode { get; set; }

    public static StashMavenResult<T> Success(
        T data) =>
        new()
        {
            Data = data,
            IsSuccess = true,
            Message = string.Empty
        };

    public static StashMavenResult<T> Error(
        string message) =>
        new()
        {
            Data = default,
            IsSuccess = false,
            Message = message
        };

    public static StashMavenResult<T> Error(
        ErrorCode errorCode) =>
        new()
        {
            Data = default,
            IsSuccess = false,
            Message = string.Empty,
            ErrorCode = errorCode
        };

    public static StashMavenResult<T> Error(
        ErrorCode errorCode,
        string message) =>
        new()
        {
            Data = default,
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode
        };

    public static StashMavenResult Error(
        StashMavenResult<T> result) =>
        new()
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode
        };
}
