using System.Text.Json;

namespace StashMaven.WebApi.Features.Partnership.Partners;

public partial class PartnerController
{
    [HttpPost]
    [Route("import")]
    public async Task<IActionResult> ImportPartnersAsync(
        IFormFile file,
        [FromServices]
        ImportPartnersHandler handler)
    {
        if (file.Length == 0)
        {
            return BadRequest();
        }

        await using Stream stream = file.OpenReadStream();
        List<ImportPartnersHandler.ImportedPartner>? partners =
            await JsonSerializer.DeserializeAsync<List<ImportPartnersHandler.ImportedPartner>>(
                stream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (partners == null)
        {
            return BadRequest("Could not deserialize input file.");
        }

        StashMavenResult response = await handler.ImportPartnersAsync(partners);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Message);
        }

        return Ok();
    }
}

[Injectable]
public class ImportPartnersHandler(StashMavenContext context)
{
    public class ImportedPartner
    {
        public string? CustomIdentifier { get; set; }
        public string? LegalName { get; set; }
        public List<ImportedTaxIdentifier>? TaxIdentifiers { get; set; }
        public ImportedAddress? Address { get; set; }
    }

    public class ImportedTaxIdentifier
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class ImportedAddress
    {
        public string? Street { get; set; }
        public string? StreetAdditional { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryCode { get; set; }
    }

    public async Task<StashMavenResult> ImportPartnersAsync(
        List<ImportedPartner> partners)
    {
        foreach (ImportedPartner importedPartner in partners)
        {
            if (string.IsNullOrWhiteSpace(importedPartner.LegalName)
                || string.IsNullOrWhiteSpace(importedPartner.CustomIdentifier))
            {
                return StashMavenResult.Error("LegalName and CustomIdentifier are required");
            }

            if (importedPartner.Address is null
                || string.IsNullOrWhiteSpace(importedPartner.Address.Street)
                || string.IsNullOrWhiteSpace(importedPartner.Address.City)
                || string.IsNullOrWhiteSpace(importedPartner.Address.PostalCode)
                || string.IsNullOrWhiteSpace(importedPartner.Address.CountryCode))
            {
                return StashMavenResult.Error("Address is required");
            }

            if (importedPartner.TaxIdentifiers is null
                || importedPartner.TaxIdentifiers.Count == 0
                || importedPartner.TaxIdentifiers.Count(t => t.IsPrimary) != 1)
            {
                return StashMavenResult.Error("Primary TaxIdentifier is required");
            }

            Partner partner = new()
            {
                Address = new Address
                {
                    City = importedPartner.Address.City,
                    CountryCode = importedPartner.Address.CountryCode,
                    PostalCode = importedPartner.Address.PostalCode,
                    State = importedPartner.Address.State,
                    Street = importedPartner.Address.Street,
                    StreetAdditional = importedPartner.Address.StreetAdditional,
                },
                CustomIdentifier = importedPartner.CustomIdentifier,
                LegalName = importedPartner.LegalName,
                PartnerId = new PartnerId(Guid.NewGuid().ToString()),
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
            };

            HashSet<string> allowedTypes = Enum.GetNames(typeof(TaxIdentifierType))
                .Select(x => x.ToLowerInvariant())
                .ToHashSet();

            foreach (ImportedTaxIdentifier taxIdentifier in importedPartner.TaxIdentifiers)
            {
                if (string.IsNullOrWhiteSpace(taxIdentifier.Type)
                    || string.IsNullOrWhiteSpace(taxIdentifier.Value))
                {
                    return StashMavenResult.Error("TaxIdentifier Type and Value are required");
                }

                if (!allowedTypes.Contains(taxIdentifier.Type.ToLowerInvariant()))
                {
                    return StashMavenResult.Error(
                        $"TaxIdentifier Type must be one of {string.Join(", ", allowedTypes)}");
                }

                partner.TaxIdentifiers.Add(new TaxIdentifier
                {
                    Type = Enum.Parse<TaxIdentifierType>(taxIdentifier.Type),
                    Value = taxIdentifier.Value,
                    IsPrimary = taxIdentifier.IsPrimary,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                });
            }

            await context.Partners.AddAsync(partner);
        }

        await context.SaveChangesAsync();

        return StashMavenResult.Success();
    }
}
