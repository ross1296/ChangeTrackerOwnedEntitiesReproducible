using ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject;

namespace ChangeTrackerOwnedEntitiesReproducible.Domain
{
    public class Customer : Entity<Customer>
    {
        private Customer(Guid id,
            string? title,
            Name forename,
            Name surname,
            CustomerAddress correspondenceAddress,
            CustomerAddress? deliveryAddress,
            string? deliveryInstructions,
            Email? email,
            PhoneNumber? mobilePhoneNumber,
            PhoneNumber? altTelephoneNumber,
            bool marketingOptIn,
            bool welcomeCallComplete,
            bool smsMarketing
        ) : base(id)
        {
            Title = title;
            Forename = forename;
            Surname = surname;
            CorrespondenceAddress = correspondenceAddress;
            DeliveryAddress = deliveryAddress;
            DeliveryInstructions = deliveryInstructions;
            Email = email;
            MobilePhoneNumber = mobilePhoneNumber;
            AltTelephoneNumber = altTelephoneNumber;
            MarketingOptIn = marketingOptIn;
            WelcomeCallComplete = welcomeCallComplete;
            SmsMarketing = smsMarketing;
        }

        private Customer()
        {
        }

        public string? Title { get; private set; }
        public Name Forename { get; private set; }
        public Name Surname { get; private set; }
        public CustomerAddress CorrespondenceAddress { get; private set; }
        public CustomerAddress? DeliveryAddress { get; private set; }
        public string? DeliveryInstructions { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? MobilePhoneNumber { get; private set; }
        public PhoneNumber? AltTelephoneNumber { get; private set; }
        public bool MarketingOptIn { get; private set; }
        public bool WelcomeCallComplete { get; private set; }
        public bool SmsMarketing { get; private set; }


        public static Customer Create(string? title,
            Name forename,
            Name surname,
            CustomerAddress correspondenceAddress,
            CustomerAddress? deliveryAddress,
            string? deliveryInstructions,
            Email? email,
            PhoneNumber? mobilePhoneNumber,
            PhoneNumber? altTelephoneNumber,
            bool marketingOptIn,
            bool welcomeCallComplete,
            bool smsMarketing)
        {
            Customer patient = new Customer(Guid.NewGuid(),
                title,
                forename,
                surname,
                correspondenceAddress,
                deliveryAddress,
                deliveryInstructions,
                email,
                mobilePhoneNumber,
                altTelephoneNumber,
                marketingOptIn,
                welcomeCallComplete,
                smsMarketing);

            return patient;
        }

        public void Update(string? title,
            Name forename,
            Name surname,
            CustomerAddress correspondenceAddress,
            CustomerAddress? deliveryAddress,
            string? deliveryInstructions,
            Email? email,
            PhoneNumber? mobilePhoneNumber,
            PhoneNumber? altTelephoneNumber,
            bool marketingOptIn,
            bool welcomeCallComplete,
            bool smsMarketing)
        {
            Title = title;
            Forename = forename;
            Surname = surname;
            CorrespondenceAddress = correspondenceAddress;
            DeliveryAddress = deliveryAddress;
            DeliveryInstructions = deliveryInstructions;
            Email = email;
            MobilePhoneNumber = mobilePhoneNumber;
            AltTelephoneNumber = altTelephoneNumber;
            MarketingOptIn = marketingOptIn;
            WelcomeCallComplete = welcomeCallComplete;
            SmsMarketing = smsMarketing;
        }
    }
}