namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using AutoFixture;
    using Core.Admin;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using RequestHandlers.Factories;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Xunit;

    public class GetAatfApprovalDateChangeStatusTests
    {
        private readonly GetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly QuarterWindow quarterWindow;

        private readonly Fixture fixture;

        public GetAatfApprovalDateChangeStatusTests()
        {
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();
            quarterWindow = new QuarterWindow(DateTime.MaxValue, DateTime.MaxValue, QuarterType.Q1);

            getAatfApprovalDateChangeStatus = new GetAatfApprovalDateChangeStatus(aatfDataAccess, quarterWindowFactory, organisationDataAccess);

            fixture = new Fixture();
        }

        [Fact]
        public async Task Validate_GivenApprovalDateHasNotChanged_EmptyFlagsShouldBeReturned()
        {
            var date = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalDate).Returns(date);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, date);

            result.Should().Be(0);
        }

        [Fact]
        public async Task Validate_GivenAatfHasNoApprovalDate_EmptyFlagsShouldBeReturned()
        {
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.ApprovalDate).Returns(null);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, fixture.Create<DateTime>());

            result.Should().Be(0);
        }

        [Fact]
        public async Task Validate_GivenQuarterHasNotChanged_EmptyFlagsShouldBeReturned()
        {
            var date = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalDate).Returns(date);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(aatf.ApprovalDate.Value)).Returns(QuarterType.Q1);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(date)).Returns(QuarterType.Q1);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, date);

            result.Should().Be(0);
        }

        [Fact]
        public async Task Validate_GivenQuarterHasChangedToPreviousQuarter_EmptyFlagsShouldBeReturned()
        {
            var date = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalDate).Returns(date);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(aatf.ApprovalDate.Value)).Returns(QuarterType.Q2);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(date)).Returns(QuarterType.Q1);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, date);

            result.Should().Be(0);
        }

        [Fact]
        public async Task Validate_GivenApprovalDateAndQuarterHasChanged_ShouldHaveChangedFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().HaveFlag(CanApprovalDateBeChangedFlags.DateChanged);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasOtherFacilities_ShouldMultipleFacilityFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();

            var aatf = A.Fake<Aatf>();

            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);
            A.CallTo(() => aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf)).Returns(true);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().HaveFlag(CanApprovalDateBeChangedFlags.HasMultipleFacility);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasNoOtherFacilities_ShouldNotHaveMultipleFacilityFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();

            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);
            A.CallTo(() => aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf)).Returns(false);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().NotHaveFlag(CanApprovalDateBeChangedFlags.HasMultipleFacility);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasStartedReturn_ShouldHaveStartedReturnFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var startedReturn = A.Fake<Return>();

            A.CallTo(() => startedReturn.ReturnStatus).Returns(ReturnStatus.Created);
            A.CallTo(() => startedReturn.Quarter).Returns(new Quarter(2019, QuarterType.Q1));

            var returns = new List<Return>()
            {
                startedReturn
            };

            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(returns);
            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().HaveFlag(CanApprovalDateBeChangedFlags.HasStartedReturn);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasSubmittedReturn_ShouldHaveSubmittedReturnFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var startedReturn = A.Fake<Return>();

            A.CallTo(() => startedReturn.ReturnStatus).Returns(ReturnStatus.Submitted);
            A.CallTo(() => startedReturn.Quarter).Returns(new Quarter(2019, QuarterType.Q1));

            var returns = new List<Return>()
            {
                startedReturn
            };

            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(returns);
            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().HaveFlag(CanApprovalDateBeChangedFlags.HasSubmittedReturn);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasResubmissionReturn_ShouldHaveReSubmissionReturnFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var resubmission = A.Fake<Return>();

            A.CallTo(() => resubmission.ReturnStatus).Returns(ReturnStatus.Submitted);
            A.CallTo(() => resubmission.Quarter).Returns(new Quarter(2019, QuarterType.Q1));
            A.CallTo(() => resubmission.ParentId).Returns(Guid.NewGuid());

            var returns = new List<Return>()
            {
                resubmission
            };

            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(returns);
            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().HaveFlag(CanApprovalDateBeChangedFlags.HasResubmission);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasNotStartedReturn_ShouldNotHaveStartedReturnFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var startedReturn = A.Fake<Return>();

            A.CallTo(() => startedReturn.ReturnStatus).Returns(ReturnStatus.Created);
            A.CallTo(() => startedReturn.Quarter).Returns(new Quarter(2019, QuarterType.Q2));

            var returns = new List<Return>()
            {
                startedReturn
            };

            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(returns);
            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().NotHaveFlag(CanApprovalDateBeChangedFlags.HasStartedReturn);
        }

        [Fact]
        public async Task Validate_GivenAatfOrganisationHasNotSubmittedReturn_ShouldNotHaveSubmittedReturnFlag()
        {
            var currentApprovalDate = fixture.Create<DateTime>();
            var newApprovalDate = fixture.Create<DateTime>();
            var aatf = A.Fake<Aatf>();
            var organisation = A.Fake<Organisation>();
            var startedReturn = A.Fake<Return>();

            A.CallTo(() => startedReturn.ReturnStatus).Returns(ReturnStatus.Submitted);
            A.CallTo(() => startedReturn.Quarter).Returns(new Quarter(2019, QuarterType.Q2));

            var returns = new List<Return>()
            {
                startedReturn
            };

            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => organisationDataAccess.GetReturnsByComplianceYear(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(returns);
            SetupApprovalDateMovedToNextQuarter(aatf, currentApprovalDate, newApprovalDate);

            var result = await getAatfApprovalDateChangeStatus.Validate(aatf, newApprovalDate);

            result.Should().NotHaveFlag(CanApprovalDateBeChangedFlags.HasSubmittedReturn);
        }

        private void SetupApprovalDateMovedToNextQuarter(Aatf aatf, DateTime currentApprovalDate, DateTime newApprovalDate)
        {
            A.CallTo(() => aatf.ApprovalDate).Returns(currentApprovalDate);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(currentApprovalDate)).Returns(QuarterType.Q1);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(newApprovalDate)).Returns(QuarterType.Q2);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarter(A<Quarter>.That.Matches(x =>
                x.Q.Equals(QuarterType.Q1) && x.Year.Equals(currentApprovalDate.Year)))).Returns(quarterWindow);
        }
    }
}
