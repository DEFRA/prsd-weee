namespace EA.Weee.Domain
{
    using System;
    using Prsd.Core;

    public class User
    {
        private User()
        {
        }

        public User(string id, string firstName, string surname, string email)
        {
            Guard.ArgumentNotNullOrEmpty(() => id, id);
            Guard.ArgumentNotNullOrEmpty(() => firstName, firstName);
            Guard.ArgumentNotNullOrEmpty(() => surname, surname);
            Guard.ArgumentNotNullOrEmpty(() => email, email);
       
            FirstName = firstName;
            Surname = surname;
            Email = email;
            Id = id;
        }

        public string Id { get; private set; }

        public string FirstName { get; private set; }

        public string Surname { get; private set; }

        public string Email { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public void LinkToOrganisation(Organisation organisation)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);

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