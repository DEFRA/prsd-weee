namespace EA.Weee.DataAccess.Tests.Integration.Unalignment
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Organisation;
    using Domain.Producer;
    using Domain.Scheme;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using Xunit;

    public class RegisteredProducerUnalignmentTests
    {
        private readonly IUserContext userContext;

        public RegisteredProducerUnalignmentTests()
        {
            userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.NewGuid());
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsAligned()
        {
            var context = WeeeContext();

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
            Assert.True(producer.IsAligned);
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsAligned_ThenUnlign_ProducerEntityNotInDbSet()
        {
            var context = WeeeContext();

            var organisation = Organisation.CreateSoleTrader("My trading name");
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var scheme = new Scheme(organisation.Id);
            context.Schemes.Add(scheme);
            await context.SaveChangesAsync();

            var producer = new RegisteredProducer("ABC12345", 2017, scheme);
            context.AllRegisteredProducers.Add(producer);
            await context.SaveChangesAsync();

            producer.Unalign();

            await context.SaveChangesAsync();

            producer = context.RegisteredProducers
                .SingleOrDefault(p => p.Id == producer.Id);

            Assert.Null(producer);
        }

        [Fact]
        public async Task CreateRegisteredProducer_ProducerIsAligned_ThenUnlign_ThenReRegister_ReturnsAlignedProducer()
        {
            var context = WeeeContext();

            var organisation = Organisation.CreateSoleTrader("My trading name");
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var scheme = new Scheme(organisation.Id);
            context.Schemes.Add(scheme);
            await context.SaveChangesAsync();

            var producer = new RegisteredProducer("ABC12345", 2017, scheme);
            context.AllRegisteredProducers.Add(producer);
            await context.SaveChangesAsync();

            producer.Unalign();
            await context.SaveChangesAsync();

            var alignedProducer = new RegisteredProducer("ABC12345", 2017, scheme);
            context.AllRegisteredProducers.Add(alignedProducer);
            await context.SaveChangesAsync();

            producer = context.RegisteredProducers
                .SingleOrDefault(p => p.Id == alignedProducer.Id);

            Assert.NotNull(producer);
            Assert.True(producer.IsAligned);
        }

        private WeeeContext WeeeContext()
        {
            var eventDispatcher = A.Fake<IEventDispatcher>();

            return new WeeeContext(userContext, eventDispatcher);
        }
    }
}
