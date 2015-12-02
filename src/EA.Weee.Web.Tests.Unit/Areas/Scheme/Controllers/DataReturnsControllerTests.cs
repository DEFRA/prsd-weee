namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using Api.Client;
    using Core.DataReturns;
    using Core.Shared;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using TestHelpers;
    using Web.Areas.Scheme.Controllers;
    using Web.Areas.Scheme.ViewModels;
    using Web.Areas.Scheme.ViewModels.DataReturns;
    using Weee.Requests.DataReturns;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Xunit;

    public class DataReturnsControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;

        public DataReturnsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
        }
        
        [Fact]
        public async void GetAuthorisationRequired_ChecksStatusOfScheme()
        {
            await DataReturnsController().AuthorisationRequired(A<Guid>._);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsPendingApproval_ReturnsViewWithPendingStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Pending);

            var result = await DataReturnsController().AuthorisationRequired(A<Guid>._);

            Assert.IsType<ViewResult>(result);

            var view = ((AuthorizationRequiredViewModel)((ViewResult)result).Model);

            Assert.Equal(SchemeStatus.Pending, view.Status);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsRejected_ReturnsViewWithRejectedStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Rejected);

            var result = await DataReturnsController().AuthorisationRequired(A<Guid>._);

            Assert.IsType<ViewResult>(result);

            var view = ((AuthorizationRequiredViewModel)((ViewResult)result).Model);

            Assert.Equal(SchemeStatus.Rejected, view.Status);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsApproved_RedirectsToPcsMemberSummary()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Approved);

            var result = await DataReturnsController().AuthorisationRequired(A<Guid>._);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("SubmitDataReturns", routeValues["action"]);
            Assert.Equal("DataReturns", routeValues["controller"]);
        }

        [Fact]
        public async void GetUpload_ChecksForValidityOfOrganisation()
        {
            try
            {
                await DataReturnsController().Upload(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetUpload_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => DataReturnsController().Upload(A<Guid>._));
        }

        [Fact]
        public async void GetUpload_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await DataReturnsController().Upload(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void OnActionExecuting_ActionAuthorizationRequired_DoesNotCheckPcsId()
        {
            var fakeController = BuildFakeDataReturnsController();
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
            var fakeController = BuildFakeDataReturnsController();
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
            var fakeController = BuildFakeDataReturnsController();
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
            var fakeController = BuildFakeDataReturnsController();
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

        /// <summary>
        /// This test ensures that the user is redireted to the GET Submit action
        /// following the upload of a data return that generates no errors during processing.
        /// </summary>
        [Fact]
        public async void PostUpload_WithDataReturnWithNoErrors_RedirectsToSubmit()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessDataReturnsXMLFile>._))
                .Returns(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            PCSFileUploadViewModel viewModel = new PCSFileUploadViewModel();

            // Act
            ActionResult result = await controller.Upload(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), viewModel);

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Submit", redirectToRouteResult.RouteValues["action"]);
        }

        /// <summary>
        /// This test ensures that the user is redireted to the GET Review action
        /// following the upload of a data return that generates errors during processing.
        /// </summary>
        [Fact]
        public async void PostUpload_WithDataReturnWithErrors_RedirectsToReview()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessDataReturnsXMLFile>._))
                .Returns(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            PCSFileUploadViewModel viewModel = new PCSFileUploadViewModel();

            // Act
            ActionResult result = await controller.Upload(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), viewModel);

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Review", redirectToRouteResult.RouteValues["action"]);
        }

        /// <summary>
        /// This test ensures that an exception is thrown when the pcsId (organisation ID) provided
        /// to the controller action does not match the organisation ID of the specified data return.
        /// </summary>
        [Fact]
        public async void GetReview_WithDataReturnWithDifferentOrganisationId_ThrowsAnException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            Func<Task<ActionResult>> testCode = async () =>
                await controller.Review(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(testCode);
        }

        /// <summary>
        /// This test ensures that the user is redirected to the GET Submit action
        /// if they use the GET Review action for a data return with no errors.
        /// </summary>
        [Fact]
        public async void GetReview_WithDataReturnWithNoErrors_RedirectsToSubmit()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.Review(
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Submit", redirectToRouteResult.RouteValues["action"]);
        }

        /// <summary>
        /// This test ensures that the user is shown the Submit view when they use the
        /// GET Review action for a data return with errors.
        /// </summary>
        [Fact]
        public async void GetReview_HappyPath_ReturnsSubmitViewWithPopulatedViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.Review(
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.True(viewResult.ViewName == "Submit",
                "The GET Review action must return the view called \"Submit\" when the data return has no errors.");

            Assert.IsAssignableFrom<SubmitViewModel>(viewResult.Model);
            SubmitViewModel viewModel = viewResult.Model as SubmitViewModel;

            Assert.Equal(dataReturnForSubmission, viewModel.DataReturn);
        }

        /// <summary>
        /// This test ensures that an exception is thrown when the pcsId (organisation ID) provided
        /// to the controller action does not match the organisation ID of the specified data return.
        /// </summary>
        [Fact]
        public async void GetSubmit_WithDataReturnWithDifferentOrganisationId_ThrowsAnException()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            Func<Task<ActionResult>> testCode = async () =>
                await controller.Submit(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(testCode);
        }

        /// <summary>
        /// This test ensures that the user is redirected to the GET Review action
        /// if they use the GET Submit action for a data return with one or more errors.
        /// </summary>
        [Fact]
        public async void GetSubmit_WithDataReturnWithErrors_RedirectsToReview()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.Submit(
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Review", redirectToRouteResult.RouteValues["action"]);
        }

        /// <summary>
        /// This test ensures that the GET Submit action returns the "Submit" view with a populated
        /// view model when a data return is sucessfully requested.
        /// </summary>
        [Fact]
        public async void GetSubmit_HappyPath_ReturnsSubmitViewWithPopulatedViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.Submit(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "Submit",
                "The GET Submit action must return the view called \"Submit\".");

            Assert.IsAssignableFrom<SubmitViewModel>(viewResult.Model);
            SubmitViewModel viewModel = viewResult.Model as SubmitViewModel;

            Assert.Equal(dataReturnForSubmission, viewModel.DataReturn);
        }

        /// <summary>
        /// This test ensures that the POST Submit action returns the "Submit" view with a populated
        /// view model when the model state is invalid.
        /// </summary>
        [Fact]
        public async void PostSubmit_WithInvalidModelState_ReturnsSubmitViewWithPopulatedViewModel()
        {
            // Arrange
            IWeeeClient weeeClient = A.Fake<IWeeeClient>();

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                new Quarter(2015, QuarterType.Q4),
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            controller.ModelState.AddModelError("Key", "Some Error");

            // Act
            ActionResult result = await controller.Submit(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                    new SubmitViewModel());

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "Submit",
                "The POST Submit action must return the view called \"Submit\" when the model state is invalid.");

            Assert.IsAssignableFrom<SubmitViewModel>(viewResult.Model);
            SubmitViewModel viewModel = viewResult.Model as SubmitViewModel;

            Assert.Equal(dataReturnForSubmission, viewModel.DataReturn);
        }

        /// <summary>
        /// This test ensures that the POST Submit action returns the "Submitted" view after
        /// a successful submission.
        /// </summary>
        [Fact]
        public async void PostSubmit_HappyPath_ReturnsSubmittedView()
        {
            // Arrange
            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            // Act
            ActionResult result = await controller.Submit(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                    new SubmitViewModel());

            // Assert
            Assert.IsAssignableFrom<ViewResultBase>(result);
            ViewResultBase viewResult = result as ViewResultBase;

            Assert.True(viewResult.ViewName == "Submitted",
                "The POST Submit action must return the view called \"Submitted\" after a successful submission.");
        }

        private DataReturnsController GetRealDataReturnsControllerWithFakeContext()
        {
            var controller = DataReturnsController();
            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            return controller;
        }

        private DataReturnsController GetRealDataReturnsControllerWithAjaxRequest()
        {
            var controller = DataReturnsController();
            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => request.Headers).Returns(new WebHeaderCollection { { "X-Requested-With", "XMLHttpRequest" } });
            A.CallTo(() => controllerContext.Request).Returns(request);
            return controller;
        }

        private DataReturnsController DataReturnsController()
        {
            var controller = new DataReturnsController(
                () => weeeClient,
                A.Fake<IWeeeCache>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<CsvWriterFactory>(),
                 mapper);

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private DataReturnsController DataReturnsController(object viewModel)
        {
            var controller = new DataReturnsController(
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

        private FakeDataReturnsController BuildFakeDataReturnsController()
        {
            var controller = new FakeDataReturnsController
               (weeeClient,
                A.Fake<IWeeeCache>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<CsvWriterFactory>(),
                A.Fake<IMapper>());

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private class FakeDataReturnsController : DataReturnsController
        {
            public IWeeeClient ApiClient { get; private set; }

            public FakeDataReturnsController(
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
