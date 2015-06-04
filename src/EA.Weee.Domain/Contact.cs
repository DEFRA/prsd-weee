namespace EA.Weee.Domain
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class Contact : Entity
    {
        public Contact(string firstName, string lastName, string position)
        {
            Guard.ArgumentNotNull(firstName);
            Guard.ArgumentNotNull(lastName);
            Guard.ArgumentNotNull(position);

            FirstName = firstName;
            LastName = lastName;
            Position = position;
        }

        protected Contact()
        {
        }

        public string Position { get; set; }
        
        public string LastName { get; set; }

        public string FirstName { get; set; }
    }
}