using System.Text;
using System.Text.Json;

HttpClient httpClient = new();

httpClient.BaseAddress = new Uri("http://localhost:5253/api/v1/");
string jsonArray = File.ReadAllText("mock_partners.json");

List<CreatePartnerRequest> partners =
    JsonSerializer.Deserialize<List<CreatePartnerRequest>>(jsonArray,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    ?? throw new InvalidOperationException("Failed to deserialize partners.");

foreach (CreatePartnerRequest partner in partners)
{
    string json = JsonSerializer.Serialize(partner);
    StringContent content = new(json, Encoding.UTF8, "application/json");
    HttpResponseMessage response = await httpClient.PostAsync("partner", content);
    response.EnsureSuccessStatusCode();
}

public class CreatePartnerRequest
{
    public class PartnerAddress
    {
        public required string Street { get; set; }
        public string? StreetAdditional { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string PostalCode { get; set; }

        public required string CountryCode { get; set; }
    }

    public enum TaxIdentifierType
    {
        Nip = 0,
        Regon = 1,
        Krs = 2
    }

    public class TaxIdentifier
    {
        public required TaxIdentifierType TaxIdentifierType { get; set; }
        public required string Value { get; set; }
        public bool IsPrimary { get; set; }
    }

    public required string CustomIdentifier { get; set; }

    public required string LegalName { get; set; }
    public required List<TaxIdentifier> BusinessIdentifications { get; set; }
    public required PartnerAddress Address { get; set; }
}
