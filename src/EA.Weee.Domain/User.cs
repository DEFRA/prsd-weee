namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;

    public class User
    {
        private User()
        {
        }

        public User(string id, string firstName, string surname, string phoneNumber, string email)
        {
            Guard.ArgumentNotNull(firstName);
            Guard.ArgumentNotNull(surname);
            Guard.ArgumentNotNull(phoneNumber);
            Guard.ArgumentNotNull(email);
            Guard.ArgumentNotNull(id);

            FirstName = firstName;
            Surname = surname;
            PhoneNumber = phoneNumber;
            Email = email;
            Id = id;
        }

        public string Id { get; private set; }

        public string FirstName { get; private set; }

        public string Surname { get; private set; }

        public string PhoneNumber { get; private set; }

        public string Email { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public void LinkToOrganisation(Organisation organisation)
        {
            Guard.ArgumentNotNull(organisation);

            if (Organisation != null)
            {
                throw new InvalidOperationException(
                    "User is already linked to an organisation and may not be linked to another. This user is linked to organisation: " +
                    Organisation.Id);
            }

            Organisation = organisation;
        }
    }
}