namespace EA.Weee.Web.Tests.Unit.Areas.PCS.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Services;
    using Web.Areas.PCS.Controllers;
    using Web.Areas.PCS.ViewModels;
    using Weee.Requests.MemberRegistration;
    using Weee.Requests.Organisations;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IFileConverterService fileConverter;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            fileConverter = A.Fake<IFileConverterService>();
        }

        [Fact]
        public async void GetManageMembers_ChecksForValidityOfOrganisation()
        {
            try
            {
                await HomeController().ManageMembers(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetManageMembers_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ManageMembers(A<Guid>._));
        }

        [Fact]
        public async void GetManageMembers_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ManageMembers(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostManageMembers_ModelIsInvalid_ReturnsView()
        {
            var controller = HomeController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var result = await controller.ManageMembers(A<Guid>._, new ManageMembersViewModel());

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostManageMembers_ConvertsFileToString()
        {
            try
            {
                await HomeController().ManageMembers(A<Guid>._, new ManageMembersViewModel());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostManageMembers_FileIsConvertedSuccessfully_ValidateRequestSentWithConvertedFileDataAndOrganisationId()
        {
            const string fileData = "myFileContent";
            var organisationId = Guid.NewGuid();
            var request = new ValidateXmlFile(A<Guid>._, A<string>._);

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .Returns(fileData);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Invokes((string token, IRequest<Guid> req) => request = (ValidateXmlFile)req);

            try
            {
                await HomeController().ManageMembers(organisationId, new ManageMembersViewModel());
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(request);
            Assert.Equal(fileData, request.Data);
            Assert.Equal(organisationId, request.OrganisationId);
        }

        [Fact]
        public async void PostManageMembers_ValidateRequestIsProcessedSuccessfully_RedirectsToResults()
        {
            var validationId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXmlFile>._))
                .Returns(validationId);

            var result = await HomeController().ManageMembers(A<Guid>._, new ManageMembersViewModel());
            var redirect = (RedirectToRouteResult)result;

            Assert.Equal("PCS", redirect.RouteValues["area"]);
            Assert.Equal("Home", redirect.RouteValues["controller"]);
            Assert.Equal("ViewErrorsAndWarnings", redirect.RouteValues["action"]);
            Assert.Equal(validationId, redirect.RouteValues["memberUploadId"]);
        }

        private HomeController HomeController()
        {
            var controller = new HomeController(() => weeeClient, fileConverter);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }
    }
}
