namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Core.DataReturns;
    using Core.Shared;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using Prsd.Core.Mapper;
    using Services;
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
        private readonly ConfigurationService configService;

        public DataReturnsControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            configService = A.Fake<ConfigurationService>();
        }

        /// <summary>
        /// This test ensures that the OnActionExecuting method will throw an
        /// InvalidOperationException if the application configuration has "EnableDataReturns"
        /// set to false.
        /// </summary>
        [Fact]
        public void OnActionExecuting_ConfigDisabled_ThrowsException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(false);
            
            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act           
            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            Assert.Throws<InvalidOperationException>(testCode);
        }

        [Fact]
        public void OnActionExecuting_ConfigEnabledAndPcsIdNotSpecified_ThrowsArgumentException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act
            ActionExecutingContext actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();

            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { actionExecutingContext };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            Assert.Throws<ArgumentException>(testCode);
        }

        [Fact]
        public void OnActionExecuting_ConfigEnabledAndSpecifiedPcsIdNotAGuid_ThrowsArgumentException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act
            ActionExecutingContext actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            actionExecutingContext.ActionParameters["pcsId"] = 12345;

            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { actionExecutingContext };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            Assert.Throws<ArgumentException>(testCode);
        }

        [Fact]
        public void OnActionExecuting_ConfigEnabledAndSpecifiedPcsIdDoesNotExist_ThrowsArgumentException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .WhenArgumentsMatch(a => a.Get<VerifyOrganisationExists>("request").OrganisationId == new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9"))
                .Returns(false);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act
            ActionExecutingContext actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            actionExecutingContext.ActionParameters["pcsId"] = new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9");

            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { actionExecutingContext };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
        }
            };

            // Assert
            Assert.Throws<ArgumentException>(testCode);
        }

        /// <summary>
        /// This test ensures that the OnActionExecuting filter doesn't interupt the action when the configuration
        /// has data returns enabled, the specified "pcsId" parameter is for a non-approved scheme and the user
        /// is requesting the "AuthorisationRequired" action. This prevents an infinite loop from occuring.
        /// </summary>
        public void OnActionExecuting_ConfigEnabledAndSpecifiedSchemeIsNotApprovedAndActionIsAuthorisationRequired_DoesNothing()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .WhenArgumentsMatch(a => a.Get<VerifyOrganisationExists>("request").OrganisationId == new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9"))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeStatus>("request").PcsId == new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9"))
                .Returns(SchemeStatus.Pending);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act
            ActionDescriptor actionDescriptor = A.Fake<ActionDescriptor>();
            A.CallTo(() => actionDescriptor.ActionName == "AuthorisationRequired");

            ActionExecutingContext actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            actionExecutingContext.ActionParameters["pcsId"] = new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9");
            actionExecutingContext.ActionDescriptor = actionDescriptor;

            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { actionExecutingContext };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            Assert.Null(actionExecutingContext.Result);
        }

        /// <summary>
        /// This test ensures that the OnActionExecuting filter redirects the user to the "AuthorisationRequired" action
        /// when the configuration has data returns enabled, the specified "pcsId" parameter is for a non-approved scheme and the user
        /// is not already requesting the "AuthorizationRequired" action.
        /// </summary>
        public void OnActionExecuting_ConfigEnabledAndSpecifiedSchemeIsNotApprovedAndActionIsNotAuthorisationRequired_RedirectsToAuthorisationRequired()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnableDataReturns).Returns(true);

            ConfigurationService configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration).Returns(configuration);

            IWeeeClient weeeClient = A.Fake<IWeeeClient>();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .WhenArgumentsMatch(a => a.Get<VerifyOrganisationExists>("request").OrganisationId == new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9"))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .WhenArgumentsMatch(a => a.Get<GetSchemeStatus>("request").PcsId == new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9"))
                .Returns(SchemeStatus.Pending);

            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                configurationService);

            // Act
            ActionDescriptor actionDescriptor = A.Fake<ActionDescriptor>();
            A.CallTo(() => actionDescriptor.ActionName == "SomeOtherAction");

            ActionExecutingContext actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            actionExecutingContext.ActionParameters["pcsId"] = new Guid("51254A73-D885-4F9A-BC47-2787CB1416B9");
            actionExecutingContext.ActionDescriptor = actionDescriptor;

            MethodInfo onActionExecutingMethod = typeof(DataReturnsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Action testCode = () =>
            {
                object[] args = new object[] { actionExecutingContext };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
        }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            };

            // Assert
            RedirectToRouteResult redirectResult = actionExecutingContext.Result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("AuthorisationRequired", redirectResult.RouteValues["action"]);
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
        public async void GetAuthorizationRequired_SchemeIsWithdrawn_ReturnsViewWithWithdrawnStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Withdrawn);

            var result = await DataReturnsController().AuthorisationRequired(A<Guid>._);

            Assert.IsType<ViewResult>(result);

            var view = ((AuthorizationRequiredViewModel)((ViewResult)result).Model);

            Assert.Equal(SchemeStatus.Withdrawn, view.Status);
        }

        [Fact]
        public async void GetAuthorizationRequired_SchemeIsApproved_RedirectsToIndex()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeStatus>._))
                .Returns(SchemeStatus.Approved);

            var result = await DataReturnsController().AuthorisationRequired(A<Guid>._);

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("Index", routeValues["action"]);
        }

        /// <summary>
        /// This test ensures that the GET "Index" action returns the "Index" view.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIndex_Always_ReturnsIndexView()
        {
            // Arrange
            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                A.Dummy<ConfigurationService>());

            // Act
            ActionResult result = await controller.Index(A.Dummy<Guid>());

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals("Index", viewResult.ViewName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// This test ensures that the GET "Index" action calls the API to fetch summary
        /// data about the data returns for the specified organisation. It ensures that
        /// this information is populated into the IndexViewModel provided with the
        /// result.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetIndex_CallsApiToFetchSummary_AndPopulatesViewModel()
        {
            IWeeeClient client = A.Fake<IWeeeClient>();

            List<int> complianceYearsForScheme = new List<int>() { 2016, 2017, 2018 };

            A.CallTo(() => client.SendAsync(A<string>._, A<FetchDataReturnComplianceYearsForScheme>._))
                .WhenArgumentsMatch(a => a.Get<FetchDataReturnComplianceYearsForScheme>("request").PcsId == new Guid("BA7F772F-626D-4CBD-8D50-50A7B852A9AC"))
                .Returns(complianceYearsForScheme);

            // Arrange
            DataReturnsController controller = new DataReturnsController(
                () => client,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                A.Dummy<ConfigurationService>());

            // Act
            ActionResult result = await controller.Index(new Guid("BA7F772F-626D-4CBD-8D50-50A7B852A9AC"));

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            IndexViewModel viewModel = viewResult.Model as IndexViewModel;
            Assert.NotNull(viewModel);

            Assert.Equal(new Guid("BA7F772F-626D-4CBD-8D50-50A7B852A9AC"), viewModel.OrganisationId);
            Assert.Collection(viewModel.ComplianceYears,
                r1 => Assert.Equal(2016, r1),
                r2 => Assert.Equal(2017, r2),
                r3 => Assert.Equal(2018, r3));
        }

        [Fact]
        public async void GetUpload_IdDoesBelongToAnExistingOrganisationAndSubmissionWindowIsClose_RedirectToCannotSubmitDataReturn()
        {   
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsSubmissionWindowOpen>._))
                .Returns(false);

            var result = await DataReturnsController().Upload(A<Guid>._);

            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("CannotSubmitDataReturn", redirectValues["action"]);
        }

        [Fact]
        public async void GetUpload_IdDoesBelongToAnExistingOrganisationAndSubmissionWindowIsOpen_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IsSubmissionWindowOpen>._))
            .Returns(true);

            var result = await DataReturnsController().Upload(A<Guid>._);
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);

            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals("Upload", viewResult.ViewName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// This test ensures that the user is redireted to the GET Submit action
        /// following the upload of a data return that generates no errors during processing.
        /// </summary>
        [Fact]
        public async void PostUpload_WithDataReturnWithNoErrors_RedirectsToSubmit()
        {
            // Arrange   
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessDataReturnXmlFile>._))
                .Returns(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

            PCSFileUploadViewModel viewModel = new PCSFileUploadViewModel();

            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => controllerContext.Request).Returns(request);

            // Act
            ActionResult result = await controller.Upload(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), viewModel);

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Submit", redirectToRouteResult.RouteValues["action"]);
            Assert.Equal(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), redirectToRouteResult.RouteValues["pcsId"]);
            Assert.Equal(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"), redirectToRouteResult.RouteValues["dataReturnUploadId"]);
        }

        [Fact]
        public async void PostUpload_AjaxRequest_ModelIsInvalid_ReturnsError()
        {
            var controller = GetRealDataReturnsControllerWithAjaxRequest();

            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.Upload(A<Guid>._, new PCSFileUploadViewModel());

            Assert.IsType<HttpStatusCodeResult>(result);
        }

        /// <summary>
        /// This test ensures that the user is redireted to the GET Review action
        /// following the upload of a data return that generates errors during processing.
        /// </summary>
        [Fact]
        public async void PostUpload_WithDataReturnWithErrors_RedirectsToReview()
        {
            // Arrange   
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessDataReturnXmlFile>._))
                .Returns(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error },
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

            PCSFileUploadViewModel viewModel = new PCSFileUploadViewModel();

            var controllerContext = A.Fake<HttpContextBase>();
            controller.ControllerContext = new ControllerContext(controllerContext, new RouteData(), controller);
            var request = A.Fake<HttpRequestBase>();
            A.CallTo(() => controllerContext.Request).Returns(request);

            // Act
            ActionResult result = await controller.Upload(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), viewModel);

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Review", redirectToRouteResult.RouteValues["action"]);
            Assert.Equal(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), redirectToRouteResult.RouteValues["pcsId"]);
            Assert.Equal(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"), redirectToRouteResult.RouteValues["dataReturnUploadId"]);
        }

        [Fact]
        public async void PostUpload_AjaxRequest_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var dataReturnId = Guid.NewGuid();
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ProcessDataReturnXmlFile>._))
                .Returns(dataReturnId);

            var controller = GetRealDataReturnsControllerWithAjaxRequest();

            var result = await controller.Upload(A<Guid>._, new PCSFileUploadViewModel());

            Assert.IsType<JsonResult>(result);
        }

        /// <summary>
        /// This test ensures that an exception is thrown when the pcsId (organisation ID) provided
        /// to the controller action does not match the organisation ID of the specified data return.
        /// </summary>
        [Fact]
        public async void GetReview_WithDataReturnWithDifferentOrganisationId_ThrowsAnException()
        {
            // Arrange
            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

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
            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

            // Act
            ActionResult result = await controller.Review(
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Submit", redirectToRouteResult.RouteValues["action"]);
            Assert.Equal(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), redirectToRouteResult.RouteValues["pcsId"]);
            Assert.Equal(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"), redirectToRouteResult.RouteValues["dataReturnUploadId"]);
        }

        /// <summary>
        /// This test ensures that the user is shown the Submit view when they use the
        /// GET Review action for a data return with errors.
        /// </summary>
        [Fact]
        public async void GetReview_HappyPath_ReturnsSubmitViewWithPopulatedViewModel()
        {
            // Arrange           
            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error },
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

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
            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);
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
            DataReturnError error = new DataReturnError("Test Error");

            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                new List<DataReturnError>() { error },
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

            // Act
            ActionResult result = await controller.Submit(
                new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"),
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"));

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("Review", redirectToRouteResult.RouteValues["action"]);
            Assert.Equal(new Guid("DDE08793-D655-4CDD-A87A-083307C1AA66"), redirectToRouteResult.RouteValues["pcsId"]);
            Assert.Equal(new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"), redirectToRouteResult.RouteValues["dataReturnUploadId"]);
        }

        /// <summary>
        /// This test ensures that the GET Submit action returns the "Submit" view with a populated
        /// view model when a data return is sucessfully requested.
        /// </summary>
        [Fact]
        public async void GetSubmit_HappyPath_ReturnsSubmitViewWithPopulatedViewModel()
        {
            // Arrange          
            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

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
            DataReturnForSubmission dataReturnForSubmission = new DataReturnForSubmission(
                new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                2015,
                QuarterType.Q4,
                A.Dummy<IReadOnlyCollection<DataReturnWarning>>(),
                A.Dummy<IReadOnlyCollection<DataReturnError>>(),
                false);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<FetchDataReturnForSubmission>._))
                .WhenArgumentsMatch(args => args.Get<FetchDataReturnForSubmission>("request").DataReturnUploadId == new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"))
                .Returns(dataReturnForSubmission);

            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

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

        [Fact]
        public async Task PostSubmit_HappyPath_SubmitsDataReturnUpload()
        {
            // Arrange
            Guid dataReturnUploadId = Guid.NewGuid();
            var controller = BuildFakeDataReturnsController();

            // Act
            await controller.Submit(A.Dummy<Guid>(), dataReturnUploadId, A.Dummy<SubmitViewModel>());

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<SubmitDataReturnUpload>._))
                .WhenArgumentsMatch(args => args.Get<SubmitDataReturnUpload>("request").DataReturnUploadId == dataReturnUploadId)
                .MustHaveHappened();
        }

        /// <summary>
        /// This test ensures that the POST Submit action redirects the user to the GET SuccessfulSubmission
        /// action after a successful submission.
        /// </summary>
        [Fact]
        public async void PostSubmit_HappyPath_RedirectsToSuccessfulSubmission()
        {
            // Arrange
            DataReturnsController controller = GetDummyDataReturnsController(weeeClient);

            // Act
            ActionResult result = await controller.Submit(
                    new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"),
                    new Guid("06FFB265-46D3-4CE3-805A-A81F1B11622A"),
                    new SubmitViewModel());

            // Assert
            Assert.IsAssignableFrom<RedirectToRouteResult>(result);
            RedirectToRouteResult redirectToRouteResult = result as RedirectToRouteResult;

            Assert.Equal("SuccessfulSubmission", redirectToRouteResult.RouteValues["action"]);
            Assert.Equal(new Guid("AA7DA88A-19AF-4130-A24D-45389D97B274"), redirectToRouteResult.RouteValues["pcsId"]);
        }

        /// <summary>
        /// This test ensures that the GET "DownloadEeeWeeeData" action calls the
        /// API to retrieve the file data for the specified organisation and compliance
        /// year and provides the data in the result.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadEeeWeeeData_Always_CallsApiAndReturnsFileContents()
        {
            // Arrange
            Guid organisationId = new Guid("ADED8BDE-CF03-4696-B972-DDAB9306A6DD");

            FileInfo fileInfo = new FileInfo("Test file.csv", A.Dummy<byte[]>());

            IWeeeClient client = A.Fake<IWeeeClient>();
            A.CallTo(() => client.SendAsync(A<string>._, A<FetchSummaryCsv>._))
                .WhenArgumentsMatch(a => a.Get<FetchSummaryCsv>("request").OrganisationId == organisationId
                    && a.Get<FetchSummaryCsv>("request").ComplianceYear == 2017)
                .Returns(fileInfo);

            DataReturnsController controller = new DataReturnsController(
                () => client,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                A.Dummy<ConfigurationService>());

            // Act
            ActionResult result = await controller.DownloadEeeWeeeData(organisationId, 2017);

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("Test file.csv", fileResult.FileDownloadName);
        }
       
        /// <summary>
        /// This test ensures that the GET "DownloadEeeWeeeData" returns
        /// a FileResult with a content type of "text/csv".
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetDownloadEeeWeeeData_Always_ReturnsFileWithContentTypeTextCsv()
        {
            // Arrange
            DataReturnsController controller = new DataReturnsController(
                () => A.Dummy<IWeeeClient>(),
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                A.Dummy<ConfigurationService>());

            // Act
            ActionResult result = await controller.DownloadEeeWeeeData(A.Dummy<Guid>(), A.Dummy<int>());

            // Assert
            FileResult fileResult = result as FileResult;
            Assert.NotNull(fileResult);

            Assert.Equal("text/csv", fileResult.ContentType);
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

        private DataReturnsController DataReturnsController(object viewModel)
        {
            DataReturnsController controller = DataReturnsController();

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

        private DataReturnsController DataReturnsController()
        {
            configService.CurrentConfiguration.EnableDataReturns = true;
            DataReturnsController controller = new DataReturnsController(
                 () => weeeClient,
                 A.Fake<IWeeeCache>(),
                 A.Fake<BreadcrumbService>(),
                 A.Fake<CsvWriterFactory>(),
                 mapper,
                 configService);

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }

        private static DataReturnsController GetDummyDataReturnsController(IWeeeClient weeeClient)
        {
            DataReturnsController controller = new DataReturnsController(
                () => weeeClient,
                A.Dummy<IWeeeCache>(),
                A.Dummy<BreadcrumbService>(),
                A.Dummy<CsvWriterFactory>(),
                A.Dummy<IMapper>(),
                A.Dummy<ConfigurationService>());

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
                IMapper mapper,
                ConfigurationService configurationService)
                : base(() => apiClient,
                cache,
                breadcrumb,
                csvWriterFactory,
                 mapper,
                 configurationService)
            {
                ApiClient = apiClient;
            }

            public void InvokeOnActionExecuting(ActionExecutingContext filterContext)
            {
                OnActionExecuting(filterContext);
            }
        }

        private FakeDataReturnsController BuildFakeDataReturnsController()
        {
            var controller = new FakeDataReturnsController
               (weeeClient,
                A.Fake<IWeeeCache>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<CsvWriterFactory>(),
                mapper,
                configService);

            new HttpContextMocker().AttachToController(controller);

            return controller;
        }
    }
}
