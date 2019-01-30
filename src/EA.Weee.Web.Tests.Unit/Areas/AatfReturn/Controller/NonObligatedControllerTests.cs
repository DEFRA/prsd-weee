namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Web.OAuth;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Owin.Security;
    using Requests.AatfReturn;
    using Requests.DataReturns;
    using System;
    using System.Security;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Xunit;

    public class NonObligatedControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly ConfigurationService configService;
        private readonly INonObligatedWeeRequestCreator requestCreator;
        private readonly NonObligatedController controller;

        public NonObligatedControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            mapper = A.Fake<IMapper>();
            configService = A.Fake<ConfigurationService>();
            requestCreator = A.Fake<INonObligatedWeeRequestCreator>();
            controller = new NonObligatedController(mapper, A.Fake<IOAuthClient>, () => weeeClient, A.Fake<IAuthenticationManager>(), A.Fake<IExternalRouteService>(), A.Fake<IAppConfiguration>(), requestCreator);
        }

        [Fact]
        public void CheckNonObligatedControllerInheritsExternalSiteController()
        {
            typeof(NonObligatedController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void Index_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new NonObligatedValuesViewModel();
            var request = new AddNonObligatedRequest();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
