﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.GetProducerDetails
{
    using DataAccess.DataAccess;
    using FakeItEasy;
    using RequestHandlers.Admin.GetProducerDetails;
    using RequestHandlers.Security;
    using Requests.Admin;
    using System;
    using System.Security;
    using Xunit;

    public class IsProducerRegisteredForComplianceYearTests
    {
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IRegisteredProducerDataAccess registeredProducerDataAccess;

        public IsProducerRegisteredForComplianceYearTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            registeredProducerDataAccess = A.Fake<IRegisteredProducerDataAccess>();
        }

        [Fact]
        public async void WhenUserIsUnauthorised_ShouldNotGetProducerRegistrations()
        {
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .Throws<SecurityException>();

            var request = new IsProducerRegisteredForComplianceYear("ABC12345", 2016);

            await Assert.ThrowsAsync<SecurityException>(() => IsProducerRegisteredForComplianceYearHandler().HandleAsync(request));

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistration(A<Guid>._))
                .MustNotHaveHappened();
        }

        [Fact]
        public async void WhenUserIsAuthorised_ShouldGetProducerRegistrations()
        {
            var request = new IsProducerRegisteredForComplianceYear("ABC12345", 2016);

            await IsProducerRegisteredForComplianceYearHandler().HandleAsync(request);

            A.CallTo(() => registeredProducerDataAccess.GetProducerRegistrations(request.RegistrationNumber, request.ComplianceYear))
                .MustHaveHappened(1, Times.Exactly);
        }

        private IsProducerRegisteredForComplianceYearHandler IsProducerRegisteredForComplianceYearHandler()
        {
            return new IsProducerRegisteredForComplianceYearHandler(weeeAuthorization, registeredProducerDataAccess);
        }
    }
}
