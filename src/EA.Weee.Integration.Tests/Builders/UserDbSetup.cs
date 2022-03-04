namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using Domain.User;

    public class UserDbSetup : DbTestDataBuilder<User, UserDbSetup>
    {
        protected override User Instantiate()
        {
            return Instance;
        }
    }
}
