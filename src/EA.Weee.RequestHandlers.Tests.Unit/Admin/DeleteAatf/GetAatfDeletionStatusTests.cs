namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.DeleteAatf
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Admin;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Admin.DeleteAatf.DeleteValidation;
    using Xunit;

    public class GetAatfDeletionStatusTests
    {
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IGetOrganisationDeletionStatus getOrganisationDeletionStatus;
        private readonly GetAatfDeletionStatus getAatfDeletionStatus;
        private readonly Fixture fixture;

        public GetAatfDeletionStatusTests()
        {
            fixture = new Fixture();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            getOrganisationDeletionStatus = A.Fake<IGetOrganisationDeletionStatus>();

            getAatfDeletionStatus = new GetAatfDeletionStatus(aatfDataAccess, getOrganisationDeletionStatus);
        }

        [Theory]
        [InlineData(false, false, true, true, true, false, false)]
        [InlineData(false, false, true, true, false, true, false)]
        [InlineData(false, false, true, false, true, false, false)]
        [InlineData(false, false, true, false, false, true, false)]
        [InlineData(false, false, false, true, true, false, false)]
        [InlineData(false, false, false, true, false, false, false)]
        [InlineData(false, false, false, false, true, false, false)]
        [InlineData(false, false, false, false, false, true, true)]
        [InlineData(false, true, false, false, false, true, false)]
        [InlineData(true, false, false, false, false, true, false)]
        public async Task Validate_GivenScenario_ExpectedFlagsShouldBePresent(bool organisationHasOtherFacilityType, bool organisationHasAssociatedScheme,
            bool organisationHasOtherEntityOfSameType, bool organisationHasReturns, bool aatfHasReturnData, bool expectedToDeleteAatf, bool expectedToDeleteOrganisation)
        {
            var organisation = Organisation.CreatePartnership("trading");
            var aatfId = fixture.Create<Guid>();
            var aatf = A.Fake<Aatf>();
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);
            A.CallTo(() => aatf.Id).Returns(aatfId);

            var canOrganisationBeDeletedFlags = new CanOrganisationBeDeletedFlags();

            if (organisationHasAssociatedScheme)
            {
                canOrganisationBeDeletedFlags |= CanOrganisationBeDeletedFlags.HasScheme;
            }

            if (organisationHasReturns)
            {
                canOrganisationBeDeletedFlags |= CanOrganisationBeDeletedFlags.HasReturns;
            }

            if (organisationHasOtherFacilityType)
            {
                canOrganisationBeDeletedFlags |= CanOrganisationBeDeletedFlags.HasAe;
            }

            A.CallTo(() => aatfDataAccess.GetDetails(aatfId)).Returns(aatf);
            A.CallTo(() => aatfDataAccess.HasAatfData(aatfId)).Returns(aatfHasReturnData);
            A.CallTo(() => getOrganisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear)).Returns(canOrganisationBeDeletedFlags);
            A.CallTo(() => aatfDataAccess.HasAatfOrganisationOtherAeOrAatf(aatf)).Returns(organisationHasOtherEntityOfSameType);

            var result = await getAatfDeletionStatus.Validate(aatfId);

            if (expectedToDeleteAatf)
            {
                result.Should().HaveFlag(CanAatfBeDeletedFlags.CanDelete);
            }
            else
            {
                result.Should().NotHaveFlag(CanAatfBeDeletedFlags.CanDelete);
            }

            if (expectedToDeleteOrganisation)
            {
                result.Should().HaveFlag(CanAatfBeDeletedFlags.CanDeleteOrganisation);
            }
            else
            {
                result.Should().NotHaveFlag(CanAatfBeDeletedFlags.CanDeleteOrganisation);
            }
        }
    }
}
