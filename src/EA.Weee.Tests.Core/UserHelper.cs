namespace EA.Weee.Tests.Core
{
    using Domain;
    using System;

    public class UserHelper
    {
        public User GetUser(Guid id)
        {
            return GetUser(id, "TestFirstName", "TestLastName", "test@test.com");
        }

        public User GetUser(Guid id, string firstName, string secondName, string email)
        {
            return new User(id.ToString(), firstName, secondName, email);
        }
    }
}
