namespace EA.Weee.RequestHandlers.Tests.Unit.Helpers
{
    using System;
    using Domain;

    internal class UserHelper
    {
        internal User GetUser(Guid id)
        {
            return GetUser(id, "TestFirstName", "TestLastName", "test@test.com");
        }

        internal User GetUser(Guid id, string firstName, string secondName, string email)
        {
            return new User(id.ToString(), firstName, secondName, email);
        }
    }
}
