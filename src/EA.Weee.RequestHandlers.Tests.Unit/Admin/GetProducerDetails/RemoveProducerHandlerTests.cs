namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Security;
    using DataAccess.Repositories;
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Xunit;

    public class RemoveProducerHandlerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IRegisteredProducerRepository registeredProducerRepository;

        public RemoveProducerHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            registeredProducerRepository = A.Fake<IRegisteredProducerRepository>();
        }

        [Fact]
        public async void WhenUserIsUnauthorised_ShouldNotGetProducer_OrSaveChanges()
        {
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new RemoveProducer(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(() => RemoveProducerHandler().HandleAsync(request));

            A.CallTo(() => registeredProducerRepository.Get(A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => registeredProducerRepository.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async void WhenUserIsAuthorised_ShouldGetProducer_AndSaveChanges()
        {
            var request = new RemoveProducer(Guid.NewGuid());

            await RemoveProducerHandler().HandleAsync(request);

            A.CallTo(() => registeredProducerRepository.Get(request.RegisteredProducerId))
                .MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(() => registeredProducerRepository.SaveChangesAsync())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private RemoveProducerHandler RemoveProducerHandler()
        {
            return new RemoveProducerHandler(weeeAuthorization, registeredProducerRepository);
        }
    }
}
