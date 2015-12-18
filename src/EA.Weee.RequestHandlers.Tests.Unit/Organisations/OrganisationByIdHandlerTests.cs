namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
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

    public class OrganisationByIdHandlerTests
    {
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        [Fact]
        public async Task OrganisationByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new OrganisationByIdHandler(authorization, A<WeeeContext>._, A<OrganisationMap>._);
            var message = new GetOrganisationInfo(Guid.NewGuid());

            await Assert.ThrowsAsync<SecurityException>(async () => await handler.HandleAsync(message));
        }

        [Fact]
        public async Task OrganisationByIdHandler_NoSuchOrganisation_ThrowsArgumentException()
        {
            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var organisationId = Guid.NewGuid();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>()));

            var handler = new OrganisationByIdHandler(authorization, context, A<OrganisationMap>._);
            var message = new GetOrganisationInfo(organisationId);

            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await handler.HandleAsync(message));

            Assert.True(exception.Message.Contains(organisationId.ToString()));
            Assert.True(exception.Message.ToUpperInvariant().Contains("COULD NOT FIND"));
            Assert.True(exception.Message.ToUpperInvariant().Contains("ORGANISATION"));
        }

        [Fact]
        public async Task OrganisationByIdHandler_HappyPath_ReturnsOrganisationFromId()
        {
            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var organisationId = Guid.NewGuid();
            var context = A.Fake<WeeeContext>();
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            OrganisationData expectedReturnValue = new OrganisationData();
            Organisation mappedOrganisation = null;
            var organisationMap = A.Fake<IMap<Organisation, OrganisationData>>();
            A.CallTo(() => organisationMap.Map(A<Organisation>._))
                .Invokes((Organisation o) => mappedOrganisation = o)
                .Returns(expectedReturnValue);

            var handler = new OrganisationByIdHandler(authorization, context, organisationMap);
            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(mappedOrganisation);
            Assert.Equal(organisationId, mappedOrganisation.Id);
            Assert.Same(expectedReturnValue, result);
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }
    }
}
