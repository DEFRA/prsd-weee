namespace EA.Weee.Domain.Organisation
{
    using System;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class Contact : Entity, IEquatable<Contact>
    {
        public Contact(string firstName, string lastName, string position)
        {
            Guard.ArgumentNotNullOrEmpty(() => firstName, firstName);
            Guard.ArgumentNotNullOrEmpty(() => lastName, lastName);
            Guard.ArgumentNotNullOrEmpty(() => position, position);

            FirstName = firstName;
            LastName = lastName;
            Position = position;
        }

        protected Contact()
        {
        }

        private string firstname;
        private string lastname;
        private string position;

        public string Position
        {
            get { return position; }
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Position cannot be greater than 35 characters")));
                }
                position = value;
            }
        }

        public string FirstName
        {
            get { return firstname; }
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("Firstname cannot be greater than 35 characters")));
                }
                firstname = value;
            }
        }

        public string LastName
        {
            get { return lastname; }
            private set
            {
                if (value != null && value.Length > 35)
                {
                    throw new InvalidOperationException(string.Format(("LastName cannot be greater than 35 characters")));
                }
                lastname = value;
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        public Contact OverwriteWhereNull(Contact otherContact)
        {
            if (otherContact == null)
            {
                return this;
            }

            otherContact.FirstName = FirstName;
            otherContact.LastName = LastName;
            otherContact.Position = Position;

            return otherContact;
        }

        public virtual bool Equals(Contact other)
        {
            if (other == null)
            {
                return false;
            }

            return FirstName == other.FirstName &&
                   LastName == other.LastName &&
                   Position == other.Position;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Contact);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
