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

    public class IsProducerRegisteredForComplianceYearTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IRegisteredProducerRepository registeredProducerRepository;

        public IsProducerRegisteredForComplianceYearTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            registeredProducerRepository = A.Fake<IRegisteredProducerRepository>();
        }

        [Fact]
        public async void WhenUserIsUnauthorised_ShouldNotGetProducerRegistrations_OrSaveChanges()
        {
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new IsProducerRegisteredForComplianceYear("ABC12345", 2016);

            await Assert.ThrowsAsync<SecurityException>(() => IsProducerRegisteredForComplianceYearHandler().HandleAsync(request));

            A.CallTo(() => registeredProducerRepository.GetProducerRegistration(A<Guid>._))
                .MustNotHaveHappened();

            A.CallTo(() => registeredProducerRepository.SaveChangesAsync())
                .MustNotHaveHappened();
        }

        [Fact]
        public async void WhenUserIsAuthorised_ShouldGetProducerRegistrations()
        {
            var request = new IsProducerRegisteredForComplianceYear("ABC12345", 2016);

            await IsProducerRegisteredForComplianceYearHandler().HandleAsync(request);

            A.CallTo(() => registeredProducerRepository.GetProducerRegistrations(request.RegistrationNumber, request.ComplianceYear))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private IsProducerRegisteredForComplianceYearHandler IsProducerRegisteredForComplianceYearHandler()
        {
            return new IsProducerRegisteredForComplianceYearHandler(weeeAuthorization, registeredProducerRepository);
        }
    }
}
