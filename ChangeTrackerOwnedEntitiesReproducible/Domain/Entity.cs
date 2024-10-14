namespace ChangeTrackerOwnedEntitiesReproducible.Domain
{
    public abstract class Entity
    {
        protected Entity(Guid id)
        {
            Id = id;
            CreatedOnUtc = DateTime.UtcNow;
            CreatedBy = Guid.NewGuid();
        }

        protected Entity()
        {
        }

        public Guid Id { get; init; }
        public DateTime CreatedOnUtc { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime? ModifiedOnUtc { get; private set; }
        public Guid? ModifiedBy { get; private set; }

        public void Created(Guid createdBy, DateTime createdOnUtc)
        {
            CreatedOnUtc = createdOnUtc;
            CreatedBy = createdBy;
        }

        public void Modified(Guid createdBy, DateTime modifiedOnUtc)
        {
            ModifiedBy = createdBy;
            ModifiedOnUtc = modifiedOnUtc;
        }
    }
    
    public abstract class Entity<T> : Entity where T : class
    {
        protected Entity(Guid id) : base(id)
        {
        }

        protected Entity()
        {
        }
    }
}