namespace EA.Weee.Domain
{
    using System.Reflection;
    using Prsd.Core;

    public class Contact
    {
        public Contact(string title, string firstName, string lastName, string position)
        {
            Guard.ArgumentNotNull(firstName);
            Guard.ArgumentNotNull(lastName);
            Guard.ArgumentNotNull(Position);

            Title = title;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
        }

        protected Contact()
        {
        }

        public string Title { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Position { get; private set; }
    }
}