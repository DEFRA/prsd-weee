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
    using Weee.Requests.Organisations;
    using Xunit;

    public class HomeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IFileConverterService fileConverter;

        public HomeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async void GetChooseActivity_ChecksForValidityOfOrganisation()
        {
            try
            {
                await HomeController().ChooseActivity(A<Guid>._);
            }
            catch (Exception)
            {
            }

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void GetChooseActivity_IdDoesNotBelongToAnExistingOrganisation_ThrowsException()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(false);

            await Assert.ThrowsAnyAsync<Exception>(() => HomeController().ChooseActivity(A<Guid>._));
        }

        [Fact]
        public async void GetChooseActivity_IdDoesBelongToAnExistingOrganisation_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            var result = await HomeController().ChooseActivity(A<Guid>._);

            Assert.IsType<ViewResult>(result);
        }
        
        private HomeController HomeController()
        {
            var controller = new HomeController(() => weeeClient);
            new HttpContextMocker().AttachToController(controller);

            return controller;
        }
    }
}
