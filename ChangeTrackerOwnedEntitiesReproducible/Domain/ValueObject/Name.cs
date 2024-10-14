namespace ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject
{
    public sealed record Name : ValueObject
    {
        private Name(string value) => Value = value;

        public string Value { get; }

        public static Name Create(string value)
        {
            Ensure.NotNullOrEmpty(value);

            return new Name(value);
        }
    }
}
