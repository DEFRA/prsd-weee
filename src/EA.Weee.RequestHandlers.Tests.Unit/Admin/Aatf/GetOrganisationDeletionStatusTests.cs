namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using System;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using Xunit;

    public class GetOrganisationDeletionStatusTests
    {
        private readonly IOrganisationDataAccess organisationDataAccess;
        private readonly GetOrganisationDeletionStatus getOrganisationDeletionStatus;
        private const int ComplianceYear = 2019;
        private readonly Guid organisationId = Guid.NewGuid();

        public GetOrganisationDeletionStatusTests()
        {
            organisationDataAccess = A.Fake<IOrganisationDataAccess>();

            getOrganisationDeletionStatus = new GetOrganisationDeletionStatus(organisationDataAccess);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Validate_GivenHasReturnsCheck_FlagsShouldBeValid(bool hasReturns)
        {
            A.CallTo(() => organisationDataAccess.HasReturns(organisationId, ComplianceYear)).Returns(hasReturns);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            if (hasReturns)
            {
                result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasReturns);
            }
            else
            {
                result.Should().NotHaveFlag(CanOrganisationBeDeletedFlags.HasReturns);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Validate_GivenHasActiveUsersCheck_FlagsShouldBeValid(bool hasActiveUsers)
        {
            A.CallTo(() => organisationDataAccess.HasActiveUsers(organisationId)).Returns(hasActiveUsers);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            if (hasActiveUsers)
            {
                result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasActiveUsers);
            }
            else
            {
                result.Should().NotHaveFlag(CanOrganisationBeDeletedFlags.HasActiveUsers);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Validate_GivenHasSchemeCheck_FlagsShouldBeValid(bool hasScheme)
        {
            A.CallTo(() => organisationDataAccess.HasScheme(organisationId)).Returns(hasScheme);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            if (hasScheme)
            {
                result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasScheme);
            }
            else
            {
                result.Should().NotHaveFlag(CanOrganisationBeDeletedFlags.HasScheme);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Validate_GivenHasFacilityAeCheck_FlagsShouldBeValid(bool hasAe)
        {
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Ae)).Returns(hasAe);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            if (hasAe)
            {
                result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasAe);
            }
            else
            {
                result.Should().NotHaveFlag(CanOrganisationBeDeletedFlags.HasAe);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Validate_GivenHasFacilityaatfCheck_FlagsShouldBeValid(bool hasAatf)
        {
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Aatf)).Returns(hasAatf);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            if (hasAatf)
            {
                result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasAatf);
            }
            else
            {
                result.Should().NotHaveFlag(CanOrganisationBeDeletedFlags.HasAatf);
            }
        }

        [Fact]
        public async Task Validate_GivenFalseForAllChecks_FlagsShouldBeEmpty()
        {
            A.CallTo(() => organisationDataAccess.HasReturns(organisationId, ComplianceYear)).Returns(false);
            A.CallTo(() => organisationDataAccess.HasActiveUsers(organisationId)).Returns(false);
            A.CallTo(() => organisationDataAccess.HasScheme(organisationId)).Returns(false);
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Aatf)).Returns(false);
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Ae)).Returns(false);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            result.Should().Be(0);
        }

        [Fact]
        public async Task Validate_GivenTrueForAllChecks_ShouldContainAllFlags()
        {
            A.CallTo(() => organisationDataAccess.HasReturns(organisationId, ComplianceYear)).Returns(true);
            A.CallTo(() => organisationDataAccess.HasActiveUsers(organisationId)).Returns(true);
            A.CallTo(() => organisationDataAccess.HasScheme(organisationId)).Returns(true);
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Aatf)).Returns(true);
            A.CallTo(() => organisationDataAccess.HasFacility(organisationId, FacilityType.Ae)).Returns(true);

            var result = await getOrganisationDeletionStatus.Validate(organisationId, ComplianceYear);

            result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasScheme);
            result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasActiveUsers);
            result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasActiveUsers);
            result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasAe);
            result.Should().HaveFlag(CanOrganisationBeDeletedFlags.HasAatf);
        }
    }
}
