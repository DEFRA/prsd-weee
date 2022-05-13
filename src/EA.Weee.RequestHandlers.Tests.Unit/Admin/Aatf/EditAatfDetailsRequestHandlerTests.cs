namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Aatf
{
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Lookup;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Admin.Aatf;
    using RequestHandlers.Factories;
    using RequestHandlers.Organisations;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Admin.Aatf;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class EditAatfDetailsRequestHandlerTests
    {
        private readonly Fixture fixture;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMap<AatfAddressData, AatfAddress> addressMapper;
        private readonly IOrganisationDetailsDataAccess organisationDetailsDataAccess;
        private readonly EditAatfDetailsRequestHandler handler;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IGetAatfApprovalDateChangeStatus getAatfApprovalDateChangeStatus;
        private readonly IQuarterWindowFactory quarterWindowFactory;
        private readonly IWeeeTransactionAdapter context;

        public EditAatfDetailsRequestHandlerTests()
        {
            fixture = new Fixture();
            var authorization = A.Fake<IWeeeAuthorization>();
            genericDataAccess = A.Fake<IGenericDataAccess>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            addressMapper = A.Fake<IMap<AatfAddressData, AatfAddress>>();
            organisationDetailsDataAccess = A.Fake<IOrganisationDetailsDataAccess>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            getAatfApprovalDateChangeStatus = A.Fake<IGetAatfApprovalDateChangeStatus>();
            quarterWindowFactory = A.Fake<IQuarterWindowFactory>();
            context = A.Fake<IWeeeTransactionAdapter>();

            handler = new EditAatfDetailsRequestHandler(authorization,
                aatfDataAccess,
                genericDataAccess,
                addressMapper,
                organisationDetailsDataAccess,
                commonDataAccess,
                getAatfApprovalDateChangeStatus,
                quarterWindowFactory,
                context);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new EditAatfDetailsRequestHandler(authorization,
                aatfDataAccess,
                genericDataAccess,
                addressMapper,
                organisationDetailsDataAccess,
                commonDataAccess,
                getAatfApprovalDateChangeStatus,
                quarterWindowFactory,
                context);

            Func<Task> action = async () => await handler.HandleAsync(A.Dummy<EditAatfDetails>());

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoAdminRoleAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().DenyRole(Roles.InternalAdmin).Build();

            var handler = new EditAatfDetailsRequestHandler(authorization,
                aatfDataAccess,
                genericDataAccess,
                addressMapper,
                organisationDetailsDataAccess,
                commonDataAccess,
                getAatfApprovalDateChangeStatus,
                quarterWindowFactory,
                context);

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
            var localAreaDomain = A.Fake<LocalArea>();
            var panAreaDomain = A.Fake<PanArea>();

            A.CallTo(() => addressMapper.Map(data.SiteAddress)).Returns(siteAddress);
            A.CallTo(() => aatf.ComplianceYear).Returns((Int16)2019);
            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(aatf);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(updateRequest.Data.CompetentAuthority.Abbreviation)).Returns(competentAuthorityDomain);
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(updateRequest.Data.LocalAreaDataId.Value)).Returns(localAreaDomain);
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(updateRequest.Data.PanAreaDataId.Value)).Returns(panAreaDomain);

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfDataAccess.UpdateDetails(aatf, A<Aatf>.That.Matches(a => a.Name == data.Name &&
                a.CompetentAuthority.Equals(competentAuthorityDomain) &&
                a.LocalArea.Equals(localAreaDomain) &&
                a.PanArea.Equals(panAreaDomain) &&
                a.ApprovalNumber == data.ApprovalNumber &&
                a.AatfStatus == Domain.AatfReturn.AatfStatus.Approved &&
                a.Size == Domain.AatfReturn.AatfSize.Large &&
                a.ApprovalDate == data.ApprovalDate.GetValueOrDefault() &&
                a.ComplianceYear == aatf.ComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenNoLocalArea_LocalAreaIsNull()
        {
            var data = CreateAatfData(out var competentAuthority);
            data.LocalAreaDataId = null;
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(A<Guid>._)).MustNotHaveHappened();
            A.CallTo(() => aatfDataAccess.UpdateDetails(A<Aatf>._, A<Aatf>.That.Matches(a => a.LocalArea == null))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task HandleAsync_GivenNoPanArea_LocalAreaIsNull()
        {
            var data = CreateAatfData(out var competentAuthority);
            data.PanAreaDataId = null;
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(A<Guid>._)).MustNotHaveHappened();
            A.CallTo(() => aatfDataAccess.UpdateDetails(A<Aatf>._, A<Aatf>.That.Matches(a => a.PanArea == null))).MustHaveHappened(1, Times.Exactly);
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

        [Fact]
        public async Task HandleAsync_GivenMessage_AatfApprovalFlagsShouldBeCalculated()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var existingAatf = GetAatf();

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(existingAatf);

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(existingAatf, data.ApprovalDate.Value)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageAndAatfApprovalDateHasChanged_QuartersForDatesShouldBeRetrieved()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var existingAatf = GetAatf();
            var flags = new CanApprovalDateBeChangedFlags();
            flags |= CanApprovalDateBeChangedFlags.DateChanged;

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(existingAatf);
            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(existingAatf, data.ApprovalDate.Value)).Returns(flags);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(A<DateTime>._)).ReturnsNextFromSequence(new QuarterType[] { QuarterType.Q1, QuarterType.Q2 });

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(existingAatf.ApprovalDate.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(data.ApprovalDate.Value)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageAndAatfApprovalDateHasNotChanged_AatfDataShouldNotBeRemoved()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var existingAatf = GetAatf();
            var flags = new CanApprovalDateBeChangedFlags();

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(existingAatf);
            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(existingAatf, data.ApprovalDate.Value)).Returns(flags);

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfDataAccess.RemoveAatfData(A<Aatf>._, A<IEnumerable<int>>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task HandleAsync_GivenMessageAndAatfApprovalDateHasChanged_AatfDataShouldBeRemoved()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var existingAatf = GetAatf();
            var flags = new CanApprovalDateBeChangedFlags();
            flags |= CanApprovalDateBeChangedFlags.DateChanged;

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).Returns(existingAatf);
            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(existingAatf, data.ApprovalDate.Value)).Returns(flags);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(existingAatf.ApprovalDate.Value)).ReturnsNextFromSequence(new QuarterType[] { QuarterType.Q1, QuarterType.Q2 });
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(data.ApprovalDate.Value)).Returns(QuarterType.Q4);

            var result = await handler.HandleAsync(updateRequest);

            var range = Enumerable.Range(1, 3);

            A.CallTo(() => aatfDataAccess.RemoveAatfData(existingAatf, A<IEnumerable<int>>.That.IsSameSequenceAs(range)))
                .MustHaveHappenedOnceExactly();
        }
        [Fact]
        public async Task HandleAsync_GivenMessageAndAatfApprovalDateHasChanged_AatfDataShouldBeRemovedBeforeAatfIsUpdated()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();
            var existingAatf = GetAatf();
            var flags = new CanApprovalDateBeChangedFlags();
            flags |= CanApprovalDateBeChangedFlags.DateChanged;

            A.CallTo(() => genericDataAccess.GetById<Aatf>(A<Guid>._)).Returns(existingAatf);
            A.CallTo(() => getAatfApprovalDateChangeStatus.Validate(A<Aatf>._, A<DateTime>._)).Returns(flags);
            A.CallTo(() => quarterWindowFactory.GetAnnualQuarterForDate(A<DateTime>._)).ReturnsNextFromSequence(new QuarterType[] { QuarterType.Q1, QuarterType.Q2 });

            var result = await handler.HandleAsync(updateRequest);

            A.CallTo(() => aatfDataAccess.RemoveAatfData(existingAatf, A<IEnumerable<int>>._))
                .MustHaveHappenedOnceExactly().Then(A.CallTo(() => aatfDataAccess.UpdateDetails(A<Aatf>._, A<Aatf>._)).MustHaveHappenedOnceExactly());
        }

        [Fact]
        public async Task HandleAsync_GivenMessage_ExistingAatfShouldBeRetrieved()
        {
            var data = CreateAatfData(out var competentAuthority);
            var updateRequest = fixture.Build<EditAatfDetails>().With(e => e.Data, data).Create();

            await handler.HandleAsync(updateRequest);

            A.CallTo(() => genericDataAccess.GetById<Aatf>(updateRequest.Data.Id)).MustHaveHappenedOnceExactly();
        }

        private AatfData CreateAatfData(out UKCompetentAuthorityData competentAuthority)
        {
            competentAuthority = fixture.Create<UKCompetentAuthorityData>();
            var localArea = fixture.Create<LocalAreaData>();
            var panAreaData = fixture.Create<PanAreaData>();

            var data = fixture.Build<AatfData>()
                .With(e => e.CompetentAuthority, competentAuthority)
                .With(e => e.AatfStatus, Core.AatfReturn.AatfStatus.Approved)
                .With(e => e.Size, Core.AatfReturn.AatfSize.Large)
                .With(e => e.LocalAreaData, localArea)
                .With(e => e.PanAreaData, panAreaData)
                .With(e => e.AatfSizeValue, Core.AatfReturn.AatfSize.Large.Value)
                .With(e => e.AatfStatusValue, Core.AatfReturn.AatfStatus.Approved.Value)
                .Create();

            return data;
        }

        private Aatf GetAatf()
        {
            var organisation = Organisation.CreatePartnership("trading");

            var aatf = A.Fake<Aatf>();

            A.CallTo(() => aatf.ApprovalDate).Returns(new DateTime(2019, 1, 1));
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            return aatf;
        }
    }
}
