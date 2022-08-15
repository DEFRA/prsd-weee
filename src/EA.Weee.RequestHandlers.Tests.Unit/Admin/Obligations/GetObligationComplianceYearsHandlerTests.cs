namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared;
    using DataAccess.DataAccess;
    using Domain;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Requests.Admin.Obligations;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;

    public class GetObligationComplianceYearsHandlerTests : SimpleUnitTestBase
    {
        private GetObligationComplianceYearsHandler handler;
        private readonly GetObligationComplianceYears request;
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetObligationComplianceYearsHandlerTests()
        {
            authorization = A.Fake<IWeeeAuthorization>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            obligationDataAccess = A.Fake<IObligationDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();

            request = new GetObligationComplianceYears(TestFixture.Create<CompetentAuthority>(), false);

            handler = new GetObligationComplianceYearsHandler(authorization, obligationDataAccess, commonDataAccess, systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetObligationComplianceYearsHandler(authorization, obligationDataAccess, commonDataAccess, systemDataDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NotAnAdminUser_ThrowsSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyAnyRole().Build();

            handler = new GetObligationComplianceYearsHandler(authorization, obligationDataAccess, commonDataAccess, systemDataDataAccess);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_InternalAccess_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_UserInAdminRole_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureUserInRole(Roles.InternalAdmin)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_CompetentAuthorityShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.Authority.Value)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_SystemDataTimeShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ObligationComplianceYearsShouldBeRetrieved()
        {
            //arrange
            var authority = TestFixture.Create<UKCompetentAuthority>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._))
                .Returns(authority);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.GetObligationComplianceYears(A<UKCompetentAuthority>.That.IsSameAs(authority))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationYears_YearsShouldBeReturned()
        {
            //arrange
            var years = TestFixture.CreateMany<int>().ToList();

            A.CallTo(() => obligationDataAccess.GetObligationComplianceYears(A<UKCompetentAuthority>._)).Returns(years);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeEquivalentTo(years);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationYears_OrderedYearsShouldBeReturned()
        {
            //arrange
            var years = new List<int>() { 2021, 2019, 2022, 2018 };

            A.CallTo(() => obligationDataAccess.GetObligationComplianceYears(A<UKCompetentAuthority>._)).Returns(years);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeInDescendingOrder(r => r);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationYearsAndCurrentYearIsNotInList_YearsShouldBeReturned()
        {
            //arrange
            var years = new List<int>() { 2020, 2019 };
            var currentDate = new DateTime(2021, 1, 1);

            A.CallTo(() => obligationDataAccess.GetObligationComplianceYears(A<UKCompetentAuthority>._)).Returns(years);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = new GetObligationComplianceYears(TestFixture.Create<CompetentAuthority>(), true);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeEquivalentTo(new List<int>() { 2021, 2020, 2019 });
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndObligationYearsAndCurrentYearIsNotInListAndIsNotRequestedToBe_YearsShouldBeReturned()
        {
            //arrange
            var years = new List<int>() { 2020, 2019 };
            var currentDate = new DateTime(2021, 1, 1);

            A.CallTo(() => obligationDataAccess.GetObligationComplianceYears(A<UKCompetentAuthority>._)).Returns(years);
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(currentDate);

            var request = new GetObligationComplianceYears(TestFixture.Create<CompetentAuthority>(), false);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeEquivalentTo(new List<int>() { 2020, 2019 });
        }
    }
}
