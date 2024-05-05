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
    public List<TaxIdentifier> TaxIdentifiers { get; set; } = [];
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
        }
    }
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
    
    public class TypeConfig : IEntityTypeConfiguration<TaxIdentifier>
    {
        public void Configure(
            EntityTypeBuilder<TaxIdentifier> builder)
        {
            builder.ToTable("TaxIdentifier", "partnership");
        }
    }
}

public enum TaxIdentifierType
{
    Nip = 0,
    Regon = 1,
    Krs = 2
}

public record BusinessIdentifierId(string Value);

public class BusinessIdentifier
{
    public const int NameMaxLength = 256;
    public const int ShortCodeMaxLength = 10;
    
    public int Id { get; set; }
    public BusinessIdentifierId BusinessIdentifierId { get; set; }
    public required string Name { get; set; }
    public required string ShortCode { get; set; }
    
    public class TypeConfig : IEntityTypeConfiguration<BusinessIdentifier>
    {
        public void Configure(
            EntityTypeBuilder<BusinessIdentifier> builder)
        {
            builder.ToTable("BusinessIdentifier", "partnership");
            builder.OwnsOne(e => e.BusinessIdentifierId)
                .Property(e => e.Value)
                .HasColumnName("BusinessIdentifierId");
            builder.Property(e => e.Name)
                .HasMaxLength(NameMaxLength);
            builder.Property(e => e.ShortCode)
                .HasMaxLength(ShortCodeMaxLength);
            builder.HasIndex(e => e.ShortCode)
                .IsUnique();
        }
    }
}
