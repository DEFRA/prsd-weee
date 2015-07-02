namespace EA.Weee.Web.Tests.Unit.Areas.PCS.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using Services;
    using Web.Areas.PCS.Controllers;
    using Weee.Requests.MemberRegistration;
    using Weee.Requests.Organisations;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IFileConverter fileConverter;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            fileConverter = A.Fake<IFileConverter>();
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
        public async void PostManageMembers_ConvertFileToString()
        {
            try
            {
                await HomeController().ManageMembers(A<Guid>._, A<HttpPostedFileBase>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => fileConverter.Convert(A<HttpPostedFileBase>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostManageMembers_FileIsConvertedSuccessfully_ValidateRequestSent()
        {
            try
            {
                await HomeController().ManageMembers(A<Guid>._, A<HttpPostedFileBase>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<ValidateXMLFile>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        private HomeController HomeController()
        {
            return new HomeController(() => weeeClient, fileConverter);
        }
    }
}
