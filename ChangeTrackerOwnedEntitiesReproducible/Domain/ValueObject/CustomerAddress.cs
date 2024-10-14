namespace ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject
{
    public record CustomerAddress(
        string AddressLine1,
        string? AddressLine2,
        string? City,
        string? County,
        string Postcode) : ValueObject;
}
