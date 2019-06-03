namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Organisation;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
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
        private readonly DbContextHelper dbHelper = new DbContextHelper();

        [Fact]
        public async Task OrganisationByIdHandler_NotOrganisationUser_ThrowsSecurityException()
        {
            var authorization = AuthorizationBuilder.CreateUserDeniedFromAccessingOrganisation();

            var handler = new OrganisationByIdHandler(authorization, A.Dummy<WeeeContext>(), A.Dummy<OrganisationMap>());
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

            var handler = new OrganisationByIdHandler(authorization, context, A.Dummy<OrganisationMap>());
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

            List<Aatf> aatfs = new List<Aatf>();
            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme> { new Domain.Scheme.Scheme(organisationId) };

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(schemes));
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(aatfs));

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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task HandleAsync_NoSchemesOrAatfs_OrganisationDataSetCorrectly(bool hasAatfOrScheme)
        {
            var authorization = AuthorizationBuilder.CreateUserAllowedToAccessOrganisation();

            var organisationId = Guid.NewGuid();
            var context = A.Fake<WeeeContext>();
            Organisation organisation = GetOrganisationWithId(organisationId);

            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                organisation
            }));

            OrganisationData expectedReturnValue = new OrganisationData();
            Organisation mappedOrganisation = null;
            var organisationMap = A.Fake<IMap<Organisation, OrganisationData>>();
            A.CallTo(() => organisationMap.Map(A<Organisation>._))
                .Invokes((Organisation o) => mappedOrganisation = o)
                .Returns(expectedReturnValue);

            List<Aatf> aatfs = new List<Aatf>();
            List<Domain.Scheme.Scheme> schemes = new List<Domain.Scheme.Scheme>();

            if (hasAatfOrScheme)
            {
                aatfs.Add(CreateAatf(organisation));
                schemes.Add(new Domain.Scheme.Scheme(organisationId));
            }

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(schemes));
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(aatfs));

            var handler = new OrganisationByIdHandler(authorization, context, organisationMap);
            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.Equal(hasAatfOrScheme, result.HasAatfs);
            Assert.Equal(hasAatfOrScheme, result.SchemeId != null);

            if (hasAatfOrScheme)
            {
                Assert.Equal(schemes.FirstOrDefault().Id, result.SchemeId);
            }
        }

        private Organisation GetOrganisationWithId(Guid id)
        {
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(id);
            return organisation;
        }

        private Aatf CreateAatf(Organisation organisation)
        {
            return new Aatf("name", A.Dummy<UKCompetentAuthority>(), "number", A.Dummy<AatfStatus>(), organisation, A.Dummy<AatfAddress>(), A.Dummy<AatfSize>(), DateTime.Now, A.Dummy<AatfContact>(), A.Dummy<FacilityType>(), (Int16)2019);
        }
    }
}
