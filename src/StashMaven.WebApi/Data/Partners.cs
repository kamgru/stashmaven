namespace StashMaven.WebApi.Data;

public class Address
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required string Street { get; set; }
    public string? StreetAdditional { get; set; }
    public required string City { get; set; }
    public string? State { get; set; }
    public required string PostalCode { get; set; }
    public required string CountryCode { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Partner Partner { get; set; } = null!;
}

public record PartnerId(
    string Value);

public class Partner
{
    public int Id { get; set; }
    public required PartnerId PartnerId { get; set; }
    public required string LegalName { get; set; }
    public required string CustomIdentifier { get; set; }
    public Address? Address { get; set; }
    public List<TaxIdentifier> TaxIdentifiers { get; set; } = [];
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}

public class TaxIdentifier
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required TaxIdentifierType Type { get; set; }
    public required string Value { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Partner Partner { get; set; } = null!;
}

public enum TaxIdentifierType
{
    Nip = 0,
    Regon = 1,
    Krs = 2
}
