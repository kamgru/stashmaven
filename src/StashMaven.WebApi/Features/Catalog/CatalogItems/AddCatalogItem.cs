using Npgsql;

namespace StashMaven.WebApi.Features.Catalog.CatalogItems;

public partial class CatalogItemController
{
    [HttpPost]
    public async Task<IActionResult> AddCatalogItemAsync(
        AddCatalogItemHandler.AddCatalogItemRequest request,
        [FromServices]
        AddCatalogItemHandler handler)
    {
        StashMavenResult<AddCatalogItemHandler.AddCatalogItemResponse> response =
            await handler.AddCatalogItemAsync(request);

        if (!response.IsSuccess || response.Data is null)
        {
            return BadRequest(response.Message);
        }

        return Created($"api/v1/catalog/catalog-item/{response.Data.CatalogItemId}", response.Data);
    }
}

[Injectable]
public class AddCatalogItemHandler(
    StashMavenContext context)
{
    public class AddCatalogItemRequest
    {
        [MinLength(5)]
        [MaxLength(50)]
        public required string Sku { get; set; }

        [MinLength(3)]
        [MaxLength(256)]
        public required string Name { get; set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; set; }
        public required string BuyTaxDefinitionId { get; set; }
        public required string SellTaxDefinitionId { get; set; }
    }

    public record AddCatalogItemResponse(string CatalogItemId);

    public async Task<StashMavenResult<AddCatalogItemResponse>> AddCatalogItemAsync(
        AddCatalogItemRequest request)
    {
        TaxDefinition? buyTax = await context.TaxDefinitions
            .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.BuyTaxDefinitionId);

        if (buyTax == null)
        {
            return StashMavenResult<AddCatalogItemResponse>.Error(
                $"Tax definition {request.BuyTaxDefinitionId} not found");
        }

        TaxDefinition? sellTax = await context.TaxDefinitions
            .SingleOrDefaultAsync(t => t.TaxDefinitionId.Value == request.SellTaxDefinitionId);

        if (sellTax == null)
        {
            return StashMavenResult<AddCatalogItemResponse>.Error(
                $"Tax definition {request.SellTaxDefinitionId} not found");
        }

        CatalogItemId catalogItemId = new(Guid.NewGuid().ToString());

        CatalogItem catalogItem = new()
        {
            CatalogItemId = catalogItemId,
            Sku = request.Sku,
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            BuyTax = new CatalogItemTaxReference
            {
                Name = buyTax.Name,
                Rate = buyTax.Rate,
                TaxDefinitionIdValue = buyTax.TaxDefinitionId.Value
            },
            SellTax = new CatalogItemTaxReference
            {
                Name = sellTax.Name,
                Rate = sellTax.Rate,
                TaxDefinitionIdValue = sellTax.TaxDefinitionId.Value
            },
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        context.CatalogItems.Add(catalogItem);

        try
        {
            await context.SaveChangesAsync();
            return StashMavenResult<AddCatalogItemResponse>.Success(new AddCatalogItemResponse(catalogItemId.Value));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult<AddCatalogItemResponse>.Error(
                    $"Catalog item with SKU {request.Sku} already exists");
            }

            throw;
        }
    }
}
