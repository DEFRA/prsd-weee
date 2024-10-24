namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;
  
    public class ProducerSubmissionControllerUnitTests : SimpleUnitTestBase
    {
        private readonly EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController controller;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumb;
        private readonly Guid organisationId = Guid.NewGuid();
        private readonly IMapper mapper;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly ISubmissionService submissionService;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeClient client;

        public ProducerSubmissionControllerUnitTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            templateExecutor = A.Fake<IMvcTemplateExecutor>();
            pdfDocumentProvider = A.Fake<IPdfDocumentProvider>();
            client = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            apiClient = () => client;

            submissionService = A.Fake<ISubmissionService>();

            controller = new EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController(
               breadcrumb,
               weeeCache,
               mapper,
               templateExecutor,
               pdfDocumentProvider,
               submissionService,
               apiClient);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController).Should().BeDerivedFrom<AdminController>();
        }

        [Theory]
        [InlineData("ContactDetails")]
        [InlineData("OrganisationDetails")]
        [InlineData("RepresentedOrganisationDetails")]
        [InlineData("ServiceOfNoticeDetails")]
        [InlineData("TotalEEEDetails")]
        public void Get_ShouldHaveSmallProducerSubmissionContextAttribute(string method)
        {
            // Arrange
            var methodInfo = typeof(EA.Weee.Web.Areas.Admin.Controllers.ProducerSubmissionController).GetMethod(method, new[] { typeof(string), typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<AdminSmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task OrganisationDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "InternalAdmin") }, "TestAuthentication"));
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.OrganisationViewModel = new OrganisationViewModel();

            A.CallTo(() => this.submissionService.OrganisationDetails(year)).Returns(expcted);

            var result = (await controller.OrganisationDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/OrganisationDetails");

            A.CallTo(() => this.submissionService.OrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task Submissions_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "InternalAdmin") }, "TestAuthentication"));
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.OrganisationViewModel = new OrganisationViewModel();

            A.CallTo(() => this.submissionService.Submissions(year)).Returns(expcted);

            var result = (await controller.Submissions("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/OrganisationDetails");

            A.CallTo(() => this.submissionService.Submissions(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("InternalAdmin", true)]
        [InlineData("", false)]
        public async Task OrganisationDetails_ReturnViewModelWithCorrectClaims(string role, bool isAdmin)
        {
            int? year = 2004;

            SetupDefaultControllerData();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, role) }, "TestAuthentication"));
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);

            var result = (await controller.OrganisationDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.IsAdmin.Should().Be(isAdmin);
        }

        [Theory]
        [InlineData("InternalAdmin", true)]
        [InlineData("", false)]
        public async Task Submissions_ReturnViewModelWithCorrectClaims(string role, bool isAdmin)
        {
            int? year = 2004;

            SetupDefaultControllerData();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, role) }, "TestAuthentication"));
            controller.ControllerContext = A.Fake<ControllerContext>();
            A.CallTo(() => controller.ControllerContext.HttpContext.User).Returns(user);

            var result = (await controller.Submissions("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.IsAdmin.Should().Be(isAdmin);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ContactDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.ContactDetailsViewModel = new ContactDetailsViewModel();

            A.CallTo(() => this.submissionService.ContactDetails(year)).Returns(expcted);

            var result = (await controller.ContactDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.ContactDetailsViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/ContactDetails");

            A.CallTo(() => this.submissionService.ContactDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ServiceOfNoticeDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.ServiceOfNoticeViewModel = new ServiceOfNoticeViewModel();

            A.CallTo(() => this.submissionService.ServiceOfNoticeDetails(year)).Returns(expcted);

            var result = (await controller.ServiceOfNoticeDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.ServiceOfNoticeViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/ServiceOfNoticeDetails");

            A.CallTo(() => this.submissionService.ServiceOfNoticeDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task RepresentedOrganisationDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.RepresentingCompanyDetailsViewModel = new RepresentingCompanyDetailsViewModel();

            A.CallTo(() => this.submissionService.RepresentedOrganisationDetails(year)).Returns(expcted);

            var result = (await controller.RepresentedOrganisationDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.RepresentingCompanyDetailsViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/RepresentedOrganisationDetails");

            A.CallTo(() => this.submissionService.RepresentedOrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task TotalEEEDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.EditEeeDataViewModel = new EditEeeDataViewModel();

            A.CallTo(() => this.submissionService.TotalEEEDetails(year)).Returns(expcted);

            var result = (await controller.TotalEEEDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.EditEeeDataViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/TotalEEEDetails");

            A.CallTo(() => this.submissionService.TotalEEEDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        private void SetupDefaultControllerData()
        {
            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
                {
                    { 2024, TestFixture.Build<SmallProducerSubmissionHistoryData>().With(s => s.Status, SubmissionStatus.Submitted).Create() },
                },
                OrganisationData = new OrganisationData
                {
                    Id = organisationId,
                    CompanyRegistrationNumber = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString(),
                    OrganisationType = OrganisationType.Partnership,
                    TradingName = Guid.NewGuid().ToString(),
                    BusinessAddress = new Core.Shared.AddressData
                    {
                        Address1 = Guid.NewGuid().ToString(),
                        Address2 = Guid.NewGuid().ToString(),
                        TownOrCity = Guid.NewGuid().ToString(),
                        CountryName = Guid.NewGuid().ToString(),
                        WebAddress = Guid.NewGuid().ToString(),
                        Telephone = "4567894563",
                        Postcode = Guid.NewGuid().ToString()
                    }
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    ComplianceYear = 2005,
                    OrganisationDetailsComplete = true,
                    ContactDetailsComplete = true,
                    ServiceOfNoticeComplete = true,
                    RepresentingCompanyDetailsComplete = true,
                    EEEDetailsComplete = true,
                    ServiceOfNoticeData = new Core.Shared.AddressData
                    {
                        Address1 = Guid.NewGuid().ToString(),
                        Address2 = Guid.NewGuid().ToString(),
                        TownOrCity = Guid.NewGuid().ToString(),
                        CountryName = Guid.NewGuid().ToString(),
                        WebAddress = Guid.NewGuid().ToString(),
                        Telephone = "4567894563",
                        Postcode = Guid.NewGuid().ToString()
                    },
                    AuthorisedRepresentitiveData = TestFixture.Create<AuthorisedRepresentitiveData>()
                },
                HasAuthorisedRepresentitive = true,
                AuthorisedRepresentitiveData = TestFixture.Create<AuthorisedRepresentitiveData>()
            };
        }
    }
}