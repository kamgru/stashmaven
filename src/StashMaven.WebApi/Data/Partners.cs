using System.Runtime.InteropServices;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    
    public class TypeConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(
            EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address", "partnership");
        }
    }
    
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Street);
        if (!string.IsNullOrWhiteSpace(StreetAdditional))
        {
            sb.Append(' ');
            sb.Append(StreetAdditional);
        }
        
        sb.Append(", ");
        sb.Append(PostalCode);
        sb.Append(", ");
        sb.Append(City);
        if (!string.IsNullOrWhiteSpace(State))
        {
            sb.Append(", ");
            sb.Append(State);
        }
        
        sb.Append(", ");
        sb.Append(CountryCode);
        return sb.ToString();
    }
}

public record PartnerId(
    string Value);

public class Partner
{
    public const int LegalNameMaxLength = 2048;
    public const int CustomIdentifierMaxLength = 50;
    public const int LegalNameMinLength = 3;
    public const int CustomIdentifierMinLength = 3;
    
    public int Id { get; set; }
    public required PartnerId PartnerId { get; set; }
    public required string LegalName { get; set; }
    public required string CustomIdentifier { get; set; }
    public Address? Address { get; set; }
    public List<BusinessIdentifier> BusinessIdentifiers { get; set; } = [];
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public List<Shipment> Shipments { get; set; } = [];
    
    public class TypeConfig : IEntityTypeConfiguration<Partner>
    {
        public void Configure(
            EntityTypeBuilder<Partner> builder)
        {
            builder.ToTable("Partner", "partnership");
            builder.OwnsOne(e => e.PartnerId)
                .Property(e => e.Value)
                .HasColumnName("PartnerId");
            builder.Property(e => e.LegalName)
                .HasMaxLength(LegalNameMaxLength);
            builder.Property(e => e.CustomIdentifier)
                .HasMaxLength(CustomIdentifierMaxLength);
            builder.HasIndex(e => e.CustomIdentifier)
                .IsUnique();
        }
    }
}

public class BusinessIdentifier
{
    public const int ValueMinLength = 2;
    public const int ValueMaxLength = 64;
    public const int TypeMinLength = 2;
    public const int TypeMaxLength = 16;
    
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public required string Type { get; set; }
    public required string Value { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Partner Partner { get; set; } = null!;
    
    public class TypeConfig : IEntityTypeConfiguration<BusinessIdentifier>
    {
        public void Configure(
            EntityTypeBuilder<BusinessIdentifier> builder)
        {
            builder.ToTable("BusinessIdentifier", "partnership");
            builder.Property(e => e.Type)
                .HasMaxLength(TypeMaxLength);
            builder.Property(e => e.Value)
                .HasMaxLength(ValueMaxLength);
            builder.HasIndex(e => new { e.Type, e.Value })
                .IsUnique();
        }
    }
}

public class BusinessIdType
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string CountryCode { get; init; }
    public bool IsPrimary { get; init; }
    
    public static readonly BusinessIdType Unknown = new()
    {
        Id = "",
        Name = "",
        CountryCode = "",
        IsPrimary = false
    };
    
    public static readonly BusinessIdType Nip = new()
    {
        Id = "nip",
        Name = "NIP",
        CountryCode = "pl",
        IsPrimary = true
    };
    
    public static readonly BusinessIdType Regon = new()
    {
        Id = "regon",
        Name = "REGON",
        CountryCode = "pl",
        IsPrimary = false
    };
    
    public static readonly BusinessIdType Krs = new()
    {
        Id = "krs",
        Name = "KRS",
        CountryCode = "pl",
        IsPrimary = false
    };
    
    public static BusinessIdType FromId(
        string id)
    {
        return id.ToLowerInvariant() switch
        {
            "nip" => Nip,
            "regon" => Regon,
            "krs" => Krs,
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }
    
    public static IReadOnlyList<BusinessIdType> All() =>
        new List<BusinessIdType>
        {
            Nip,
            Regon,
            Krs
        };
    
    public override bool Equals(
        object? obj) =>
        obj is BusinessIdType businessIdType
        && Id.Equals(businessIdType.Id, StringComparison.OrdinalIgnoreCase);
    
    protected bool Equals(
        BusinessIdType other) =>
        Id.Equals(other.Id, StringComparison.OrdinalIgnoreCase);
    
    public override int GetHashCode()
        => HashCode.Combine(Id, Name, CountryCode, IsPrimary);
}
