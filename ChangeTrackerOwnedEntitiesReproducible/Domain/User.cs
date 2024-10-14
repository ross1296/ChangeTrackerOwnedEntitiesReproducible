using ChangeTrackerOwnedEntitiesReproducible.Domain.ValueObject;

namespace ChangeTrackerOwnedEntitiesReproducible.Domain
{
    public class User : Entity<User>
    {
        private User(Guid id,
            string? title,
            Name forename,
            Name surname,
            Email? email,
            PhoneNumber? mobilePhoneNumber
        ) : base(id)
        {
            Title = title;
            Forename = forename;
            Surname = surname;
            Email = email;
            MobilePhoneNumber = mobilePhoneNumber;
        }

        private User()
        {
        }

        public string? Title { get; private set; }
        public Name Forename { get; private set; }
        public Name Surname { get; private set; }
        public Email? Email { get; private set; }
        public PhoneNumber? MobilePhoneNumber { get; private set; }


        public static User Create(string? title,
            Name forename,
            Name surname,
            Email? email,
            PhoneNumber? mobilePhoneNumber)
        {
            User user = new User(Guid.NewGuid(),
                title,
                forename,
                surname,
                email,
                mobilePhoneNumber);

            return user;
        }

        public void Update(string? title,
            Name forename,
            Name surname,
            Email? email,
            PhoneNumber? mobilePhoneNumber)
        {
            Title = title;
            Forename = forename;
            Surname = surname;
            Email = email;
            MobilePhoneNumber = mobilePhoneNumber;
        }
    }
}