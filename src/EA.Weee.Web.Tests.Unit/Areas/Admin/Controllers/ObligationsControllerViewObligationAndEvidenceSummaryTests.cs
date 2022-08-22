namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.Admin.Obligation;
    using Core.Scheme;
    using EA.Weee.Api.Client;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Web.Areas.Admin.ViewModels.Obligations;
    using Weee.Requests.Admin.Obligations;
    using Weee.Tests.Core;
    using Xunit;

    public class ObligationsControllerViewObligationAndEvidenceSummaryTests : SimpleUnitTestBase
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeClient client;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligationsController controller;

        public ObligationsControllerViewObligationAndEvidenceSummaryTests()
        {
            configuration = A.Fake<IAppConfiguration>();
            client = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            apiClient = () => client;
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            controller = new ObligationsController(configuration, breadcrumb, cache, apiClient, mapper);
        }

        [Fact]
        public void ViewObligationAndEvidenceSummaryGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("ViewObligationAndEvidenceSummary", 
                new[] { typeof(int?), typeof(Guid?) }).Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_BreadCrumbShouldBeSet()
        {
            //act
            await controller.ViewObligationAndEvidenceSummary(TestFixture.Create<int?>(), TestFixture.Create<Guid?>());

            //assert
            breadcrumb.InternalActivity.Should().Be("View PCS obligation and evidence summary");
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_ObligationComplianceYearsShouldBeRetrieved()
        {
            //act
            await controller.ViewObligationAndEvidenceSummary(TestFixture.Create<int?>(), TestFixture.Create<Guid?>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetObligationComplianceYears>.That.Matches(g => g.IncludeCurrentYear == false &&
                                                                      g.Authority == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            ViewObligationAndEvidenceSummaryGet_GivenNoSelectedComplianceYearAndEmptyAvailableComplianceYears_GetSchemesWithObligationShouldNotBeCalled()
        {
            //arrange
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(new List<int>());

            //act
            await controller.ViewObligationAndEvidenceSummary(null, TestFixture.Create<Guid?>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligation>._)).MustNotHaveHappened();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task
            ViewObligationAndEvidenceSummaryGet_GivenNoSelectedComplianceYearAndEmptyAvailableComplianceYears_ModelMapperShouldBeCalled()
        {
            //arrange
            var complianceYears = new List<int>();
            var schemeData = new List<SchemeData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            var schemeId = TestFixture.Create<Guid?>();

            //act
            await controller.ViewObligationAndEvidenceSummary(null, schemeId);

            //assert
            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>
                    .That.Matches(v => v.SchemeId == schemeId &&
                                       v.SchemeData.SequenceEqual(schemeData) &&
                                       v.ComplianceYears.SequenceEqual(complianceYears) &&
                                       v.ObligationEvidenceSummaryData == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenNoSelectedComplianceYearAndAvailableComplianceYears_GetSchemesWithObligationShouldBeCalled()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);

            //act
            await controller.ViewObligationAndEvidenceSummary(null, TestFixture.Create<Guid?>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligation>.That.Matches(g => 
                g.ComplianceYear == complianceYears.ElementAt(0)))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2020)]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenSelectedComplianceYearAndAvailableComplianceYears_ModelMapperShouldBeCalled(int? complianceYear)
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligation>._)).Returns(schemeData);

            //act
            await controller.ViewObligationAndEvidenceSummary(complianceYear, null);

            //assert
            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>._)).MustHaveHappenedOnceExactly();

            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>
                    .That.Matches(v => v.SchemeId == null &&
                                       v.SchemeData.SequenceEqual(schemeData) &&
                                       v.ComplianceYears.SequenceEqual(complianceYears) &&
                                       v.ObligationEvidenceSummaryData == null))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenSelectedComplianceYearAndAvailableComplianceYears_GetSchemesWithObligationShouldBeCalled()
        {
            //arrange
            var complianceYears = new List<int>() { 2020 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            const int selectedComplianceYear = 2021;

            //act
            await controller.ViewObligationAndEvidenceSummary(selectedComplianceYear, TestFixture.Create<Guid?>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligation>.That.Matches(g => 
                g.ComplianceYear == selectedComplianceYear))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenNoSelectedComplianceYearAndAvailableComplianceYearsButNoSelectedScheme_GetObligationSummaryRequestShouldNotBeCalled()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);

            //act
            await controller.ViewObligationAndEvidenceSummary(null, null);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenNoSelectedComplianceYearAndAvailableComplianceYearsWithSelectedScheme_GetObligationSummaryRequestShouldBeCalled()
        {
            //arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            var schemeId = TestFixture.Create<Guid>();

            //act
            await controller.ViewObligationAndEvidenceSummary(null, schemeId);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(g =>
                g.ComplianceYear == complianceYears.ElementAt(0)
                && g.SchemeId == schemeId))).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2020)]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenObligationSummaryRequestData_ModelMapperShouldBeCalled(int? complianceYear)
        {
            ///arrange
            var complianceYears = TestFixture.CreateMany<int>().ToList();
            var schemeData = TestFixture.CreateMany<SchemeData>().ToList();
            var obligationData = TestFixture.Create<ObligationEvidenceSummaryData>();

            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetSchemesWithObligation>._)).Returns(schemeData);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationSummaryRequest>._)).Returns(obligationData);
            var schemeId = TestFixture.Create<Guid>();

            //act
            await controller.ViewObligationAndEvidenceSummary(complianceYear, schemeId);

            //assert
            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>
                    .That.Matches(v => v.SchemeId == schemeId &&
                                       v.SchemeData.SequenceEqual(schemeData) &&
                                       v.ComplianceYears.SequenceEqual(complianceYears) &&
                                       v.ObligationEvidenceSummaryData.Equals(obligationData)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task
            ViewObligationAndEvidenceSummaryGet_GivenSelectedComplianceYearAndAvailableComplianceYearsWithSelectedScheme_GetObligationSummaryRequestShouldBeCalled()
        {
            //arrange
            var complianceYears = new List<int>() { 2020 };
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationComplianceYears>._)).Returns(complianceYears);
            const int selectedComplianceYear = 2021;
            var schemeId = TestFixture.Create<Guid>();

            //act
            await controller.ViewObligationAndEvidenceSummary(selectedComplianceYear, schemeId);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetObligationSummaryRequest>.That.Matches(g =>
                g.ComplianceYear == selectedComplianceYear
                && g.SchemeId == schemeId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenMappedModel_ModelShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ViewObligationsAndEvidenceSummaryViewModel>();
            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await controller.ViewObligationAndEvidenceSummary(TestFixture.Create<int?>(), TestFixture.Create<Guid?>()) as ViewResult;

            //assert
            ((ViewObligationsAndEvidenceSummaryViewModel)result.Model).Should().Be(model);
        }

        [Fact]
        public async Task ViewObligationAndEvidenceSummaryGet_GivenMappedModel_DefaultViewShouldBeReturned()
        {
            //arrange
            var model = TestFixture.Create<ViewObligationsAndEvidenceSummaryViewModel>();
            A.CallTo(() => mapper.Map<ViewObligationsAndEvidenceSummaryViewModel>(
                A<ViewObligationsAndEvidenceSummaryViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await controller.ViewObligationAndEvidenceSummary(TestFixture.Create<int?>(), TestFixture.Create<Guid?>()) as ViewResult;

            //assert
            result.ViewName.Should().Be(string.Empty);
        }
    }
}
