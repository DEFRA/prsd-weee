namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationInternalByIdHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> map;
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private readonly OrganisationInternalByIdHandler handler;
        private readonly Guid organisationId;

        public OrganisationInternalByIdHandlerTests()
        {
            map = A.Fake<IMap<Organisation, OrganisationData>>();
            context = A.Fake<WeeeContext>();
            organisationId = Guid.NewGuid();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            handler = new OrganisationInternalByIdHandler(AuthorizationBuilder.CreateUserWithAllRights(),
                context,
                map);
        }

        [Fact]
        public async Task OrganisationByIdHandler_NotInternalUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateFromUserType(AuthorizationBuilder.UserType.External);

            var handler = new OrganisationInternalByIdHandler(authorization, context, map);
            var message = new GetInternalOrganisation(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task OrganisationByIdHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var message = new GetInternalOrganisation(organisationId);

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.Contains(organisationId.ToString()));
            Assert.True(exception.Message.ToUpperInvariant().Contains("COULD NOT FIND"));
            Assert.True(exception.Message.ToUpperInvariant().Contains("ORGANISATION"));
        }

        [Fact]
        public async Task OrganisationByIdHandler_HappyPath_ReturnsOrganisationFromId()
        {
            var authorization = AuthorizationBuilder.CreateUserWithAllRights();

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var message = new GetInternalOrganisation(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(expectedReturnValue);
            Assert.Same(expectedReturnValue, result);
        }

        [Fact]
        public async Task OrganisationByIdHandler_ReturnsFalseForCanEditOrganisation_WhenCurrentUserIsNotInternalAdmin()
        {
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new OrganisationInternalByIdHandler(weeeAuthorization, context, map);

            var message = new GetInternalOrganisation(organisationId);

            var result = await handler.HandleAsync(message);

            result.CanEditOrganisation.Should().BeFalse();
        }

        [Fact]
        public async Task OrganisationByIdHandler_ReturnsTrueForCanEditOrganisation_WhenCurrentUserIsInternalAdmin()
        {
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .AllowRole(Roles.InternalAdmin)
                .Build();

            var message = new GetInternalOrganisation(organisationId);

            var handler = new OrganisationInternalByIdHandler(weeeAuthorization, context, map);

            var result = await handler.HandleAsync(message);

            result.CanEditOrganisation.Should().BeTrue();
        }

        [Fact]
        public async Task OrganisationByIdHandler_GivenMappedOrganisation_MappedOrganisationShouldBeReturned()
        {
            var message = new GetInternalOrganisation(organisationId);

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var result = await handler.HandleAsync(message);

            result.Should().Be(expectedReturnValue);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
