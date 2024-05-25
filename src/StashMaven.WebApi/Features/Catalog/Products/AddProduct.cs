using Npgsql;

namespace StashMaven.WebApi.Features.Catalog.Products;

public partial class ProductController
{
    [HttpPost]
    public async Task<IActionResult> AddProductAsync(
        AddProductHandler.AddProductRequest request,
        [FromServices]
        AddProductHandler handler)
    {
        StashMavenResult<AddProductHandler.AddProductResponse> response =
            await handler.AddProductAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response.ErrorCode);
        }

        return Created($"api/v1/product/{response.Data?.ProductId}", response.Data);
    }
}

[Injectable]
public class AddProductHandler(
    StashMavenRepository repository,
    UnitOfWork unitOfWork)
{
    public class AddProductRequest
    {
        [MinLength(5)]
        [MaxLength(50)]
        public required string Sku { get; set; }

        [MinLength(3)]
        [MaxLength(256)]
        public required string Name { get; set; } = null!;

        public UnitOfMeasure UnitOfMeasure { get; set; }
        
        public required string DefaultTaxDefinitionId { get; set; }
    }

    public record AddProductResponse(string ProductId);

    public async Task<StashMavenResult<AddProductResponse>> AddProductAsync(
        AddProductRequest request)
    {
        TaxDefinition? taxDefinition = await repository.GetTaxDefinitionAsync(
            new TaxDefinitionId(request.DefaultTaxDefinitionId));
        
        if (taxDefinition is null)
        {
            return StashMavenResult<AddProductResponse>.Error(ErrorCodes.TaxDefinitionNotFound);
        }
        
        ProductId productId = new(Guid.NewGuid().ToString());

        Product product = new()
        {
            ProductId = productId,
            Sku = request.Sku,
            Name = request.Name,
            UnitOfMeasure = request.UnitOfMeasure,
            DefaultTaxDefinition = taxDefinition,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow
        };

        repository.InsertProduct(product);
        
        IReadOnlyList<Stockpile> stockpiles = await repository.GetAllStockpilesAsync();
        
        foreach (Stockpile stockpile in stockpiles)
        {
            InventoryItem inventoryItem = new()
            {
                InventoryItemId = new InventoryItemId(Guid.NewGuid().ToString()),
                Sku = product.Sku,
                Name = product.Name,
                Product = product,
                Stockpile = stockpile,
            };

            repository.InsertInventoryItem(inventoryItem);
        }

        try
        {
            await unitOfWork.SaveChangesAsync();
            return StashMavenResult<AddProductResponse>.Success(new AddProductResponse(productId.Value));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return StashMavenResult<AddProductResponse>.Error(ErrorCodes.SkuAlreadyExists);
            }

            throw;
        }
    }
}
