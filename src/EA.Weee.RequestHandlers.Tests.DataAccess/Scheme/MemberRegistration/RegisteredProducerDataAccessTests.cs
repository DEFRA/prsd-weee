namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public RegisteredProducerDataAccessTests()
        {
            dbHelper = new DbContextHelper();            
            context = A.Fake<WeeeContext>();

            dataAccess = new RegisteredProducerDataAccess(context);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenMultipleResults_ArgumentExceptionShouldBeThrown()
        {
            var registeredProducers = new List<RegisteredProducer> { RegisteredProducer(ComplianceYear), RegisteredProducer(ComplianceYear) };

            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(registeredProducers));

            Func<Task> action = async () =>
                await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenComplianceYearDiffers_ResultShouldBeNull()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var registeredProducers = new List<RegisteredProducer> { registeredProducer };

            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, 2000, SchemeApprovalNumber);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenSchemeApprovalNumberDiffers_ResultShouldBeNull()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, "myscheme");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenProducerRegistrationNumberDiffers_ResultShouldBeNull()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.GetProducerRegistration("myreg", ComplianceYear, SchemeApprovalNumber);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenQueryParametersMatch_ResultShouldResult()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.Equal(registeredProducer, result);
        }

        [Fact]
        public async Task GetProducerRegistration_GivenCurrentSubmissionIsNull_ResultShouldBeNull()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear, true);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = await dataAccess.GetProducerRegistration(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.Null(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoStatusMatch_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(1, StatusType.Insert, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoComplianceYearMatch_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(2018);
            var producerSubmission = ProducerSubmission(1, StatusType.Amendment, 2018, registeredProducer, DateTime.Now);

            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            
            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenNoPreviousAmendmentChargeNoAmountMatch_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            var producerSubmission = ProducerSubmission(0, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission> { producerSubmission }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenAnInsertAndThenAmendment_TrueShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(
                new List<ProducerSubmission> { ProducerSubmission(1, StatusType.Insert, ComplianceYear, registeredProducer, DateTime.Now), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now.AddMilliseconds(1)) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.True(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenAnInitialAmendmentAndThenAmendment_TrueShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(
                new List<ProducerSubmission> { ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now.AddMilliseconds(1)) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.True(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenAnInitialAmendmentAndThenNoFurtherSubmissions_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(
                new List<ProducerSubmission> { ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenAnInitialInsertAndNoFurtherSubmissions_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);
            
            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(
                new List<ProducerSubmission> { ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenNoSubmissions_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission>()));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenAmendmentThatWasNotSubmittedAndNewAmendment_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission>() { ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now, false), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now, true) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenInsertThatWasNotSubmittedAndNewAmendment_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission>() { ProducerSubmission(1, StatusType.Insert, ComplianceYear, registeredProducer, DateTime.Now, false), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now, true) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        [Fact]
        public void GetPreviousAmendmentCharge_GivenInsertThatWasNotSubmittedAndAmendmentThatWasNotSubmitted_FalseShouldBeReturned()
        {
            var registeredProducer = RegisteredProducer(ComplianceYear);

            A.CallTo(() => context.ProducerSubmissions).Returns(dbHelper.GetAsyncEnabledDbSet(new List<ProducerSubmission>() { ProducerSubmission(1, StatusType.Insert, ComplianceYear, registeredProducer, DateTime.Now, false), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now, false), ProducerSubmission(1, StatusType.Amendment, ComplianceYear, registeredProducer, DateTime.Now, true) }));
            A.CallTo(() => context.RegisteredProducers).Returns(dbHelper.GetAsyncEnabledDbSet(new List<RegisteredProducer> { registeredProducer }));

            var result = dataAccess.HasPreviousAmendmentCharge(ProducerRegistrationNumber, ComplianceYear, SchemeApprovalNumber);

            Assert.False(result);
        }

        private ProducerSubmission ProducerSubmission(decimal amount, StatusType status, int complianceYear, RegisteredProducer registeredProducer, DateTime date, bool submitted = true)
        {
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.Id).Returns(registeredProducer.Scheme.Id);
            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(complianceYear);
            A.CallTo(() => memberUpload.IsSubmitted).Returns(submitted);
            A.CallTo(() => memberUpload.CreatedDate).Returns(date);

            return new ProducerSubmission(registeredProducer,
                memberUpload,
                A.Dummy<ProducerBusiness>(),
                A.Dummy<AuthorisedRepresentative>(),
                date.AddDays(1),
                A.Dummy<decimal?>(),
                A.Dummy<bool>(),
                A.Dummy<DateTime>(),
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

        private RegisteredProducer RegisteredProducer(int complianceYear, bool currentSubmissionNUll = false)
        {
            var scheme = A.Fake<Scheme>();
            A.CallTo(() => scheme.ApprovalNumber).Returns(SchemeApprovalNumber);
            var memberUpload = A.Fake<MemberUpload>();
            A.CallTo(() => memberUpload.Scheme).Returns(scheme);
            A.CallTo(() => memberUpload.ComplianceYear).Returns(complianceYear);

            var registered = new RegisteredProducer(ProducerRegistrationNumber, complianceYear, scheme);

            var sub = new ProducerSubmission(registered,
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
                0,
                StatusType.Insert);

            if (!currentSubmissionNUll)
            {
                registered.SetCurrentSubmission(sub);
            }

            return registered;
        }
    }
}
