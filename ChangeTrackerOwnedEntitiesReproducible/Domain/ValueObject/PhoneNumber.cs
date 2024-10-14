using FluentValidation;
using PhoneNumbers;

namespace ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject
{
    public sealed record PhoneNumber : ValueObject
    {
        private PhoneNumber(string value) => Value = value;

        public string Value { get; }

        public static PhoneNumber Create(string value)
        {
            try
            {
                Ensure.NotNullOrEmpty(value);

                PhoneNumberUtil? phoneNumberUtil = PhoneNumberUtil.GetInstance();
                PhoneNumbers.PhoneNumber? parsedPhoneNumber = phoneNumberUtil.Parse(value, "GB");

                if (!phoneNumberUtil.IsValidNumberForRegion(parsedPhoneNumber, "GB"))
                {
                    throw new ValidationException("Bla bla...");
                }

                return new PhoneNumber(value);
            }
            catch (NumberParseException)
            {
                throw new ValidationException("Bla bla...");
            }
        }
    }
}
