namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using AutoFixture;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.ViewModels.Obligations;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Core.Admin.Obligation;
    using Prsd.Core.Mapper;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Xunit;

    public class ObligationsControllerTests
    {
        private readonly IAppConfiguration configuration;
        private readonly Func<IWeeeClient> apiClient;
        private readonly IWeeeClient client;
        private readonly IWeeeCache cache;
        private readonly IMapper mapper;
        private readonly Fixture fixture;
        private readonly BreadcrumbService breadcrumb;
        private readonly ObligationsController controller;

        public ObligationsControllerTests()
        {
            configuration = A.Fake<IAppConfiguration>();
            client = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            apiClient = () => client;
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();

            fixture = new Fixture();

            controller = new ObligationsController(configuration, breadcrumb, cache, apiClient, mapper);
        }

        [Fact]
        public void Controller_ShouldInheritFromObligationsBaseController()
        {
            typeof(ObligationsController).Should().BeDerivedFrom<ObligationsBaseController>();
        }

        [Fact]
        public void ChooseAuthorityGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("SelectAuthority", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void HoldingPost_IsDecoratedWith_HttpPostAttribute()
        {
            typeof(ObligationsController).GetMethod("SelectAuthority", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(SelectAuthorityViewModel) }, null)
            .Should()
            .BeDecoratedWith<HttpPostAttribute>();
        }

        /// <summary>
        /// Ensures that the OnActionExecuting method will throw an InvalidOperationException 
        /// if the application configuration has "EnablePCSObligations" set to false.
        /// </summary>
        [Fact]
        public void OnActionExecuting_WhenPCSObligationsDisabledInConfig_ThrowsInvalidOperationException()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(false);
            ObligationsController controller = new ObligationsController(configuration, A.Dummy<BreadcrumbService>(), A.Dummy<IWeeeCache>(), () => A.Dummy<IWeeeClient>(), mapper);
            MethodInfo onActionExecutingMethod = typeof(ObligationsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var exception = Record.Exception(() =>
            {
                object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            });

            // Assert
            Assert.Equal("PCS Obligations is not enabled.", exception.Message);
            exception.Should().BeOfType<InvalidOperationException>();
        }

        /// <summary>
        /// Ensures that the OnActionExecuting method will not throw an exception 
        /// when application configuration has "EnablePCSObligations" set to true.
        /// </summary>
        [Fact]
        public void OnActionExecuting_WhenPCSObligationsEnabledInConfig_NoExceptionThrown()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(true);
            ObligationsController controller = new ObligationsController(configuration, A.Dummy<BreadcrumbService>(), A.Dummy<IWeeeCache>(), () => A.Dummy<IWeeeClient>(), mapper);
            MethodInfo onActionExecutingMethod = typeof(ObligationsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            Action testCode = () =>
            {
                object[] args = new object[] { A.Dummy<ActionExecutingContext>() };
                try
                {
                    onActionExecutingMethod.Invoke(controller, args);
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            };

            // Assert - No exception.
            testCode.Should().NotThrow<InvalidOperationException>();
        }

        /// <summary>
        /// Ensures that the OnActionExecuting method sets the InternalActivity
        /// property of the breadcrumb to "Manage PCS obligations".
        /// </summary>
        [Fact]
        public void OnActionExecuting_Always_SetsBreadcrumbInternalActivityToManageObligations()
        {
            // Arrange
            IAppConfiguration configuration = A.Fake<IAppConfiguration>();
            A.CallTo(() => configuration.EnablePCSObligations).Returns(true);
            BreadcrumbService breadcrumb = new BreadcrumbService();
            ObligationsController controller = new ObligationsController(configuration, breadcrumb, A.Dummy<IWeeeCache>(), () => A.Dummy<IWeeeClient>(), mapper);
            MethodInfo onActionExecutingMethod = typeof(ObligationsController).GetMethod(
                "OnActionExecuting",
                BindingFlags.NonPublic | BindingFlags.Instance);
            object[] args = new object[] { A.Dummy<ActionExecutingContext>() };

            // Act
            try
            {
                onActionExecutingMethod.Invoke(controller, args);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            // Assert
            Assert.Equal("Manage PCS obligations", breadcrumb.InternalActivity);
        }

        /// <summary>
        /// This test ensures that the GET "SelectAuthority" action always returns
        /// "SelectAuthority" view with a view model.
        /// </summary>
        [Fact]
        public void GetSelectAuthority_Always_ReturnsSelectAuthorityViewWithViewModel()
        {
            // Arrange
            ObligationsController controller = new ObligationsController(
                                                                        A.Dummy<IAppConfiguration>(),
                                                                        A.Dummy<BreadcrumbService>(),
                                                                        A.Dummy<IWeeeCache>(), 
                                                                        () => A.Dummy<IWeeeClient>(),
                                                                        mapper);

            // Act
            ActionResult result = controller.SelectAuthority();

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "SelectAuthority");
            Assert.NotNull(viewResult.Model as SelectAuthorityViewModel);
        }

        [Fact]
        public void PostSelectAuthority_WithInvalidModel_SelectAuthorityViewWithViewModel()
        {
            // Arrange
            
            ObligationsController controller = new ObligationsController(
                                                                        A.Dummy<IAppConfiguration>(),
                                                                        A.Dummy<BreadcrumbService>(),
                                                                        A.Dummy<IWeeeCache>(), 
                                                                        () => A.Dummy<IWeeeClient>(),
                                                                        mapper);
            var model = fixture.Create<SelectAuthorityViewModel>();
            controller.ModelState.AddModelError("error", new Exception());

            // Act - holding page until obligations page is implemented
            ActionResult result = controller.SelectAuthority(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(viewResult.ViewName == string.Empty || viewResult.ViewName == "SelectAuthority");  
            Assert.True(viewResult.Model.Equals(model));
        }

        [Fact]
        public void PostSelectAuthority_WithValidModel_RedirectsToUploadObligationsAction()
        {
            // Arrange
            var model = fixture.Create<SelectAuthorityViewModel>();

            var result = controller.SelectAuthority(model) as RedirectToRouteResult;

            // Assert
            result.RouteValues["action"].Should().Be("UploadObligations");
            result.RouteValues["authority"].Should().Be(model.SelectedAuthority.Value);
        }

        [Fact]
        public void UploadObligationsGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("UploadObligations", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(CompetentAuthority), typeof(Guid?) }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void UploadObligationsPost_IsDecoratedWith_HttpPostAttribute()
        {
            typeof(ObligationsController).GetMethod("UploadObligations", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(UploadObligationsViewModel) }, null)
            .Should()
            .BeDecoratedWith<HttpPostAttribute>();
        }

        [Fact]
        public void UploadObligationsPost_IsDecoratedWith_ValidateAntiForgeryTokenAttribute()
        {
            typeof(ObligationsController).GetMethod("UploadObligations", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(UploadObligationsViewModel) }, null)
            .Should()
            .BeDecoratedWith<ValidateAntiForgeryTokenAttribute>();
        }

        [Fact]
        public void DownloadTemplateGet_IsDecoratedWith_HttpGetAttribute()
        {
            typeof(ObligationsController).GetMethod("DownloadTemplate", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { typeof(CompetentAuthority) }, null)
            .Should()
            .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task UploadObligationsGet_GivenSelectedAuthorityAndNoObligationUploadId_ModelShouldBeMapped()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();

            //act
            await controller.UploadObligations(authority, fixture.Create<Guid>());

            //assert
            A.CallTo(() =>
                    mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(
                        A<UploadObligationsViewModelMapTransfer>.That.Matches(u => u.CompetentAuthority == authority)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UploadObligationsGet_GivenSelectedAuthorityAndObligationUploadId_SchemeUploadObligationShouldBeRetrieved()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();
            var obligationId = fixture.Create<Guid>();

            //act
            await controller.UploadObligations(authority, obligationId);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetSchemeObligationUpload>.That.Matches(s => s.ObligationUploadId == obligationId)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UploadObligationsGet_GivenSelectedAuthorityAndObligationUploadId_ModelShouldBeMapped()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();
            var obligationId = fixture.Create<Guid>();
            var schemeUploadObligationData = fixture.CreateMany<SchemeObligationUploadErrorData>().ToList();

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeObligationUpload>._)).Returns(schemeUploadObligationData);

            //act
            await controller.UploadObligations(authority, obligationId);

            //assert
            A.CallTo(() =>
                    mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(
                        A<UploadObligationsViewModelMapTransfer>.That.Matches(u => u.CompetentAuthority == authority && u.ErrorData == schemeUploadObligationData)))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UploadObligationsGet_GivenSelectedAuthorityAndObligationUploadId_ModelShouldBeReturned()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();
            var obligationId = fixture.Create<Guid>();
            var schemeUploadObligationData = fixture.CreateMany<SchemeObligationUploadErrorData>().ToList();
            var model = ValidUploadObligationsViewModel();

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeObligationUpload>._)).Returns(schemeUploadObligationData);

            A.CallTo(() =>
                    mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(
                        A<UploadObligationsViewModelMapTransfer>._)).Returns(model);

            //act
            var viewResult = await controller.UploadObligations(authority, obligationId) as ViewResult;

            //assert
            viewResult.Model.Should().Be(model);
        }

        [Fact]
        public async Task UploadObligationsGet_GivenSelectedAuthorityAndNoObligationUploadId_ModelShouldBeReturned()
        {
            //arrange
            var authority = fixture.Create<CompetentAuthority>();
            var model = ValidUploadObligationsViewModel();

            A.CallTo(() =>
                    mapper.Map<UploadObligationsViewModelMapTransfer, UploadObligationsViewModel>(
                        A<UploadObligationsViewModelMapTransfer>._)).Returns(model);

            //act
            var result = await controller.UploadObligations(authority, fixture.Create<Guid>()) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task DownloadTemplateGet_ShouldCallHandler_Once()
        {
            //arrange
            var authority = CompetentAuthority.England;
            A.CallTo(() => client.SendAsync(A<string>._, A<GetPcsObligationsCsv>.That.Matches(c => c.Authority.Equals(authority)))).Returns(new CSVFileData() { FileContent = "Test", FileName = "Test" });

            //act
            await controller.DownloadTemplate(authority);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, A<GetPcsObligationsCsv>.That.Matches(c => c.Authority.Equals(authority)))).MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task UploadObligations_GivenInvalidViewModel_DefaultViewShouldBeReturned()
        {
            //arrange
            var model = new UploadObligationsViewModel();
            controller.ModelState.AddModelError("error", new Exception());
            
            //act
            var result = await controller.UploadObligations(model) as ViewResult;

            //assert
            result.ViewName.Should().Be(string.Empty);
        }

        [Fact]
        public async Task UploadObligations_GivenInvalidViewModel_ModelShouldBeReturned()
        {
            //arrange
            var model = new UploadObligationsViewModel();
            controller.ModelState.AddModelError("error", new Exception());

            //act
            var result = await controller.UploadObligations(model) as ViewResult;

            //assert
            result.Model.Should().Be(model);
        }

        [Fact]
        public async Task UploadObligations_GivenModelFilePropertyHasError_ModelDisplaySelectFileErrorShouldBeTrue()
        {
            //arrange
            var model = new UploadObligationsViewModel();
            controller.ModelState.AddModelError("File", new Exception());

            //act
            var result = await controller.UploadObligations(model) as ViewResult;

            //assert
            ((UploadObligationsViewModel)result.Model).DisplaySelectFileError.Should().BeTrue();
        }

        [Fact]
        public async Task UploadObligations_GivenValidModel_RequestShouldBeMapped()
        {
            //arrange
            var model = ValidUploadObligationsViewModel();

            //act
            await controller.UploadObligations(model);

            //assert
            A.CallTo(() => mapper.Map<UploadObligationsViewModel, SubmitSchemeObligation>(model))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UploadObligations_GivenValidModelAndSubmitObligationRequest_ApiShouldBeCalled()
        {
            //arrange
            var model = ValidUploadObligationsViewModel();
            var request = fixture.Create<SubmitSchemeObligation>();

            A.CallTo(() => mapper.Map<UploadObligationsViewModel, SubmitSchemeObligation>(model)).Returns(request);

            //act
            await controller.UploadObligations(model);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._, request)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UploadObligations_GivenValidModel_ShouldRedirectToUploadObligationsGet()
        {
            //arrange
            var model = ValidUploadObligationsViewModel();
            var obligationUploadId = fixture.Create<Guid>();

            A.CallTo(() => client.SendAsync(A<string>._, A<SubmitSchemeObligation>._)).Returns(obligationUploadId);

            //act
            var result = await controller.UploadObligations(model) as RedirectToRouteResult;

            //assert
            result.RouteValues["action"].Should().Be("UploadObligations");
            result.RouteValues["authority"].Should().Be(model.Authority);
            result.RouteValues["id"].Should().Be(obligationUploadId);
        }

        [Fact]
        public async Task UploadObligations_GivenModelFilePropertyDoesNotHaveError_ModelDisplaySelectFileErrorShouldBeFalse()
        {
            //arrange
            var model = new UploadObligationsViewModel();
            controller.ModelState.AddModelError("NotFile", new Exception());

            //act
            var result = await controller.UploadObligations(model) as ViewResult;

            //assert
            ((UploadObligationsViewModel)result.Model).DisplaySelectFileError.Should().BeFalse();
        }

        private UploadObligationsViewModel ValidUploadObligationsViewModel()
        {
            var model = new UploadObligationsViewModel()
            {
                File = A.Fake<HttpPostedFileBase>(),
                Authority = fixture.Create<CompetentAuthority>()
            };
            return model;
        }
    }
}
