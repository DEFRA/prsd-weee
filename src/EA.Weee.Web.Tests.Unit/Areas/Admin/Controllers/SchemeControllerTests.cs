namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Scheme;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels;
    using Weee.Requests.Scheme;
    using Xunit;

    public class SchemeControllerTests
    {
        private readonly IWeeeClient weeeClient;

        public SchemeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
        }

        [Fact]
        public async Task ManageSchemesPost_AllGood_ReturnsManageSchemeRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = SchemeController();

            var result = await controller.ManageSchemes(new ManageSchemesViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("EditScheme", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["id"]);
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            var controller = SchemeController();
            controller.ModelState.AddModelError(string.Empty, "Some error");

            var result = await controller.ManageSchemes(new ManageSchemesViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void GetEditScheme_ReturnsView_WithManageSchemeModel()
        {
            var schemeId = Guid.NewGuid();

            var controller = SchemeController();

            var result = await controller.EditScheme(schemeId);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;

            Assert.IsType<SchemeViewModel>(model);
        }

        [Theory]
        [InlineData(SchemeStatus.Approved)]
        [InlineData(SchemeStatus.Rejected)]
        public async void GetEditScheme_StatusIsRejectedOrApproved_StatusIsUnchangable(SchemeStatus status)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<SchemeData>>._))
                .Returns(new SchemeData
                {
                    SchemeStatus = status
                });

            var result = await SchemeController().EditScheme(Guid.NewGuid());
            var model = (SchemeViewModel)((ViewResult)result).Model;

            Assert.Equal(true, model.IsUnchangeableStatus);
        }

        [Theory]
        [InlineData(SchemeStatus.Approved)]
        [InlineData(SchemeStatus.Pending)]
        public async void PostEditScheme_ModelWithError_AndSchemeIsNotRejected_ReturnsView(SchemeStatus status)
        {
            var controller = SchemeController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var schemeId = Guid.NewGuid();
            var result = await controller.EditScheme(schemeId, new SchemeViewModel
            {
                Status = status,
            });

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostEditScheme_OldApprovalNumberAndApprovalNumberNotMatch_MustVerifyApprovalNumberExists()
        {
            var controller = SchemeController();

            var scheme = new SchemeViewModel
            {
                OldApprovalNumber = "WEE/AD1234DC/SCH",
                ApprovalNumber = "WEE/ZZ3456EE/SCH",
                SchemeName = "Any value",
                ObligationType = ObligationType.B2B,
                CompetentAuthorityId = Guid.NewGuid(),
                IbisCustomerReference = "Any value"
            };

            await controller.EditScheme(Guid.NewGuid(), scheme);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyApprovalNumberExists>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_ButSchemeIsRejected_RedirectsToRejectionConfirmation_WithSchemeId()
        {
            var controller = SchemeController();
            var schemeId = Guid.NewGuid();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var result = await controller.EditScheme(schemeId, new SchemeViewModel
            {
                Status = SchemeStatus.Rejected,
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ConfirmRejection", routeValues["action"]);
            Assert.Equal("Scheme", routeValues["controller"]);
            Assert.Equal(schemeId, routeValues["id"]);
        }

        [Fact]
        public async void PostEditScheme_ModelWithNoError_ButSchemeIsRejected_RedirectsToRejectionConfirmation_WithSchemeId()
        {
            var controller = SchemeController();
            var schemeId = Guid.NewGuid();
            var result = await controller.EditScheme(schemeId, new SchemeViewModel
            {
                Status = SchemeStatus.Rejected,
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ConfirmRejection", routeValues["action"]);
            Assert.Equal("Scheme", routeValues["controller"]);
            Assert.Equal(schemeId, routeValues["id"]);
        }

        [Fact]
        public async void HttpPost_ConfirmRejection_SendsSetStatusRequest_WithRejectedStatus_AndRedirectsToManageSchemes()
        {
            var status = SchemeStatus.Pending;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .Invokes((string t, IRequest<Guid> s) => status = ((SetSchemeStatus)s).Status)
                .Returns(Guid.NewGuid());

            var result = await SchemeController().ConfirmRejection(Guid.NewGuid(), new ConfirmRejectionViewModel());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(SchemeStatus.Rejected, status);
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageSchemes", routeValues["action"]);
            Assert.Equal("Scheme", routeValues["controller"]);
        }

        private SchemeController SchemeController()
        {
            return new SchemeController(() => weeeClient);
        }
    }
}
