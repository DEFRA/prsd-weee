namespace EA.Weee.DataAccess.Tests.Integration.ProducerRemoval
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;
    using RegisteredProducer = Domain.Producer.RegisteredProducer;
    using Scheme = Domain.Scheme.Scheme;

    public class RegisteredProducerRemovalTests
    {
        private readonly IUserContext userContext;

        public RegisteredProducerRemovalTests()
        {
            userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsNotRemoved()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;

                var organisation = Organisation.CreateSoleTrader("My trading name");
                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var scheme = new Scheme(organisation.Id);
                context.Schemes.Add(scheme);
                await context.SaveChangesAsync();

                var producer = new RegisteredProducer("ABC12345", 2017, scheme);
                context.AllRegisteredProducers.Add(producer);
                await context.SaveChangesAsync();

                producer = context.RegisteredProducers
                    .SingleOrDefault(p => p.Id == producer.Id);

                Assert.NotNull(producer);
                Assert.False(producer.Removed);
            }
        }

        [Fact]
        public void InvokeRemove_ForAlreadyRemovedProducer_ThrowsInvalidOperationException()
        {
            var producer = new RegisteredProducer("ABC12345", 2017, A.Dummy<Scheme>());
            producer.Remove();

            Assert.Throws<InvalidOperationException>(() => producer.Remove());
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsNotRemoved_ThenRemove_ProducerEntityNotInDbSet()
        {   
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var organisation = Organisation.CreateSoleTrader("My trading name");
                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var scheme = new Scheme(organisation.Id);
                context.Schemes.Add(scheme);
                await context.SaveChangesAsync();

                var producer = new RegisteredProducer("ABC12345", 2017, scheme);
                context.AllRegisteredProducers.Add(producer);
                await context.SaveChangesAsync();

                producer.Remove();

                await context.SaveChangesAsync();

                producer = context.RegisteredProducers
                    .SingleOrDefault(p => p.Id == producer.Id);

                Assert.Null(producer);
            }
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsNotRemoved_ThenRemoved_ThenReRegister_ReturnsNotRemovedProducer()
        {   
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var organisation = Organisation.CreateSoleTrader("My trading name");
                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var scheme = new Scheme(organisation.Id);
                context.Schemes.Add(scheme);
                await context.SaveChangesAsync();

                var producer = new RegisteredProducer("ABC12345", 2017, scheme);
                context.AllRegisteredProducers.Add(producer);
                await context.SaveChangesAsync();

                producer.Remove();
                await context.SaveChangesAsync();

                var notRemovedProducer = new RegisteredProducer("ABC12345", 2017, scheme);
                context.AllRegisteredProducers.Add(notRemovedProducer);
                await context.SaveChangesAsync();

                producer = context.RegisteredProducers
                    .SingleOrDefault(p => p.Id == notRemovedProducer.Id);

                Assert.NotNull(producer);
                Assert.False(producer.Removed);
            }
        }
    }
}
