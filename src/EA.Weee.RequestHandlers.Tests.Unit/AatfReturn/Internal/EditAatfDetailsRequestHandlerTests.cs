namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Internal
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Shared;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Organisations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Security;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Shared;
    using Xunit;

    public class EditAatfDetailsRequestHandlerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditAatfDetailsRequestHandler handler;
        private readonly ICommonDataAccess commonDataAccess;

        public EditAatfDetailsRequestHandlerTests()
        {
            fixture = new Fixture();
            authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            commonDataAccess = A.Fake<ICommonDataAccess>();

            handler = new EditAatfDetailsRequestHandler(authorization, aatfDataAccess, genericDataAccess, addressMapper, organisationDetailsDataAccess, commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new EditAatfDetailsRequestHandler(authorization, aatfDataAccess, genericDataAccess, addressMapper, organisationDetailsDataAccess, commonDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfDetails>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoAdminRoleAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().DenyRole(Roles.InternalAdmin).Build();

            var handler = new EditAatfDetailsRequestHandler(authorization, aatfDataAccess, genericDataAccess, addressMapper, organisationDetailsDataAccess, commonDataAccess);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfDetails>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAddress_MapperIsCalled()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var siteAddress = A.Fake<AatfAddress>();

            A.CallTo(() => addressMapper.Map(data.SiteAddress)).Returns(siteAddress);

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(aatf);

            var result = await handler.HandleAsync(updateRequest);

            Assert.True(result);

            A.CallTo(() => addressMapper.Map(data.SiteAddress)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_DetailsAreUpdatedCorrectly()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var siteAddress = fixture.Create<AatfAddress>();
            var aatf = A.Fake<Aatf>();
            var competentAuthorityDomain = A.Fake<UKCompetentAuthority>();

            A.CallTo(() => addressMapper.Map(data.SiteAddress)).Returns(siteAddress);
            A.CallTo(() => aatf.ComplianceYear).Returns((Int16)2019);
            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(aatf);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(updateRequest.Data.CompetentAuthority.Id)).Returns(competentAuthorityDomain);

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfDataAccess.UpdateDetails(aatf, A<Aatf>.That.Matches(a => a.Name == data.Name &&
                a.CompetentAuthority.Equals(competentAuthorityDomain) &&
                a.ApprovalNumber == data.ApprovalNumber &&
                a.AatfStatus == Domain.AatfReturn.AatfStatus.Approved &&
                a.Size == Domain.AatfReturn.AatfSize.Large &&
                a.ApprovalDate == data.ApprovalDate.GetValueOrDefault() &&
                a.ComplianceYear == aatf.ComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageContainingUpdatedAddress_AddressIsUpdatedCorrectly()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var siteAddress = fixture.Create<AatfAddress>();
            A.CallTo(() => addressMapper.Map(data.SiteAddress)).Returns(siteAddress);

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(aatf);

            var result = await handler.HandleAsync(updateRequest);

            Assert.True(result);

            A.CallTo(() => aatfDataAccess.UpdateAddress(A<AatfAddress>._, siteAddress, A<Country>._)).MustHaveHappenedOnceExactly();
        }
        private AatfData CreateAatfData(out UKCompetentAuthorityData competentAuthority)
        {
            competentAuthority = fixture.Create<UKCompetentAuthorityData>();
            var data = fixture.Build<AatfData>()
                .With(e => e.CompetentAuthority, competentAuthority)
                .With(e => e.AatfStatus, Core.AatfReturn.AatfStatus.Approved)
                .With(e => e.Size, Core.AatfReturn.AatfSize.Large)
                .Create();
            return data;
        }
    }
}
