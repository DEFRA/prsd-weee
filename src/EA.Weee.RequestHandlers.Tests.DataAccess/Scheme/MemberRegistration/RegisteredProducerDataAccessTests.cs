namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Producer;
    using Domain.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using FakeItEasy;
    using Weee.DataAccess;
    using Weee.Tests.Core;
    using Xunit;

    public class RegisteredProducerDataAccessTests
    {
        private readonly RegisteredProducerDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbHelper;
        private const string ProducerRegistrationNumber = "registrationNumber";
        private const string SchemeApprovalNumber = "approvalNumber";
        private const int ComplianceYear = 2019;
        private readonly Scheme scheme;
        private readonly RegisteredProducer registeredProducer;

        public RegisteredProducerDataAccessTests()
        {
            dbHelper = new DbContextHelper();
            scheme = A.Fake<Scheme>();
            context = A.Fake<WeeeContext>();
            registeredProducer = RegisteredProducer();

            A.CallTo(() => scheme.ApprovalNumber).Returns(SchemeApprovalNumber);
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            dataAccess = new RegisteredProducerDataAccess(context);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenMultipleResults_ArgumentExceptionShouldBeThrown()
        {
            var registeredProducers = new List<RegisteredProducer> { RegisteredProducer(), RegisteredProducer() };

            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(registeredProducers));

            Func<Task> action = async () =>
                await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenComplianceYearDiffers_ResultShouldBeNull()
        {
            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, 2000, SchemeApprovalNumber);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenSchemeApprovalNumberDiffers_ResultShouldBeNull()
        {
            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, "myscheme");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenProducerRegistrationNumberDiffers_ResultShouldBeNull()
        {
            var result = await dataAccess.GetProducerRegistration("myreg", ComplianceYear, SchemeApprovalNumber);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenQueryParametersMatch_ResultShouldResult()
        {
            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.Equal(registeredProducer, result);
        }

        private RegisteredProducer RegisteredProducer()
        {
            return new RegisteredProducer(ProducerRegistrationNumber, ComplianceYear, scheme);
        }
    }
}
