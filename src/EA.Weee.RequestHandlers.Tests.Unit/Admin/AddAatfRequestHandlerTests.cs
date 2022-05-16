namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using Domain;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.DataAccess.Identity;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.AspNet.Identity;
    using RequestHandlers.Shared;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Xunit;

    public class AddAatfRequestHandlerTests
    {
        private readonly IGenericDataAccess dataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IMap<AatfContactData, AatfContact> contactMapper;
        private readonly ICommonDataAccess commonDataAccess;

        private readonly AddAatfRequestHandler handler;

        public AddAatfRequestHandlerTests()
        {
            addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            contactMapper = A.Fake<IMap<AatfContactData, AatfContact>>();
            dataAccess = A.Fake<IGenericDataAccess>();
            commonDataAccess = A.Fake<ICommonDataAccess>();

            handler = new AddAatfRequestHandler(AuthorizationBuilder.CreateUserWithAllRights(), dataAccess, addressMapper, contactMapper, commonDataAccess);
        }

        [Theory]
        [Trait("Authorization", "Internal")]
        [InlineData(AuthorizationBuilder.UserType.Unauthenticated)]
        [InlineData(AuthorizationBuilder.UserType.External)]
        public async Task HandleAsync_WithNonInternalAccess_ThrowsSecurityException(AuthorizationBuilder.UserType userType)
        {
            var authorization = AuthorizationBuilder.CreateFromUserType(userType);
            var userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new AddAatfRequestHandler(authorization, dataAccess, addressMapper, contactMapper, commonDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_WithNonInternalAdminRole_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder()
                .AllowInternalAreaAccess()
                .DenyRole(Roles.InternalAdmin)
                .Build();

            var userManager = A.Fake<UserManager<ApplicationUser>>();

            var handler = new AddAatfRequestHandler(authorization, this.dataAccess, addressMapper, contactMapper, commonDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<AddAatf>());

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_WithNoLocalArea_LocalAreaIsNull(Core.AatfReturn.FacilityType facilityType)
        {
            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid()
            };

            var result = await handler.HandleAsync(request);

            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(A<Guid>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_WithNoPanArea_PanAreaIsNull(Core.AatfReturn.FacilityType facilityType)
        {
            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                null, A.Dummy<Core.Admin.LocalAreaData>())
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid()
            };

            var result = await handler.HandleAsync(request);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(A<Guid>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_ValidInput_AddsAatf(Core.AatfReturn.FacilityType facilityType)
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var localarea = A.Fake<LocalArea>();
            var panarea = A.Fake<PanArea>();

            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), A.Dummy<Core.Admin.LocalAreaData>())
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid()
            };

            var expectedFacilityType = facilityType.ToDomainEnumeration<Domain.AatfReturn.FacilityType>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(aatf.CompetentAuthority.Abbreviation)).Returns(competentAuthority);
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(aatf.LocalAreaData.Id)).Returns(localarea);
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(aatf.PanAreaData.Id)).Returns(panarea);

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(
                c => c.Name == aatf.Name
                && c.ApprovalNumber == aatf.ApprovalNumber
                && c.CompetentAuthority.Equals(competentAuthority)
                && c.LocalArea.Equals(localarea)
                && c.PanArea.Equals(panarea)
                && c.Name == aatf.Name
                && c.SiteAddress.Id == aatf.SiteAddress.Id
                && Enumeration.FromValue<Domain.AatfReturn.AatfSize>(c.Size.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfSize>(aatf.Size.Value)
                && Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(c.AatfStatus.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(aatf.AatfStatus.Value)
                && c.ApprovalDate == aatf.ApprovalDate
                && c.ComplianceYear == aatf.ComplianceYear
                && c.FacilityType == expectedFacilityType))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(true);
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_CopyAatf_WithNoLocalArea_LocalAreaIsNull(Core.AatfReturn.FacilityType facilityType)
        {
            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), null)
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid(),
                AatfId = aatfId
            };

            var result = await handler.HandleAsync(request);

            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(A<Guid>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_CopyAatf_WithNoPanArea_PanAreaIsNull(Core.AatfReturn.FacilityType facilityType)
        {
            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                null, A.Dummy<Core.Admin.LocalAreaData>())
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid(),
                AatfId = aatfId
            };

            var result = await handler.HandleAsync(request);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(A<Guid>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(Core.AatfReturn.FacilityType.Ae)]
        [InlineData(Core.AatfReturn.FacilityType.Aatf)]
        public async Task HandleAsync_CopyAatf_ValidInput_AddsAatf(Core.AatfReturn.FacilityType facilityType)
        {
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var localarea = A.Fake<LocalArea>();
            var panarea = A.Fake<PanArea>();

            var aatf = new AatfData(Guid.NewGuid(), "name", "approval number", 2019, A.Dummy<Core.Shared.UKCompetentAuthorityData>(),
                Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now,
                A.Dummy<Core.Shared.PanAreaData>(), A.Dummy<Core.Admin.LocalAreaData>())
            {
                FacilityType = facilityType
            };

            var aatfId = Guid.NewGuid();

            var request = new AddAatf()
            {
                Aatf = aatf,
                AatfContact = A.Dummy<AatfContactData>(),
                OrganisationId = Guid.NewGuid(),
                AatfId = aatfId
            };

            var expectedFacilityType = facilityType.ToDomainEnumeration<Domain.AatfReturn.FacilityType>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(aatf.CompetentAuthority.Abbreviation)).Returns(competentAuthority);
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(aatf.LocalAreaData.Id)).Returns(localarea);
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(aatf.PanAreaData.Id)).Returns(panarea);

            var result = await handler.HandleAsync(request);

            A.CallTo(() => dataAccess.Add<Domain.AatfReturn.Aatf>(A<Domain.AatfReturn.Aatf>.That.Matches(
                c => c.Name == aatf.Name
                    && c.ApprovalNumber == aatf.ApprovalNumber
                    && c.CompetentAuthority.Equals(competentAuthority)
                    && c.LocalArea.Equals(localarea)
                    && c.PanArea.Equals(panarea)
                    && c.Name == aatf.Name
                    && c.SiteAddress.Id == aatf.SiteAddress.Id
                    && Enumeration.FromValue<Domain.AatfReturn.AatfSize>(c.Size.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfSize>(aatf.Size.Value)
                    && Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(c.AatfStatus.Value) == Enumeration.FromValue<Domain.AatfReturn.AatfStatus>(aatf.AatfStatus.Value)
                    && c.ApprovalDate == aatf.ApprovalDate
                    && c.ComplianceYear == aatf.ComplianceYear
                    && c.FacilityType == expectedFacilityType
                    && c.AatfId == aatfId))).MustHaveHappened(1, Times.Exactly);

            result.Should().Be(true);
        }
    }
}