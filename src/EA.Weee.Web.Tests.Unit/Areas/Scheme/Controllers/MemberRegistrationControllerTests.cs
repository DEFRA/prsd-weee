namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services;
    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Xunit;

    public class MemberRegistrationControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;

        public MemberRegistrationControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
        }

        private MemberRegistrationController MemberRegistrationController()
        {
            var controller = new MemberRegistrationController(
                () => weeeClient,
                A.Fake<IWeeeCache>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<CsvWriterFactory>(),
                 mapper);

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private MemberRegistrationController MemberRegistrationController(object viewModel)
        {
            var controller = new MemberRegistrationController(
                 () => weeeClient,
                 A.Fake<IWeeeCache>(),
                 A.Fake<BreadcrumbService>(),
                 A.Fake<CsvWriterFactory>(),
                 mapper);

            new HttpContextMocker().AttachToController(controller);

            // Mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(viewModel, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModel, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }

            return controller;
        }

        private FakeMemberRegistrationController BuildFakeMemberRegistrationController()
        {
            var controller = new FakeMemberRegistrationController
                (weeeClient,   
                A.Fake<IWeeeCache>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<CsvWriterFactory>(),
                A.Fake<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        [Fact]
        public async void GetAuthorisationRequired_ChecksStatusOfScheme()
        {
            await MemberRegistrationController().AuthorisationRequired(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsPendingApproval_ReturnsViewWithPendingStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Pending);

            var result = await MemberRegistrationController().AuthorisationRequired(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);

            var view = ((AuthorizationRequiredViewModel)((ViewResult)result).Model);

            Assert.Equal(SchemeStatus.Pending, view.Status);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsRejected_ReturnsViewWithRejectedStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Rejected);

            var result = await MemberRegistrationController().AuthorisationRequired(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);

            var view = ((AuthorizationRequiredViewModel)((ViewResult)result).Model);

            Assert.Equal(SchemeStatus.Rejected, view.Status);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsApproved_RedirectsToPcsMemberSummary()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Approved);

            var result = await MemberRegistrationController().AuthorisationRequired(A.Dummy<Guid>());

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Summary", routeValues["action"]);
            Assert.Equal("MemberRegistration", routeValues["controller"]);
        }

        [Fact]
        public async void GetAddOrAmendMembers_ChecksForValidityOfOrganisation()
        {
            try
            {
                await MemberRegistrationController().AddOrAmendMembers(A.Dummy<Guid>());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => MemberRegistrationController().AddOrAmendMembers(A.Dummy<Guid>()));
        }

        [Fact]
        public async void GetAddOrAmendMembers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await MemberRegistrationController().AddOrAmendMembers(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_NotAjaxRequest_ModelIsInvalid_ReturnsView()
        {
            var controller = GetRealMemberRegistrationControllerWithFakeContext();

            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.AddOrAmendMembers(A.Dummy<Guid>(), new PCSFileUploadViewModel());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_AjaxRequest_ModelIsInvalid_ReturnsError()
        {
            var controller = GetRealMemberRegistrationControllerWithAjaxRequest();

            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.AddOrAmendMembers(A.Dummy<Guid>(), new PCSFileUploadViewModel());

            Assert.IsType<HttpStatusCodeResult>(result);
        }

        [Fact]
        public async void PostAddOrAmendMembers_FileIsMappedSuccessfully_ValidateRequestSentWithConvertedFileDataAndOrganisationId()
        {
            var request = new ProcessXmlFile(A.Dummy<Guid>(), A.Dummy<byte[]>(), A.Dummy<string>());

            A.CallTo(() => mapper.Map<PCSFileUploadViewModel, ProcessXmlFile>(A<PCSFileUploadViewModel>._))
                .Returns(request);

            try
            {
                await MemberRegistrationController().AddOrAmendMembers(Guid.NewGuid(), new PCSFileUploadViewModel());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostAddOrAmendMembers_NotAjaxRequest_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var validationId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessXmlFile>._))
                .Returns(validationId);

            var controller = GetRealMemberRegistrationControllerWithFakeContext();

            var result = await controller.AddOrAmendMembers(A.Dummy<Guid>(), new PCSFileUploadViewModel());

            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("ViewErrorsAndWarnings", redirect.RouteValues["action"]);
            Assert.Equal("MemberRegistration", redirect.RouteValues["controller"]);
            Assert.Equal(validationId, redirect.RouteValues["memberUploadId"]);
        }

        [Fact]
        public async void PostAddOrAmendMembers_AjaxRequest_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var validationId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessXmlFile>._))
                .Returns(validationId);

            var controller = GetRealMemberRegistrationControllerWithAjaxRequest();

            var result = await controller.AddOrAmendMembers(A.Dummy<Guid>(), new PCSFileUploadViewModel());

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public async void GetSummary_GetsSummaryOfLatestMemberUpload()
        {
            await MemberRegistrationController().Summary(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetSummary_Always_ReturnsViewWithSummaryModel()
        {
            var result = await MemberRegistrationController().Summary(A.Dummy<Guid>());

            Assert.IsType<ViewResult>(result);
            Assert.IsType<SummaryViewModel>(((ViewResult)result).Model);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_NoErrors_RedirectsToAcceptedPage()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
            .Returns(new List<ErrorData> { });

            var result = await MemberRegistrationController().ViewErrorsAndWarnings(A.Dummy<Guid>(), A.Dummy<Guid>());
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("XmlHasNoErrors", redirect.RouteValues["action"]);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_ShowsErrorPage()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
            .Returns(new List<ErrorData> { new ErrorData("An Error", ErrorLevel.Error) });

            var result = await MemberRegistrationController().ViewErrorsAndWarnings(A.Dummy<Guid>(), A.Dummy<Guid>());
            
            Assert.IsType<ViewResult>(result);
            
            var view = (ViewResult)result;

            Assert.Equal("ViewErrorsAndWarnings", view.ViewName);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_WarningPresent_ShowsAcceptedPage()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
           .Returns(new List<ErrorData> { new ErrorData("An warning", ErrorLevel.Warning) });

            var result = await MemberRegistrationController().ViewErrorsAndWarnings(A.Dummy<Guid>(), A.Dummy<Guid>());

            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("XmlHasNoErrors", redirect.RouteValues["action"]);
        }

        [Fact]
        public async Task GetXmlHasNoErrors_NoErrors_HasProvidedErrorData()
        {
            var errors = new List<ErrorData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
             .Returns(errors);

            var result = await MemberRegistrationController().XmlHasNoErrors(A.Dummy<Guid>(), A.Dummy<Guid>());
            var providedErrors = ((MemberUploadResultViewModel)((ViewResult)result).Model).ErrorData;
            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async Task GetViewErrorsOrWarnings_ErrorsPresent_HasProvidedErrorData()
        {
            var errors = new List<ErrorData>
            {
                new ErrorData("An Error", ErrorLevel.Error)
            };

            var providedErrors = await ErrorsAfterClientReturns(errors);

            Assert.Equal(errors, providedErrors);
        }

        [Fact]
        public async void GetProducerCSV_ValidComplianceYear_ReturnsCSVFile()
        {
            var testCSVData = new ProducerCSVFileData { FileContent = "Test, Test, Test", FileName = "test.csv" };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerCSV>._))
                .Returns(testCSVData);

            var result = await MemberRegistrationController().GetProducerCSV(A.Dummy<Guid>(), A.Dummy<int>());

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void PostSubmitXml_ValidMemberUploadId_ReturnsSuccessfulSubmissionView()
        {
            var memberUploadId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<MemberUploadSubmission>._))
                .Returns(memberUploadId);

            var result = await MemberRegistrationController().XmlHasNoErrors(
                A.Dummy<Guid>(),
                memberUploadId,
                new MemberUploadResultViewModel { ErrorData = new List<ErrorData>() });

            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("SuccessfulSubmission", redirect.RouteValues["action"]);
            Assert.Equal(memberUploadId, redirect.RouteValues["memberUploadId"]);
        }

        [Fact]
        public async void PostSubmitXml_PrivacyPolicyNotChecked_ReturnsValidationError()
        {
            var memberUploadId = Guid.NewGuid();

            var memberUploadResult = new MemberUploadResultViewModel
            {
                ErrorData = new List<ErrorData>(),
                PrivacyPolicy = false
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
               .Returns(new List<ErrorData>());

            var result = await MemberRegistrationController(memberUploadResult).XmlHasNoErrors(
                A.Dummy<Guid>(),
                memberUploadId,
                memberUploadResult) as ViewResult;

            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void OnActionExecuting_ActionAuthorizationRequired_DoesNotCheckPcsId()
        {
            var fakeController = BuildFakeMemberRegistrationController();
            var fakeActionParameters = ActionExecutingContextHelper.FakeActionParameters();
            var fakeActionDescriptor = ActionExecutingContextHelper.FakeActionDescriptorWithActionName("AuthorisationRequired");

            ActionExecutingContext context = new ActionExecutingContext();
            context.ActionParameters = fakeActionParameters;
            context.ActionDescriptor = fakeActionDescriptor;

            fakeController.InvokeOnActionExecuting(context);

            A.CallTo(() => fakeActionDescriptor.ActionName).MustHaveHappened(Repeated.Exactly.Once);

            object dummyObject;
            A.CallTo(() => fakeActionParameters.TryGetValue(A<string>._, out dummyObject)).MustNotHaveHappened();
        }

        [Fact]
        public void OnActionExecuting_NotActionAuthorizationRequiredNoPcs_ThrowsInvalidOperationException()
        {
            var fakeController = BuildFakeMemberRegistrationController();
            var fakeActionParameters = ActionExecutingContextHelper.FakeActionParameters(false, A.Dummy<Guid>());

            ActionExecutingContext context = new ActionExecutingContext();
            context.ActionParameters = fakeActionParameters;
            context.ActionDescriptor = ActionExecutingContextHelper.FakeActionDescriptorWithActionName("TestAction");

            Assert.Throws(typeof(InvalidOperationException), () => fakeController.InvokeOnActionExecuting(context));
        }

        [Fact]
        public void OnActionExecuting_NotActionAuthorizationRequiredNotApprovedPcs_ResultsToAuthorizationRequired()
        {
            var pcsId = Guid.NewGuid();
            var fakeController = BuildFakeMemberRegistrationController();
            var fakeActionParameters = ActionExecutingContextHelper.FakeActionParameters(true, pcsId);

            ActionExecutingContext context = new ActionExecutingContext();
            context.ActionParameters = fakeActionParameters;
            context.ActionDescriptor = ActionExecutingContextHelper.FakeActionDescriptorWithActionName("TestAction");

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Pending);

            fakeController.InvokeOnActionExecuting(context);

            var redirect = (RedirectToRouteResult)context.Result;

            Assert.Equal("AuthorisationRequired", redirect.RouteValues["action"]);
            Assert.Equal(pcsId, redirect.RouteValues["pcsId"]);
        }

        [Fact]
        public void OnActionExecuting_NotActionAuthorizationRequiredApprovedPcs_ResultsToNoRedirection()
        {
            var fakeController = BuildFakeMemberRegistrationController();
            var fakeActionParameters = ActionExecutingContextHelper.FakeActionParameters(true, A.Dummy<Guid>());

            ActionExecutingContext context = new ActionExecutingContext();
            context.ActionParameters = fakeActionParameters;
            context.ActionDescriptor = ActionExecutingContextHelper.FakeActionDescriptorWithActionName("TestAction");

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Approved);

            fakeController.InvokeOnActionExecuting(context);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .MustHaveHappened();

            Assert.Null(context.Result);
        }

        private async Task<List<ErrorData>> ErrorsAfterClientReturns(List<ErrorData> memberUploadErrorDatas)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetMemberUploadData>._))
              .Returns(memberUploadErrorDatas);

            var result = await MemberRegistrationController().ViewErrorsAndWarnings(A.Dummy<Guid>(), A.Dummy<Guid>());
     
            return ((MemberUploadResultViewModel)((ViewResult)result).Model).ErrorData;
        }

        private MemberRegistrationController GetRealMemberRegistrationControllerWithFakeContext()
        {
            var controller = MemberRegistrationController();
            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            return controller;
        }

        private MemberRegistrationController GetRealMemberRegistrationControllerWithAjaxRequest()
        {
            var controller = MemberRegistrationController();
            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => request.Headers).Returns(new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
            A.CallTo(() => controllerContext.Request).Returns(request);
            return controller;
        }

        private class FakeMemberRegistrationController : MemberRegistrationController
        {
            public IWeeeClient ApiClient { get; private set; }

            public FakeMemberRegistrationController(
                IWeeeClient apiClient,
                IWeeeCache cache,
                BreadcrumbService breadcrumb,
                CsvWriterFactory csvWriterFactory,
                IMapper mapper)
                : base(() => apiClient,
                cache,
                breadcrumb,
                csvWriterFactory,
                 mapper)
            {
                ApiClient = apiClient;
            }

            public void InvokeOnActionExecuting(ActionExecutingContext filterContext)
            {
                OnActionExecuting(filterContext);
            }
        }
    }
}
