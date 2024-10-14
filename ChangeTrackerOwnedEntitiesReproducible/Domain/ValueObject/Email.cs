using EmailValidation;
using FluentValidation;

namespace ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject
{
    public sealed record Email : ValueObject
    {
        private Email(string value) => Value = value;

        public string Value { get; }

        public static Email Create(string value)
        {
            Ensure.NotNullOrEmpty(value);

            if (!EmailValidator.TryValidate(value, false, false, out EmailValidationError error))
            {
                throw new ValidationException("Bla bla...");
            }

            return new Email(value);
        }
    }
}
