namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Controllers;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;
 
    public class CheckYourReturnControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly CheckYourReturnController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly IMapper mapper;

        public CheckYourReturnControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            controller = new CheckYourReturnController(() => weeeClient, A.Fake<IWeeeCache>(), breadcrumb, mapper);
        }

        [Fact]
        public void CheckCheckYourReturnControllerInheritsExternalSiteController()
        {
            typeof(CheckYourReturnController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexGet_GivenActionExecutes_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().Be("Index");
        }

        [Fact]
        public async void IndexGet_GivenReturn_ApiShouldBeCalledWithReturnRequest()
        {
            var returnId = Guid.NewGuid();

            await controller.Index(returnId, Guid.NewGuid());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>.That.Matches(g => g.ReturnId.Equals(returnId))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckYourReturnViewModelShouldBeBuilt()
        {
            var returnData = new ReturnData();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => mapper.Map<CheckYourReturnViewModel>(returnData)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var returnId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, returnId);

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexGet_GivenReturn_CheckYourReturnViewModelShouldBeReturned()
        {
            var model = A.Fake<CheckYourReturnViewModel>();

            A.CallTo(() => mapper.Map<CheckYourReturnViewModel>(A<ReturnData>._)).Returns(model);

            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().Be(model);
        }
    }
}
