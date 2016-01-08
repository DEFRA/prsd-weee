namespace EA.Weee.Domain
{
    using Prsd.Core;

    public class User
    {
        protected User()
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

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, Surname); }
        }

        public void UpdateUserInfo(string firstName, string lastName)
        {
            Guard.ArgumentNotNullOrEmpty(() => firstName, firstName);
            Guard.ArgumentNotNullOrEmpty(() => lastName, lastName);

            FirstName = firstName;
            Surname = lastName;
        }
    }
}