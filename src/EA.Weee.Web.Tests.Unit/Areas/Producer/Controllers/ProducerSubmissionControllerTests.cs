namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Api.Client.Models.Pay;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Requests.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ProducerSubmissionControllerTests : SimpleUnitTestBase
    {
        private readonly bool? redirectToCheckAnswers = false;
        private readonly ProducerSubmissionController controller;
        private readonly IMapper mapper;
        private readonly IWeeeClient weeeClient;
        private readonly IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>
            editOrganisationDetailsRequestCreator;
        private readonly IRequestCreator<EditContactDetailsViewModel, EditContactDetailsRequest>
            editContactDetailsRequestCreator;
        private readonly IRequestCreator<RepresentingCompanyDetailsViewModel, RepresentedOrganisationDetailsRequest>
          editRepresentedOrganisationDetailsRequestCreator;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IWeeeCache weeeCache;
        private readonly IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest>
            serviceOfNoticeRequestCreator;
        private readonly IRequestCreator<EditEeeDataViewModel, EditEeeDataRequest> editEeeDataRequestCreator;
        private readonly IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryAndCompleteRequest>
            addSignatoryAndCompleteRequestCreator;
        private readonly IPaymentService paymentService;

        public ProducerSubmissionControllerTests()
        {
            breadcrumbService = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            weeeClient = A.Fake<IWeeeClient>();
            editOrganisationDetailsRequestCreator =
                A.Fake<IRequestCreator<EditOrganisationDetailsViewModel, EditOrganisationDetailsRequest>>();
            serviceOfNoticeRequestCreator =
                A.Fake<IRequestCreator<ServiceOfNoticeViewModel, ServiceOfNoticeRequest>>();
            editRepresentedOrganisationDetailsRequestCreator =
                A.Fake<IRequestCreator<RepresentingCompanyDetailsViewModel, RepresentedOrganisationDetailsRequest>>();
            editContactDetailsRequestCreator =
                A.Fake<IRequestCreator<EditContactDetailsViewModel, EditContactDetailsRequest>>();
            editEeeDataRequestCreator = A.Fake<IRequestCreator<EditEeeDataViewModel, EditEeeDataRequest>>();
            addSignatoryAndCompleteRequestCreator = A.Fake<IRequestCreator<AppropriateSignatoryViewModel, AddSignatoryAndCompleteRequest>>();
            paymentService = A.Fake<IPaymentService>();
            controller = new ProducerSubmissionController(mapper, editOrganisationDetailsRequestCreator, editRepresentedOrganisationDetailsRequestCreator, () => weeeClient, breadcrumbService, weeeCache, editContactDetailsRequestCreator, serviceOfNoticeRequestCreator, editEeeDataRequestCreator, addSignatoryAndCompleteRequestCreator, paymentService);
        }

        [Fact]
        public void Controller_Should_Have_ExternalSiteController_As_Base()
        {
            typeof(ProducerSubmissionController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }

        [Fact]
        public void Controller_Should_Have_AuthorizeRouteClaims_Attribute()
        {
            // Arrange
            var controllerType = typeof(ProducerSubmissionController);

            // Act
            var attribute = controllerType.GetCustomAttributes(typeof(AuthorizeRouteClaimsAttribute), true)
                .FirstOrDefault() as AuthorizeRouteClaimsAttribute;

            // Assert
            attribute.Should().NotBeNull();

            var attributeType = typeof(AuthorizeRouteClaimsAttribute);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var routeIdParamField = attributeType.GetField("routeIdParam", flags);
            var routeIdParam = routeIdParamField.GetValue(attribute) as string;

            var claimsField = attributeType.GetField("claims", flags);
            var claims = claimsField.GetValue(attribute) as string[];

            routeIdParam.Should().Be("directRegistrantId");
            claims.Should().ContainSingle().Which.Should().Be(WeeeClaimTypes.DirectRegistrantAccess);

            var usage = attribute.GetType().GetCustomAttribute<AttributeUsageAttribute>();
            usage.Should().NotBeNull();
            usage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method);
            usage.AllowMultiple.Should().BeTrue();
        }

        [Fact]
        public async Task EditOrganisationDetails_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<EditOrganisationDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditOrganisationDetails(redirectToCheckAnswers) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
            viewModel.Organisation.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditOrganisationDetails_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.SmallProducerSubmissionData.OrganisationData.Id)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            var viewModel = TestFixture.Create<EditOrganisationDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditOrganisationDetailsViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            // Act
            await controller.EditOrganisationDetails(redirectToCheckAnswers);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(submissionData.SmallProducerSubmissionData.OrganisationData.Id);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditOrganisationDetails_InvalidPost_ShouldSetBreadCrumb()
        {
            // Arrange
            var viewModel = TestFixture.Create<EditOrganisationDetailsViewModel>();
            var organisationName = TestFixture.Create<string>();
            A.CallTo(() => weeeCache.FetchOrganisationName(viewModel.OrganisationId)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            controller.ModelState.AddModelError("error", "this is an error");

            // Act
            await controller.EditOrganisationDetails(viewModel);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(viewModel.OrganisationId);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditOrganisationDetails_Post_ValidModel_ShouldRedirectToTaskList()
        {
            // Arrange
            var model = TestFixture.Create<EditOrganisationDetailsViewModel>();
            model.RedirectToCheckAnswers = false;
            var request = TestFixture.Create<EditOrganisationDetailsRequest>();
            A.CallTo(() => editOrganisationDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditOrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TaskList");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditOrganisationDetails_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<EditOrganisationDetailsViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditOrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
            ((EditOrganisationDetailsViewModel)result.Model).Organisation.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public void EditOrganisationDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("EditOrganisationDetails", new[] { typeof(bool?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void EditContactDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("EditContactDetails", new[] { typeof(bool?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EditContactDetails_Post_ValidModel_ShouldRedirectToTaskList()
        {
            // Arrange
            var model = TestFixture.Create<EditContactDetailsViewModel>();
            model.RedirectToCheckAnswers = false;
            var request = TestFixture.Create<EditContactDetailsRequest>();
            A.CallTo(() => editContactDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditContactDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TaskList");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditContactDetails_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<EditContactDetailsViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditContactDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
            ((EditContactDetailsViewModel)result.Model).ContactDetails.AddressData.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditContactDetails_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<EditContactDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditContactDetails(redirectToCheckAnswers) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
            viewModel.ContactDetails.AddressData.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditContactDetails_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.SmallProducerSubmissionData.OrganisationData.Id)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            var viewModel = TestFixture.Create<EditContactDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditContactDetailsViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            // Act
            await controller.EditContactDetails(redirectToCheckAnswers);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(submissionData.SmallProducerSubmissionData.OrganisationData.Id);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditContactDetails_InvalidPost_ShouldSetBreadCrumb()
        {
            // Arrange
            var viewModel = TestFixture.Create<EditContactDetailsViewModel>();
            var organisationName = TestFixture.Create<string>();
            A.CallTo(() => weeeCache.FetchOrganisationName(viewModel.OrganisationId)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            controller.ModelState.AddModelError("error", "this is an error");

            // Act
            await controller.EditContactDetails(viewModel);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(viewModel.OrganisationId);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditRepresentedOrganisationDetails_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditRepresentedOrganisationDetails(redirectToCheckAnswers) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
            viewModel.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task EditRepresentedOrganisationDetails_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionData>();
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.OrganisationData.Id)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            var viewModel = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionData, RepresentingCompanyDetailsViewModel>(submissionData)).Returns(viewModel);

            // Act
            await controller.EditRepresentedOrganisationDetails(redirectToCheckAnswers);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(submissionData.OrganisationData.Id);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditRepresentedOrganisationDetails_InvalidPost_ShouldSetBreadCrumb()
        {
            // Arrange
            var viewModel = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            var organisationName = TestFixture.Create<string>();

            A.CallTo(() => weeeCache.FetchOrganisationName(viewModel.OrganisationId)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            controller.ModelState.AddModelError("error", "this is an error");

            // Act
            await controller.EditRepresentedOrganisationDetails(viewModel);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(viewModel.OrganisationId);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task EditRepresentedOrganisationDetails_Post_ValidModel_ShouldRedirectToTaskList()
        {
            // Arrange
            var model = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            model.RedirectToCheckAnswers = false;
            var request = TestFixture.Create<RepresentedOrganisationDetailsRequest>();
            A.CallTo(() => editRepresentedOrganisationDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditRepresentedOrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TaskList");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditRepresentedOrganisationDetails_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.EditRepresentedOrganisationDetails(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
            ((RepresentingCompanyDetailsViewModel)result.Model).Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public void EditRepresentedOrganisationDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("EditRepresentedOrganisationDetails", new[] { typeof(bool?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ServiceOfNotice_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            bool sameAsOrganisationAddress = true;

            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<ServiceOfNoticeViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.ServiceOfNotice(sameAsOrganisationAddress, redirectToCheckAnswers) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
            viewModel.Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public async Task ServiceOfNotice_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            bool sameAsOrganisationAddress = false;

            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.SmallProducerSubmissionData.OrganisationData.Id)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            var viewModel = TestFixture.Create<ServiceOfNoticeViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            // Act
            await controller.ServiceOfNotice(sameAsOrganisationAddress, redirectToCheckAnswers);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(submissionData.SmallProducerSubmissionData.OrganisationData.Id);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task ServiceOfNotice_InvalidPost_ShouldSetBreadCrumb()
        {
            // Arrange
            var viewModel = TestFixture.Create<ServiceOfNoticeViewModel>();
            var organisationName = TestFixture.Create<string>();
            A.CallTo(() => weeeCache.FetchOrganisationName(viewModel.OrganisationId)).Returns(organisationName);

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            controller.ModelState.AddModelError("error", "this is an error");

            // Act
            await controller.ServiceOfNotice(viewModel);

            // Assert
            breadcrumbService.OrganisationId.Should().Be(viewModel.OrganisationId);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task ServiceOfNotice_Post_ValidModel_ShouldRedirectToTaskList()
        {
            // Arrange
            var model = TestFixture.Create<ServiceOfNoticeViewModel>();
            model.RedirectToCheckAnswers = false;
            var request = TestFixture.Create<ServiceOfNoticeRequest>();
            A.CallTo(() => serviceOfNoticeRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.ServiceOfNotice(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("TaskList");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ServiceOfNotice_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<ServiceOfNoticeViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            var countries = TestFixture.CreateMany<CountryData>().ToList();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(Task.FromResult<IList<CountryData>>(countries));

            // Act
            var result = await controller.ServiceOfNotice(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
            ((ServiceOfNoticeViewModel)result.Model).Address.Countries.Should().BeSameAs(countries);
        }

        [Fact]
        public void ServiceOfNotice_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("ServiceOfNotice", new[] { typeof(bool?), typeof(bool?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task EditOrganisationDetails_Post_ValidModel_ShouldRedirectToCheckAnswers()
        {
            // Arrange
            var model = TestFixture.Create<EditOrganisationDetailsViewModel>();
            model.RedirectToCheckAnswers = true;
            var request = TestFixture.Create<EditOrganisationDetailsRequest>();
            A.CallTo(() => editOrganisationDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditOrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("CheckAnswers");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditContactDetails_Post_ValidModel_ShouldRedirectToCheckAnswers()
        {
            // Arrange
            var model = TestFixture.Create<EditContactDetailsViewModel>();
            model.RedirectToCheckAnswers = true;
            var request = TestFixture.Create<EditContactDetailsRequest>();
            A.CallTo(() => editContactDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditContactDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("CheckAnswers");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ServiceOfNotice_Post_ValidModel_ShouldRedirectToCheckAnswers()
        {
            // Arrange
            var model = TestFixture.Create<ServiceOfNoticeViewModel>();
            model.RedirectToCheckAnswers = true;
            var request = TestFixture.Create<ServiceOfNoticeRequest>();
            A.CallTo(() => serviceOfNoticeRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.ServiceOfNotice(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("CheckAnswers");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditRepresentedCompanyDetails_Post_ValidModel_ShouldRedirectToCheckAnswers()
        {
            // Arrange
            var model = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            model.RedirectToCheckAnswers = true;
            var request = TestFixture.Create<RepresentedOrganisationDetailsRequest>();
            A.CallTo(() => editRepresentedOrganisationDetailsRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditRepresentedOrganisationDetails(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("CheckAnswers");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditEeeData_Post_ValidModel_ShouldRedirectToCheckAnswers()
        {
            // Arrange
            var model = TestFixture.Create<EditEeeDataViewModel>();
            model.RedirectToCheckAnswers = true;
            var request = TestFixture.Create<EditEeeDataRequest>();
            A.CallTo(() => editEeeDataRequestCreator.ViewModelToRequest(model)).Returns(request);

            // Act
            var result = await controller.EditEeeeData(model) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["action"].Should().Be("CheckAnswers");
            result.RouteValues["controller"].Should().Be("Producer");
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AppropriateSignatory_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionData>();
            controller.SmallProducerSubmissionData = submissionData;

            var viewModel = TestFixture.Create<AppropriateSignatoryViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionData, AppropriateSignatoryViewModel>
                (A<SmallProducerSubmissionData>.That.Matches(sd => sd.Equals(submissionData)))).Returns(viewModel);

            // Act
            var result = await controller.AppropriateSignatory() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task AppropriateSignatory_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.SmallProducerSubmissionData.OrganisationData.Id)).Returns(organisationName);

            // Act
            await controller.AppropriateSignatory();

            // Assert
            breadcrumbService.OrganisationId.Should().Be(submissionData.SmallProducerSubmissionData.OrganisationData.Id);
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public async Task AppropriateSignatory_Post_ValidModel_ShouldRedirectToNextUrl()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionData>();
            controller.SmallProducerSubmissionData = submissionData;

            var model = TestFixture.Create<AppropriateSignatoryViewModel>();
            var request = TestFixture.Create<AddSignatoryAndCompleteRequest>();
            request.DirectRegistrantId = model.DirectRegistrantId = submissionData.DirectRegistrantId;

            var createPaymentResult = TestFixture.Create<CreatePaymentResult>();
            createPaymentResult.Links = TestFixture.Create<PaymentLinks>();
            createPaymentResult.Links.NextUrl = TestFixture.Create<Link>();
            createPaymentResult.Links.NextUrl.Href = TestFixture.Create<string>();

            A.CallTo(() => addSignatoryAndCompleteRequestCreator.ViewModelToRequest(model)).Returns(request);
            A.CallTo(() => paymentService.CheckInProgressPaymentAsync(A<string>._, request.DirectRegistrantId)).Returns(Task.FromResult((PaymentWithAllLinks)null));
            A.CallTo(() => paymentService.CreatePaymentAsync(request.DirectRegistrantId, A<string>._, A<string>._)).Returns(createPaymentResult);
            A.CallTo(() => paymentService.ValidateExternalUrl(createPaymentResult.Links.NextUrl.Href)).Returns(true);

            // Act
            var result = await controller.AppropriateSignatory(model) as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
            A.CallTo(() => paymentService.CheckInProgressPaymentAsync(A<string>._, request.DirectRegistrantId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => paymentService.CreatePaymentAsync(request.DirectRegistrantId, A<string>._, A<string>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeCache.InvalidateOrganisationNameCache(model.OrganisationId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => paymentService.ValidateExternalUrl(createPaymentResult.Links.NextUrl.Href)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AppropriateSignatory_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<AppropriateSignatoryViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            // Act
            var result = await controller.AppropriateSignatory(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
        }

        [Fact]
        public void PaymentSuccess_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("PaymentSuccess");

            // Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
        }

        [Fact]
        public async Task PaymentSuccess_ShouldReturnViewWithCorrectModel()
        {
            // Arrange
            var reference = TestFixture.Create<string>();
            var organisationId = Guid.NewGuid();
            controller.SmallProducerSubmissionData = new SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData { Id = organisationId }
            };

            // Act
            var result = await controller.PaymentSuccess(reference) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<PaymentResultModel>();
            var model = (PaymentResultModel)result.Model;
            model.PaymentReference.Should().Be(reference);
            model.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public async Task PaymentSuccess_ShouldSetBreadcrumb()
        {
            // Arrange
            var reference = TestFixture.Create<string>();
            var organisationId = Guid.NewGuid();
            var organisationName = TestFixture.Create<string>();
            controller.SmallProducerSubmissionData = new SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData { Id = organisationId }
            };

            A.CallTo(() => weeeCache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // Act
            await controller.PaymentSuccess(reference);

            // Assert
            A.CallTo(() => weeeCache.FetchOrganisationName(organisationId)).MustHaveHappenedOnceExactly();
            breadcrumbService.ExternalOrganisation.Should().Be(organisationName);
            breadcrumbService.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void PaymentFailure_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerSubmissionController).GetMethod("PaymentFailure");

            // Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
        }

        [Fact]
        public void PaymentFailure_ShouldReturnView()
        {
            // Act
            var result = controller.PaymentFailure();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }
        [Fact]
        public async Task EditEeeData_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            submissionData.RedirectToCheckAnswers = redirectToCheckAnswers;
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<EditEeeDataViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, EditEeeDataViewModel>
            (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                                                                     sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            // Act
            var result = await controller.EditEeeeData(redirectToCheckAnswers) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task EditEeeData_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var model = TestFixture.Create<EditEeeDataViewModel>();
            controller.ModelState.AddModelError("Test", "Test error");

            // Act
            var result = await controller.EditEeeeData(model) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(model);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void BackToPrevious_ShouldRedirectCorrectly(bool redirect)
        {
            // Act
            var result = controller.BackToPrevious(redirect) as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            if (redirect)
            {
                result.RouteValues["action"].Should().Be("CheckAnswers");
            }
            else
            {
                result.RouteValues["action"].Should().Be("TaskList");
            }
            result.RouteValues["controller"].Should().Be("Producer");
        }

        [Fact]
        public async Task AppropriateSignatory_Post_ValidModel_ExistingPayment_ShouldRedirectToExistingNextUrl()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionData>();
            controller.SmallProducerSubmissionData = submissionData;

            var model = TestFixture.Create<AppropriateSignatoryViewModel>();
            var request = TestFixture.Create<AddSignatoryAndCompleteRequest>();
            request.DirectRegistrantId = model.DirectRegistrantId = submissionData.DirectRegistrantId;

            var existingPayment = TestFixture.Create<PaymentWithAllLinks>();
            existingPayment.Links = TestFixture.Create<PaymentLinks>();
            existingPayment.Links.NextUrl = TestFixture.Create<Link>();
            existingPayment.Links.NextUrl.Href = TestFixture.Create<string>();

            A.CallTo(() => addSignatoryAndCompleteRequestCreator.ViewModelToRequest(model)).Returns(request);
            A.CallTo(() => paymentService.CheckInProgressPaymentAsync(A<string>._, request.DirectRegistrantId)).Returns(Task.FromResult(existingPayment));
            A.CallTo(() => paymentService.ValidateExternalUrl(existingPayment.Links.NextUrl.Href)).Returns(true);

            // Act
            var result = await controller.AppropriateSignatory(model) as RedirectResult;

            // Assert
            result.Should().NotBeNull();
            result.Url.Should().Be(existingPayment.Links.NextUrl.Href);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
            A.CallTo(() => paymentService.CheckInProgressPaymentAsync(A<string>._, request.DirectRegistrantId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => paymentService.CreatePaymentAsync(A<Guid>._, A<string>._, A<string>._)).MustNotHaveHappened();
            A.CallTo(() => paymentService.ValidateExternalUrl(existingPayment.Links.NextUrl.Href)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AppropriateSignatory_Post_ValidModel_InvalidExternalUrl_ShouldThrowException()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionData>();
            controller.SmallProducerSubmissionData = submissionData;

            var model = TestFixture.Create<AppropriateSignatoryViewModel>();
            var request = TestFixture.Create<AddSignatoryAndCompleteRequest>();
            request.DirectRegistrantId = model.DirectRegistrantId = submissionData.DirectRegistrantId;

            var createPaymentResult = TestFixture.Create<CreatePaymentResult>();
            createPaymentResult.Links = TestFixture.Create<PaymentLinks>();
            createPaymentResult.Links.NextUrl = TestFixture.Create<Link>();
            createPaymentResult.Links.NextUrl.Href = TestFixture.Create<string>();

            A.CallTo(() => addSignatoryAndCompleteRequestCreator.ViewModelToRequest(model)).Returns(request);
            A.CallTo(() => paymentService.CheckInProgressPaymentAsync(A<string>._, request.DirectRegistrantId)).Returns(Task.FromResult((PaymentWithAllLinks)null));
            A.CallTo(() => paymentService.CreatePaymentAsync(request.DirectRegistrantId, A<string>._, A<string>._)).Returns(createPaymentResult);
            A.CallTo(() => paymentService.ValidateExternalUrl(createPaymentResult.Links.NextUrl.Href)).Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.AppropriateSignatory(model));
        }
    }
}
