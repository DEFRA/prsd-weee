namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Domain;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Xunit;

    public class OrganisationIntegration
    {
        private readonly WeeeContext context;

        public OrganisationIntegration()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            context = new WeeeContext(userContext);
        }
    }
}