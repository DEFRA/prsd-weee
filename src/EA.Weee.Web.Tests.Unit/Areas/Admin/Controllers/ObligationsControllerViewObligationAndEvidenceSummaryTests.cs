namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
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
        public async Task ViewObligationAndEvidenceSummaryGet_ObligationComplianceYearsShouldBeRetrieved()
        {
            //act
            await controller.ViewObligationAndEvidenceSummary(TestFixture.Create<int>(), TestFixture.Create<Guid>());

            //assert
            A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetObligationComplianceYears>.That.Matches(g => g.IncludeCurrentYear == false &&
                                                                      g.Authority == null))).MustHaveHappenedOnceExactly();
        }
    }
}
