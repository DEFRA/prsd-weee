namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Lookup;
    using Domain.Obligation;
    using Domain.Producer;
    using Domain.Producer.Classfication;
    using Domain.Producer.Classification;
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

        [Fact]
        public async void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoStatusMatch_FalseShouldBeReturned()
        {
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { ProducerSubmission(20, StatusType.Insert, 2019, registeredProducer) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public async void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoComplianceYearMatch_FalseShouldBeReturned()
        {
            var local = RegisteredProducer(2018);
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { local }));
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { ProducerSubmission(20, StatusType.Amendment, 2018, local) }));
            
            var result = await dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public async void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoAmountMatch_FalseShouldBeReturned()
        {
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { ProducerSubmission(0, StatusType.Amendment, 2019, registeredProducer) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public async void GetPreviousAmendmentCharge_GivenPreviousAmendmentCharge_TrueShouldBeReturned()
        {
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { ProducerSubmission(1, StatusType.Amendment, 2019, registeredProducer) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.True(result);
        }

        private ProducerSubmission ProducerSubmission(decimal amount, StatusType status, int complianceYear, RegisteredProducer registeredProducer)
        {
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Id).Returns(registeredProducer.Scheme.Id);
            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(complianceYear);

            return new ProducerSubmission(registeredProducer,
                memberUpload, 
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                A.Dummy<DateTime>(),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime?>(),
                A.Dummy<string>(),
                EEEPlacedOnMarketBandType.Lessthan5TEEEplacedonmarket,
                SellingTechniqueType.DirectSellingtoEndUser,
                ObligationType.B2B,
                AnnualTurnOverBandType.Greaterthanonemillionpounds,
                A.Dummy<List<BrandName>>(),
                A.Dummy<List<SICCode>>(),
                A.Dummy<ChargeBandAmount>(),
                amount,
                status);
        }

        private RegisteredProducer RegisteredProducer(int complianceYear = ComplianceYear)
        {
            return new RegisteredProducer(ProducerRegistrationNumber, complianceYear, scheme);
        }
    }
}
