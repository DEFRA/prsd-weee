namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using AutoFixture;
    using Core.Admin;
    using Domain.AatfReturn;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Aatf;
    using Requests.Admin.Aatf;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using RequestHandlers.Aatf;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class CheckAatfCanBeDeletedHandlerTests
    {
        private readonly IGetAatfDeletionStatus aatfDeletionStatus;
        private readonly IGetOrganisationDeletionStatus organisationDeletionStatus;
        private readonly CheckAatfCanBeDeletedHandler handler;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly Fixture fixture;

        public CheckAatfCanBeDeletedHandlerTests()
        {
            aatfDeletionStatus = A.Fake<IGetAatfDeletionStatus>();
            organisationDeletionStatus = A.Fake<IGetOrganisationDeletionStatus>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            fixture = new Fixture();

            handler = new CheckAatfCanBeDeletedHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                aatfDeletionStatus,
                organisationDeletionStatus,
                aatfDataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var handler = new CheckAatfCanBeDeletedHandler(AuthorizationBuilder.CreateFromUserType(userType), aatfDeletionStatus, organisationDeletionStatus, aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var handler = new CheckAatfCanBeDeletedHandler(new AuthorizationBuilder().DenyRole(Roles.InternalAdmin).Build(), aatfDeletionStatus, organisationDeletionStatus, aatfDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<CheckAatfCanBeDeleted>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_AatfShouldBeRetrieved()
        {
            var message = fixture.Create<CheckAatfCanBeDeleted>();

            await handler.HandleAsync(message);

            A.CallTo(() => aatfDataAccess.GetDetails(message.AatfId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_AatfDeleteFlagsShouldBeValidated()
        {
            var message = fixture.Create<CheckAatfCanBeDeleted>();
            var aatf = GetAatf(message.AatfId);

            A.CallTo(() => aatfDataAccess.GetDetails(message.AatfId)).Returns(aatf);

            await handler.HandleAsync(message);

            A.CallTo(() => aatfDeletionStatus.Validate(aatf.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_OrganisationDeleteFlagsShouldBeValidated()
        {
            var message = fixture.Create<CheckAatfCanBeDeleted>();
            var aatf = GetAatf(message.AatfId);

            A.CallTo(() => aatfDataAccess.GetDetails(message.AatfId)).Returns(aatf);

            await handler.HandleAsync(message);

            A.CallTo(() => organisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_ReturnObjectShouldContainValidatedFlags()
        {
            var message = fixture.Create<CheckAatfCanBeDeleted>();
            var aatf = GetAatf(message.AatfId);

            var aatfFlags = fixture.Create<CanAatfBeDeletedFlags>();
            var organisationFlags = fixture.Create<CanOrganisationBeDeletedFlags>();

            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(aatf);
            A.CallTo(() => organisationDeletionStatus.Validate(aatf.Organisation.Id, aatf.ComplianceYear, aatf.FacilityType)).Returns(organisationFlags);
            A.CallTo(() => aatfDeletionStatus.Validate(aatf.Id)).Returns(aatfFlags);

            var result = await handler.HandleAsync(message);

            result.CanOrganisationBeDeletedFlags.Should().Be(organisationFlags);
            result.CanAatfBeDeletedFlags.Should().Be(aatfFlags);
        }

        private Aatf GetAatf(Guid id)
        {
            var organisation = Organisation.CreatePartnership("trading");

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.Id).Returns(id);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);

            return aatf;
        }
    }
}
