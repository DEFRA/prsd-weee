namespace EA.Weee.DataAccess.Tests.Integration
{
    using System;
    using System.Linq;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Xunit;

    public class MigratedProducersIntegration
    {
        private readonly WeeeContext context;

        public MigratedProducersIntegration()
        {
            var userContext = A.Fake<IUserContext>();

            A.CallTo(() => userContext.UserId).Returns(Guid.NewGuid());

            IEventDispatcher eventDispatcher = A.Fake<IEventDispatcher>();

            context = new WeeeContext(userContext, eventDispatcher);
        }

        [Fact]
        public void CanRetrieveMigratedProducers()
        {
            context.MigratedProducers.FirstOrDefault();
        }
    }
}
