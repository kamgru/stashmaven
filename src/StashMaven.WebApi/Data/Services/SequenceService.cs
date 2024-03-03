namespace StashMaven.WebApi.Data.Services;

[Injectable]
public class SequenceService(StashMavenRepository repository)
{
    public async Task<StashMavenResult<ShipmentSequenceNumber>> GenerateShipmentSequence(
        Shipment shipment)
    {
        SequenceGenerator? sequenceGenerator =
            await repository.GetSequenceGeneratorAsync(shipment.Kind.SequenceGeneratorId);

        if (sequenceGenerator == null)
        {
            return StashMavenResult<ShipmentSequenceNumber>.Error(
                $"Sequence generator {shipment.Kind.SequenceGeneratorId.Value} not found.");
        }

        SequenceEntry? entry = sequenceGenerator.Entries
            .FirstOrDefault(x => x.Delimiter == shipment.Stockpile.ShortCode && x.Group == shipment.Kind.ShortCode);

        if (entry == null)
        {
            throw new StashMavenException(
                $"Sequence entry for stockpile {shipment.Stockpile.ShortCode} not found. This should never happen.");
        }

        string shipmentSequenceIdentifier =
            $"{entry.Group} {entry.NextValue}/{entry.Delimiter}/{DateTime.UtcNow:yy}";

        entry.NextValue += 1;

        return StashMavenResult<ShipmentSequenceNumber>.Success(new ShipmentSequenceNumber(shipmentSequenceIdentifier));
    }
}
