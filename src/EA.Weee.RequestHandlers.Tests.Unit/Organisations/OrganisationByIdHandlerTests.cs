namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using EA.Weee.Security;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using FakeItEasy;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.Organisations;
    using Requests.Organisations;
    using Weee.Tests.Core;
    using Xunit;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationByIdHandlerTests
    {
        private readonly WeeeContext context;
        private readonly IMap<Organisation, OrganisationData> map;
        private readonly DbContextHelper dbHelper = new DbContextHelper();
        private readonly OrganisationByIdHandler handler;
        private readonly Guid organisationId;

        public OrganisationByIdHandlerTests()
        {
            map = A.Fake<IMap<Organisation, OrganisationData>>();
            context = A.Fake<WeeeContext>();
            organisationId = Guid.NewGuid();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            handler = new OrganisationByIdHandler(AuthorizationBuilder.CreateUserAllowedToAccessOrganisation(),
                context,
                map);
        }

        [Fact]
        public async Task OrganisationByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new OrganisationByIdHandler(authorization, context, map);
            var message = new GetOrganisationInfo(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task OrganisationByIdHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new OrganisationByIdHandler(authorization, context, map);
            var message = new GetOrganisationInfo(organisationId);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.Contains(organisationId.ToString()));
            Assert.True(exception.Message.ToUpperInvariant().Contains("COULD NOT FIND"));
            Assert.True(exception.Message.ToUpperInvariant().Contains("ORGANISATION"));
        }

        [Fact]
        public async Task OrganisationByIdHandler_HappyPath_ReturnsOrganisationFromId()
        {
            var expectedReturnValue = new OrganisationData();
            Organisation mappedOrganisation = null;
            
            A.CallTo(() => map.Map(A<Organisation>._))
                .Invokes((Organisation o) => mappedOrganisation = o)
                .Returns(expectedReturnValue);

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(mappedOrganisation);
            Assert.Equal(organisationId, mappedOrganisation.Id);
            Assert.Same(expectedReturnValue, result);
        }

        [Fact]
        public async Task OrganisationByIdHandler_ReturnsFalseForCanEditOrganisation_WhenCurrentUserIsNotInternalAdmin()
        {
            var weeeAuthorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var handler = new OrganisationByIdHandler(weeeAuthorization, context, map);

            var message = new GetOrganisationInfo(organisationId);

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

            var message = new GetOrganisationInfo(organisationId);

            var handler = new OrganisationByIdHandler(weeeAuthorization, context, map);

            var result = await handler.HandleAsync(message);

            result.CanEditOrganisation.Should().BeTrue();
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
