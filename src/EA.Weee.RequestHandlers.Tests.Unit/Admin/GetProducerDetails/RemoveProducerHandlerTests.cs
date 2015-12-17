namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using System;
    using System.Security;
    using DataAccess.DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using Xunit;

    public class RemoveProducerHandlerTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IRemoveProducerDataAccess removeProducerDataAccess;

        public RemoveProducerHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            removeProducerDataAccess = A.Fake<IRemoveProducerDataAccess>();
        }

        [Fact]
        public async void WhenUserIsUnauthorised_ShouldNotGetProducer_OrSaveChanges()
        {
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new RemoveProducer(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(() => RemoveProducerHandler().HandleAsync(request));

            A.CallTo(() => removeProducerDataAccess.GetProducerRegistration(A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => removeProducerDataAccess.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async void WhenUserIsAuthorised_ShouldGetProducer_AndProducerSubmissions_AndSaveChanges()
        {
            var request = new RemoveProducer(Guid.NewGuid());

            await RemoveProducerHandler().HandleAsync(request);

            A.CallTo(() => removeProducerDataAccess.GetProducerRegistration(request.RegisteredProducerId))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => removeProducerDataAccess.GetProducerSubmissionsForRegisteredProducer(request.RegisteredProducerId))
                .MustHaveHappened(Repeated.Exactly.Once);
            
            A.CallTo(() => removeProducerDataAccess.SaveChangesAsync())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private RemoveProducerHandler RemoveProducerHandler()
        {
            return new RemoveProducerHandler(weeeAuthorization, removeProducerDataAccess);
        }
    }
}
