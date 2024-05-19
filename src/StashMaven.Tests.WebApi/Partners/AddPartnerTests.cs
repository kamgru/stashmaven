using StashMaven.WebApi.Features.Partnership.Partners;

namespace StashMaven.Tests.WebApi.Partners;

public class AddPartnerTests(DefaultTestFixture fixture) : IClassFixture<DefaultTestFixture>
{
    [Fact]
    public async Task WhenRequestValid_ShouldAddPartner()
    {
        HttpClient client = fixture.CreateClient();

        await fixture.AddCountry();

        AddPartnerHandler.AddPartnerRequest request = new()
        {
            CustomIdentifier = Guid.NewGuid().ToString().Substring(0, 10),
            LegalName = Guid.NewGuid().ToString().Substring(0, 10),
            BusinessIdentifiers =
            [
                new AddPartnerHandler.AddPartnerRequest.AddPartnerBusinessIdentifier
                {
                    Type = "nip",
                    Value = Guid.NewGuid().ToString().Substring(0, 11)
                }
            ],
            Address = new AddPartnerHandler.AddPartnerRequest.AddPartnerAddress
            {
                City = "Warsaw",
                CountryCode = "PL",
                PostalCode = "00-000",
                State = "Mazowieckie",
                Street = "Test Street",
                StreetAdditional = "Test Street Additional"
            }
        };

        HttpResponseMessage result = await client.PostAsJsonAsync("api/v1/partner", request);

        result.EnsureSuccessStatusCode();

        AddPartnerHandler.AddPartnerResponse? response =
            await result.Content.ReadFromJsonAsync<AddPartnerHandler.AddPartnerResponse>();

        Assert.NotNull(response);

        await using StashMavenContext context = fixture.CreateDbContext();

        Partner? partner = await context.Partners.
            Include(partner => partner.Address)
            .Include(partner => partner.BusinessIdentifiers)
            .FirstOrDefaultAsync(x => x.PartnerId.Value == response.PartnerId);

        Assert.NotNull(partner);

        Assert.Equal(request.CustomIdentifier, partner.CustomIdentifier);
        Assert.Equal(request.LegalName, partner.LegalName);
        Assert.Equal(request.Address.City, partner.Address?.City);
        Assert.Equal(request.Address.CountryCode, partner.Address?.CountryCode);
        Assert.Equal(request.Address.PostalCode, partner.Address?.PostalCode);
        Assert.Equal(request.Address.State, partner.Address?.State);
        Assert.Equal(request.Address.Street, partner.Address?.Street);
        Assert.Equal(request.Address.StreetAdditional, partner.Address?.StreetAdditional);
        Assert.Equal(request.BusinessIdentifiers[0].Type, partner.BusinessIdentifiers[0].Type);
        Assert.Equal(request.BusinessIdentifiers[0].Value, partner.BusinessIdentifiers[0].Value);
    }
}
