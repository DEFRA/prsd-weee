namespace EA.Weee.Web.Tests.Unit.Areas.PCS.Controllers
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using FakeItEasy;
    using Web.Areas.PCS.Controllers;
    using Weee.Requests.Organisations;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
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

        private HomeController HomeController()
        {
            return new HomeController(() => weeeClient);
        }
    }
}
