namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using AutoFixture;
    using Core.DirectRegistrant;
    using Core.Organisations;
    using Core.Organisations.Base;
    using Core.PaymentDetails;
    using EA.Weee.Core.Admin;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Filters;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Producers;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Filters;
    using FakeItEasy;
    using FluentAssertions;
    using Security;
    using Services;
    using Services.Caching;
    using Services.SubmissionService;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Admin.Controllers.Base;
    using Xunit;

    public class ProducerSubmissionControllerUnitTests : SimpleUnitTestBase
    {
        private readonly ProducerSubmissionController controller;
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache weeeCache;
        private readonly Guid organisationId = Guid.NewGuid();
        private readonly ISubmissionService submissionService;
        private readonly BreadcrumbService breadcrumbService;

        public ProducerSubmissionControllerUnitTests()
        {
            A.Fake<BreadcrumbService>();
            weeeClient = A.Fake<IWeeeClient>();
            weeeCache = A.Fake<IWeeeCache>();
            breadcrumbService = A.Fake<BreadcrumbService>();

            submissionService = A.Fake<ISubmissionService>();

            controller = new ProducerSubmissionController(
               () => weeeClient,
               weeeCache,
               submissionService,
               breadcrumbService);
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(ProducerSubmissionController).Should().BeDerivedFrom<AdminController>();
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
            var methodInfo = typeof(ProducerSubmissionController).GetMethod(method, new[] { typeof(string), typeof(int?) });

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

            A.CallTo(() => submissionService.OrganisationDetails(year)).Returns(expcted);

            var result = (await controller.OrganisationDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/OrganisationDetails");

            A.CallTo(() => submissionService.OrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => submissionService.Submissions(year)).Returns(expcted);

            var result = (await controller.Submissions("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/OrganisationDetails");

            A.CallTo(() => submissionService.Submissions(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => submissionService.ContactDetails(year)).Returns(expcted);

            var result = (await controller.ContactDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.ContactDetailsViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/ContactDetails");

            A.CallTo(() => submissionService.ContactDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ServiceOfNoticeDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.ServiceOfNoticeViewModel = new ServiceOfNoticeViewModel();

            A.CallTo(() => submissionService.ServiceOfNoticeDetails(year)).Returns(expcted);

            var result = (await controller.ServiceOfNoticeDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.ServiceOfNoticeViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/ServiceOfNoticeDetails");

            A.CallTo(() => submissionService.ServiceOfNoticeDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task RepresentedOrganisationDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.RepresentingCompanyDetailsViewModel = new RepresentingCompanyDetailsViewModel();

            A.CallTo(() => submissionService.RepresentedOrganisationDetails(year)).Returns(expcted);

            var result = (await controller.RepresentedOrganisationDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.RepresentingCompanyDetailsViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/RepresentedOrganisationDetails");

            A.CallTo(() => submissionService.RepresentedOrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task TotalEEEDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.EditEeeDataViewModel = new EditEeeDataViewModel();

            A.CallTo(() => submissionService.TotalEEEDetails(year)).Returns(expcted);

            var result = (await controller.TotalEEEDetails("reg", year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.RegistrationNumber.Should().Be("reg");
            model.Should().NotBeNull();
            model.EditEeeDataViewModel.Should().NotBeNull();

            result.ViewName.Should().Be("Producer/ViewOrganisation/TotalEEEDetails");

            A.CallTo(() => submissionService.TotalEEEDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, true)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void AddPaymentDetails_Get_ReturnViewModel()
        {
            SetupDefaultControllerData();

            var directProducerSubmissionId = Guid.NewGuid();
            var reg = "reg";
            var year = 2004;

            var view = controller.AddPaymentDetails(directProducerSubmissionId, reg, year) as ViewResult;

            view.Model.Should().BeOfType<PaymentDetailsViewModel>();

            var vm = (view.Model as PaymentDetailsViewModel);

            vm.DirectProducerSubmissionId.Should().Be(directProducerSubmissionId);
            vm.RegistrationNumber.Should().Be(reg);
            vm.Year.Should().Be(year);
        }

        [Fact]
        public void AddPaymentDetails_Get_SetsBreadCrumb()
        {
            SetupDefaultControllerData();

            var directProducerSubmissionId = Guid.NewGuid();
            var reg = "reg";
            var year = 2004;

            var view = controller.AddPaymentDetails(directProducerSubmissionId, reg, year) as ViewResult;

            view.Model.Should().BeOfType<PaymentDetailsViewModel>();

            var vm = (view.Model as PaymentDetailsViewModel);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public void RemoveSubmission_Get_SetsBreadCrumb()
        {
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive = false;

            string registrationNumber = "reg";
            int year = 2024;

            // Act
            var result = controller.RemoveSubmission(registrationNumber, year) as ViewResult;

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public void AddPaymentDetails_Post_DecoratesWithAuthorizeInternalClaimsAttribute()
        {
            var methodInfo = typeof(ProducerSubmissionController)
                .GetMethod("AddPaymentDetails", new[] { typeof(PaymentDetailsViewModel) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void AddPaymentDetails_Get_DecoratesWithAuthorizeInternalClaimsAttribute()
        {
            var methodInfo = typeof(ProducerSubmissionController)
                .GetMethod("AddPaymentDetails", new[] { typeof(Guid), typeof(string), typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public async Task AddPaymentDetails_Post_ReturnsRouteValuesAndCallsClient()
        {
            SetupDefaultControllerData();

            var directProducerSubmissionId = Guid.NewGuid();

            var vm = new PaymentDetailsViewModel()
            {
                ConfirmPaymentMade = true,
                DirectProducerSubmissionId = directProducerSubmissionId,
                PaymentDetailsDescription = "des",
                PaymentMethod = "meth",
                PaymentReceivedDate = new DateTimeInput { Day = 01, Month = 10, Year = 1989 }
            };

            var payresult = new ManualPaymentResult
            {
                ComplianceYear = 2004,
                RegistrationNumber = "reg"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<AddPaymentDetails>.That.Matches(s => s.PaymentMethod == vm.PaymentMethod
                && s.PaymentRecievedDate.Year == vm.PaymentReceivedDate.Year
                && s.PaymentDetailsDescription == vm.PaymentDetailsDescription
                && s.DirectProducerSubmissionId == vm.DirectProducerSubmissionId)))
               .Returns(payresult);

            var view = (await controller.AddPaymentDetails(vm)) as RedirectToRouteResult;

            view.RouteValues["registrationNumber"].Should().Be(payresult.RegistrationNumber);
            view.RouteValues["year"].Should().Be(payresult.ComplianceYear);
            view.RouteValues["action"].Should().Be("OrganisationDetails");

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                A<AddPaymentDetails>.That.Matches(s => s.PaymentMethod == vm.PaymentMethod
                && s.PaymentRecievedDate == vm.PaymentReceivedDate
                && s.PaymentDetailsDescription == vm.PaymentDetailsDescription
                && s.DirectProducerSubmissionId == vm.DirectProducerSubmissionId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RemoveSubmission_Get_HasAuthorisedRepresentitive_ReturnAndPopulatesViewModel()
        {
            // Arrange
            SetupDefaultControllerData();

            string registrationNumber = "reg"; 
            int year = 2024;
            var submission = controller.SmallProducerSubmissionData.SubmissionHistory[year];
            var producerName = controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive ? controller.SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName;

            // Act
            var result = controller.RemoveSubmission(registrationNumber, year) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().NotBeNull();

            var viewModel = result.Model as ConfirmRemovalViewModel;
            
            viewModel.Producer.RegistrationNumber.Should().Be(registrationNumber); 
            viewModel.Producer.ComplianceYear.Should().Be(year);
            viewModel.Producer.ProducerName.Should().Be(producerName);
            viewModel.Producer.RegisteredProducerId.Should().Be(submission.RegisteredProducerId);
        }

        [Fact]
        public void RemoveSubmission_Get_NotHasAuthorisedRepresentitive_ReturnAndPopulatesViewModel()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive = false;

            string registrationNumber = "reg";
            int year = 2024;
            var submission = controller.SmallProducerSubmissionData.SubmissionHistory[year];
            var producerName = controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive ? controller.SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName;

            // Act
            var result = controller.RemoveSubmission(registrationNumber, year) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().NotBeNull();

            var viewModel = result.Model as ConfirmRemovalViewModel;

            viewModel.Producer.RegistrationNumber.Should().Be(registrationNumber);
            viewModel.Producer.ComplianceYear.Should().Be(year);
            viewModel.Producer.ProducerName.Should().Be(producerName);
            viewModel.Producer.RegisteredProducerId.Should().Be(submission.RegisteredProducerId);
        }

        [Fact]
        public void RemoveSubmission_Get_ShouldHaveContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("RemoveSubmission", new[] { typeof(string), typeof(int) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<AdminSmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void Removed_Get_ShouldHaveContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("Removed", new[] { typeof(string), typeof(string), typeof(int) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public async Task RemoveSubmission_Post_SelectValueYes_ReturnsAndRedirectsProducerValues()
        {
            // Arrange
            SetupDefaultControllerData();

            var returns = TestFixture.Create<RemoveSmallProducerResult>();

            var vm = new ConfirmRemovalViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "Yes",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    ComplianceYear = 2004,
                    RegistrationNumber = "regno",
                    ProducerName = "name",
                    RegisteredProducerId = Guid.NewGuid()
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
               A<RemoveSmallProducer>.That.Matches(s => s.RegisteredProducerId == vm.Producer.RegisteredProducerId)))
              .Returns(returns);

            // Act
            var result = await controller.RemoveSubmission(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["registrationNumber"].Should().Be(vm.Producer.RegistrationNumber);
            result.RouteValues["producerName"].Should().Be(vm.Producer.ProducerName);
            result.RouteValues["year"].Should().Be(vm.Producer.ComplianceYear);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Removed));
        }

        [Fact]
        public async Task RemoveSubmission_Post_SelectValueYesInvalidateProducerSearchCache_CallsInvalidate()
        {
            // Arrange
            SetupDefaultControllerData();

            var returns = new RemoveSmallProducerResult(true);

            var vm = new ConfirmRemovalViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "Yes",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    RegisteredProducerId = Guid.NewGuid()
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
               A<RemoveSmallProducer>.That.Matches(s => s.RegisteredProducerId == vm.Producer.RegisteredProducerId)))
              .Returns(returns);

            // Act
            var result = await controller.RemoveSubmission(vm) as RedirectToRouteResult;

            //Assert
            A.CallTo(() => weeeCache.InvalidateProducerSearch()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task RemoveSubmission_Post_SelectValueNoInvalidateProducerSearchCache_CallsInvalidate()
        {
            // Arrange
            SetupDefaultControllerData();

            var returns = new RemoveSmallProducerResult(true);

            var vm = new ConfirmRemovalViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "No",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    RegisteredProducerId = Guid.NewGuid()
                }
            };

            // Act
            var result = await controller.RemoveSubmission(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["RegistrationNumber"].Should().Be(vm.Producer.RegistrationNumber);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Submissions));
        }

        [Fact]
        public async Task RemoveSubmission_Post_ModelStateInvalid_ReturnsView()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new ConfirmRemovalViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "No",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    RegisteredProducerId = Guid.NewGuid()
                }
            };

            controller.ModelState.AddModelError("Field", "Problem");

            // Act
            var result = await controller.RemoveSubmission(vm) as ViewResult;

            //Assert
            var model = result.Model as ConfirmRemovalViewModel;
            model.Should().Be(vm);
        }

        [Fact]
        public async Task Removed_Post_NoSubmissions_RedirectsToNoSubmissions()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>();

            var vm = new RemovedViewModel
            {
                ComplianceYear = 2004,
                ProducerName = "Test",
                RegistrationNumber = "reg",
                SchemeName = "s"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
              A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(s => s.RegistrationNumber == vm.RegistrationNumber)))
             .Returns(controller.SmallProducerSubmissionData);

            // Act
            var result = await controller.Removed(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["organisationId"].Should().Be(controller.SmallProducerSubmissionData.OrganisationData.Id);
            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public void OrganisationHasNoSubmissions_Get_SetsBreadcrumb()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>();

            // Act
            var result = controller.OrganisationHasNoSubmissions(controller.SmallProducerSubmissionData.OrganisationData.Id) as RedirectToRouteResult;

            //Assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public async Task Removed_Post_NoSubmissions_RedirectsToSubmissions()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new RemovedViewModel
            {
                ComplianceYear = 2004,
                ProducerName = "Test",
                RegistrationNumber = "reg",
                SchemeName = "s"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
              A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(s => s.RegistrationNumber == vm.RegistrationNumber)))
             .Returns(controller.SmallProducerSubmissionData);

            // Act
            var result = await controller.Removed(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["RegistrationNumber"].Should().Be(vm.RegistrationNumber);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Submissions));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void OrganisationHasNoSubmissions_Get_SetsCorrectDisplayBackValue(bool? fromRemoved)
        {
            // Arrange
            SetupDefaultControllerData();
            var organisationId = Guid.NewGuid();

            // Act
            var result = controller.OrganisationHasNoSubmissions(organisationId, fromRemoved) as ViewResult;

            // Assert
            var model = result.Model as OrganisationIdViewModel;
            model.DisplayBack.Should().Be(fromRemoved == false);
            model.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task Removed_Post_NoSubmissions_RedirectsToNoSubmissionsWithFromRemovedTrue()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>();

            var vm = new RemovedViewModel
            {
                ComplianceYear = 2004,
                ProducerName = "Test",
                RegistrationNumber = "reg",
                SchemeName = "s"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
                    A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(s => s.RegistrationNumber == vm.RegistrationNumber)))
                .Returns(controller.SmallProducerSubmissionData);

            // Act
            var result = await controller.Removed(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["organisationId"].Should().Be(controller.SmallProducerSubmissionData.OrganisationData.Id);
            result.RouteValues["fromRemoved"].Should().Be(true);
            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public async Task RemoveSubmission_Post_SetsBreadcrumb()
        {
            // Arrange
            SetupDefaultControllerData();
            var vm = new ConfirmRemovalViewModel
            {
                SelectedValue = "No",
                Producer = new ProducerDetailsScheme()
            };

            // Act
            await controller.RemoveSubmission(vm);

            // Assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public void ReturnProducerRegistration_Get_HasAuthorisedRepresentitive_ReturnAndPopulatesViewModel()
        {
            // Arrange
            SetupDefaultControllerData();

            string registrationNumber = "reg";
            int year = 2024;
            var submission = controller.SmallProducerSubmissionData.SubmissionHistory[year];
            var producerName = controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive ? controller.SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName;

            // Act
            var result = controller.ReturnProducerRegistration(registrationNumber, year) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().NotBeNull();

            var viewModel = result.Model as ConfirmReturnViewModel;

            viewModel.Producer.RegistrationNumber.Should().Be(registrationNumber);
            viewModel.Producer.ComplianceYear.Should().Be(year);
            viewModel.Producer.ProducerName.Should().Be(producerName);
            viewModel.Producer.RegisteredProducerId.Should().Be(submission.RegisteredProducerId);
        }

        [Fact]
        public void ReturnProducerRegistration_Get_NotHasAuthorisedRepresentitive_ReturnAndPopulatesViewModel()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive = false;

            string registrationNumber = "reg";
            int year = 2024;
            var submission = controller.SmallProducerSubmissionData.SubmissionHistory[year];
            var producerName = controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive ? controller.SmallProducerSubmissionData.AuthorisedRepresentitiveData.CompanyName : submission.CompanyName;

            // Act
            var result = controller.ReturnProducerRegistration(registrationNumber, year) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().NotBeNull();

            var viewModel = result.Model as ConfirmReturnViewModel;

            viewModel.Producer.RegistrationNumber.Should().Be(registrationNumber);
            viewModel.Producer.ComplianceYear.Should().Be(year);
            viewModel.Producer.ProducerName.Should().Be(producerName);
            viewModel.Producer.RegisteredProducerId.Should().Be(submission.RegisteredProducerId);
        }

        [Fact]
        public void ReturnProducerRegistration_Get_ShouldHaveContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("ReturnProducerRegistration", new[] { typeof(string), typeof(int) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<AdminSmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void ReturnProducerRegistration_Get_SetsBreadCrumb()
        {
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.HasAuthorisedRepresentitive = false;

            string registrationNumber = "reg";
            int year = 2024;
            var submission = controller.SmallProducerSubmissionData.SubmissionHistory[year];

            // Act
            var result = controller.ReturnProducerRegistration(registrationNumber, year) as ViewResult;

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public async Task ReturnProducerRegistration_Post_SelectValueYes_ReturnsAndRedirectsProducerValues()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new ConfirmReturnViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "Yes",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    ComplianceYear = 2004,
                    RegistrationNumber = "regno",
                    ProducerName = "name",
                    RegisteredProducerId = Guid.NewGuid()
                },
                DirectProducerSubmissionId = Guid.NewGuid()
            };

            // Act
            var result = await controller.ReturnProducerRegistration(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["registrationNumber"].Should().Be(vm.Producer.RegistrationNumber);
            result.RouteValues["producerName"].Should().Be(vm.Producer.ProducerName);
            result.RouteValues["year"].Should().Be(vm.Producer.ComplianceYear);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Returned));

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
            A<ReturnSmallProducerSubmission>.That.Matches(s => s.DirectProducerSubmissionId == vm.DirectProducerSubmissionId)))
            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ReturnProducerRegistration_Post_SelectValueNo_ReturnsSubmissionsView()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new ConfirmReturnViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "No",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    ComplianceYear = 2004,
                    RegistrationNumber = "regno",
                    ProducerName = "name",
                    RegisteredProducerId = Guid.NewGuid()
                },
                DirectProducerSubmissionId = Guid.NewGuid()
            };

            // Act
            var result = await controller.ReturnProducerRegistration(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["RegistrationNumber"].Should().Be(vm.Producer.RegistrationNumber);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Submissions));
        }

        [Fact]
        public async Task ReturnProducerRegistration_Post_ModelStateInvalid_ReturnsView()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new ConfirmReturnViewModel
            {
                PossibleValues = new[] { string.Empty },
                SelectedValue = "No",
                Producer = new Core.Admin.ProducerDetailsScheme
                {
                    ComplianceYear = 2004,
                    RegistrationNumber = "regno",
                    ProducerName = "name",
                    RegisteredProducerId = Guid.NewGuid()
                },
                DirectProducerSubmissionId = Guid.NewGuid()
            };

            controller.ModelState.AddModelError("Field", "Problem");

            // Act
            var result = await controller.ReturnProducerRegistration(vm) as ViewResult;

            //Assert
            var model = result.Model as ConfirmReturnViewModel;
            model.Should().Be(vm);
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public void Returned_Get_ShouldHaveContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("Returned", new[] { typeof(string), typeof(string), typeof(int) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
        }

        [Fact]
        public void Returned_Get_SetsBreadcrumb()
        {
            // Arrange
            var registrationNumber = "testRegNo";
            var producerName = "testProducer";
            var year = 2024;

            // Act
            var result = controller.Returned(registrationNumber, producerName, year) as ViewResult;

            // Assert
            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.DirectRegistrantDetails);
        }

        [Fact]
        public void Returned_Post_ShouldHaveContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("Returned", new[] { typeof(ReturnedViewModel) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<HttpPostAttribute>();
            methodInfo.Should().BeDecoratedWith<AuthorizeInternalClaimsAttribute>(a => a.Match(new AuthorizeInternalClaimsAttribute(Claims.InternalAdmin)));
            methodInfo.Should().BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public async Task Returned_Post_submissions_RedirectsToSubmissions()
        {
            // Arrange
            SetupDefaultControllerData();

            var vm = new ReturnedViewModel
            {
                ComplianceYear = 2004,
                ProducerName = "Test",
                RegistrationNumber = "reg",
                SchemeName = "s"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
              A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(s => s.RegistrationNumber == vm.RegistrationNumber)))
             .Returns(controller.SmallProducerSubmissionData);

            // Act
            var result = await controller.Returned(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["RegistrationNumber"].Should().Be(vm.RegistrationNumber);
            result.RouteValues["action"].Should().Be(nameof(ProducerSubmissionController.Submissions));
        }

        [Fact]
        public async Task Returned_Post_NoSubmissions_RedirectsToNoSubmissions()
        {
            // Arrange
            SetupDefaultControllerData();
            controller.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>();

            var vm = new ReturnedViewModel
            {
                ComplianceYear = 2004,
                ProducerName = "Test",
                RegistrationNumber = "reg",
                SchemeName = "s"
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._,
              A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(s => s.RegistrationNumber == vm.RegistrationNumber)))
             .Returns(controller.SmallProducerSubmissionData);

            // Act
            var result = await controller.Returned(vm) as RedirectToRouteResult;

            //Assert
            result.RouteValues["organisationId"].Should().Be(controller.SmallProducerSubmissionData.OrganisationData.Id);
            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        private void SetupDefaultControllerData()
        {
            controller.SmallProducerSubmissionData = new SmallProducerSubmissionData
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
                CurrentSubmission = new SmallProducerSubmissionHistoryData
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