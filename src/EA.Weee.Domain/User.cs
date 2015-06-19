﻿namespace EA.Weee.Domain
{
    using EA.Prsd.Core;

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
    }
}