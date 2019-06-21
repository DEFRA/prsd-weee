namespace EA.Weee.RequestHandlers.Tests.Unit.Organisations
{
    using EA.Weee.Security;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Organisations;
    using DataAccess;
    using Domain.Lookup;
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

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Domain.Scheme.Scheme>()));
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

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
            A.CallTo(() => context.Organisations).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Organisation>
            {
                GetOrganisationWithId(organisationId)
            }));

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            Assert.NotNull(expectedReturnValue);
            Assert.Same(expectedReturnValue, result);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoScheme_OrganisationSchemeIdShouldBeNull()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Domain.Scheme.Scheme>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.SchemeId.Should().BeNull();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasScheme_OrganisationSchemeIdShouldBeSet()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);

            var scheme = A.Fake<Domain.Scheme.Scheme>();
            A.CallTo(() => scheme.Id).Returns(Guid.NewGuid());
            A.CallTo(() => scheme.OrganisationId).Returns(organisation.Id);

            var schemes = new List<Domain.Scheme.Scheme> { scheme };

            A.CallTo(() => context.Schemes).Returns(dbHelper.GetAsyncEnabledDbSet(schemes));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.SchemeId.Should().Be(scheme.Id);
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoAatfs_OrganisationHasAatfsShouldBeFalse()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAatfs.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasNoAes_OrganisationHasAesShouldBeFalse()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>()));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAes.Should().BeFalse();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasAes_OrganisationHasAesShouldBeTrue()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            var ae = A.Fake<Aatf>();

            A.CallTo(() => ae.FacilityType).Returns(FacilityType.Ae);
            A.CallTo(() => ae.Organisation).Returns(organisation);
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>() { ae }));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAes.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_GivenOrganisationHasAatfs_OrganisationHasAatfsShouldBeTrue()
        {
            var organisation = GetOrganisationWithId(organisationId);

            var expectedReturnValue = new OrganisationData();

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.FacilityType).Returns(FacilityType.Aatf);
            A.CallTo(() => aatf.Organisation).Returns(organisation);
            A.CallTo(() => map.Map(A<Organisation>._)).Returns(expectedReturnValue);
            A.CallTo(() => context.Aatfs).Returns(dbHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var message = new GetOrganisationInfo(organisationId);

            var result = await handler.HandleAsync(message);

            result.HasAatfs.Should().BeTrue();
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
        public async Task OrganisationByIdHandler_GivenMappedOrganisation_MappedOrganisationShouldBeReturned()
        {
            var message = new GetOrganisationInfo(organisationId);

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

        private Aatf CreateAatf(Organisation organisation)
        {
            return new Aatf("name", A.Dummy<UKCompetentAuthority>(), "number", A.Dummy<AatfStatus>(), organisation, A.Dummy<AatfAddress>(), A.Dummy<AatfSize>(), DateTime.Now, A.Dummy<AatfContact>(), A.Dummy<FacilityType>(), (Int16)2019, A.Fake<LocalArea>(), A.Fake<PanArea>());
        }
    }
}
