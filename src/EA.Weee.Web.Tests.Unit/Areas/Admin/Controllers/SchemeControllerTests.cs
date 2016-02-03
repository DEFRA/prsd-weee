namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using Prsd.Core.Mediator;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Scheme;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
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
            Assert.Equal(selectedGuid, redirectValues["schemeId"]);
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

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;

            Assert.IsType<SchemeViewModel>(model);
        }

        [Fact]
        public async void GetEditScheme_NullSchemeId_RedirectsToManageSchemes()
        {
            var controller = SchemeController();

            var result = await controller.EditScheme(null);

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("ManageSchemes", redirectValues["action"]);
        }

        [Theory]
        [InlineData(SchemeStatus.Withdrawn)]
        [InlineData(SchemeStatus.Rejected)]
        public async void GetEditScheme_StatusIsRejectedOrWithdrawn_StatusIsUnchangable(SchemeStatus status)
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

        [Fact]
        public async void GetEditScheme_StatusIsApproved_AvailableStatusToChangeIsWithdrawn()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<SchemeData>>._))
                .Returns(new SchemeData
                {
                    SchemeStatus = SchemeStatus.Approved
                });

            var result = await SchemeController().EditScheme(Guid.NewGuid());
            var model = (SchemeViewModel)((ViewResult)result).Model;

            var statuses = model.StatusSelectList.ToList();

            Assert.Equal(statuses.Count(), 2);
            Assert.True(statuses.Exists(r => r.Text == SchemeStatus.Withdrawn.ToString()));
            Assert.True(statuses.Exists(r => r.Text == SchemeStatus.Approved.ToString()));
        }

        [Fact]
        public async void GetEditScheme_StatusIsPending_DoesNotProvideWithdrawnStatus()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<SchemeData>>._))
                .Returns(new SchemeData
                {
                    SchemeStatus = SchemeStatus.Pending
                });

            var result = await SchemeController().EditScheme(Guid.NewGuid());
            var model = (SchemeViewModel)((ViewResult)result).Model;

            var statuses = model.StatusSelectList.ToList();

            Assert.Equal(statuses.Count(), 3);
            Assert.False(statuses.Exists(r => r.Text == SchemeStatus.Withdrawn.ToString()));
        }

        [Fact]
        public async void GetProducerCSV_ReturnsCSVFile()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerCSV>._))
               .Returns(new ProducerCSVFileData
               {
                   FileName = "test.csv",
                   FileContent = "test,test,test"
               });

            var result = await SchemeController().GetProducerCSV(Guid.NewGuid(), 2016, "WEE/FA9999KE/SCH");

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetProducerCSV>._))
               .MustHaveHappened(Repeated.Exactly.Once);

            Assert.IsType<FileContentResult>(result);
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
        public async void PostEditScheme_OldApprovalNumberAndApprovalNumberNotMatchAndApprovalNumberAlreadyExist_ReturnsViewWithError()
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

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyApprovalNumberExists>._)).Returns(true);

            var result = await controller.EditScheme(Guid.NewGuid(), scheme);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async void PostEditScheme_OldApprovalNumberAndApprovalNumberAreSame_ReturnToManageScheme()
        {
            var controller = SchemeController();

            var scheme = new SchemeViewModel
            {
                OldApprovalNumber = "WEE/AD1234DC/SCH",
                ApprovalNumber = "WEE/AD1234DC/SCH",
                SchemeName = "Any value",
                ObligationType = ObligationType.B2B,
                CompetentAuthorityId = Guid.NewGuid(),
                IbisCustomerReference = "Any value"
            };

            var result = await controller.EditScheme(Guid.NewGuid(), scheme);

            Assert.IsType<RedirectToRouteResult>(result);
            Assert.True(controller.ModelState.IsValid);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageSchemes", routeValues["action"]);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_ButSchemeIsRejected_RedirectsToRejectionConfirmation_WithSchemeId()
        {
            var controller = SchemeController();
            var schemeId = Guid.NewGuid();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var result = await controller.EditScheme(schemeId, new SchemeViewModel
            {
                Status = SchemeStatus.Rejected
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ConfirmRejection", routeValues["action"]);
            Assert.Equal(schemeId, routeValues["schemeId"]);
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
            Assert.Equal(schemeId, routeValues["schemeId"]);
        }

        [Fact]
        public async void HttpPost_ConfirmRejectionWithYesOption_SendsSetStatusRequest_WithRejectedStatus_AndRedirectsToManageSchemes()
        {
            var status = SchemeStatus.Pending;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .Invokes((string t, IRequest<Guid> s) => status = ((SetSchemeStatus)s).Status)
                .Returns(Guid.NewGuid());

            var result = await SchemeController().ConfirmRejection(Guid.Empty, new ConfirmRejectionViewModel
            {
                PossibleValues = new[] { ConfirmSchemeRejectionOptions.Yes, ConfirmSchemeRejectionOptions.No },
                SelectedValue = ConfirmSchemeRejectionOptions.Yes
            });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(SchemeStatus.Rejected, status);
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageSchemes", routeValues["action"]);
        }

        [Fact]
        public async void HttpPost_ConfirmRejectionWithNoOption_AndRedirectsToEditScheme()
        {
            var result = await SchemeController().ConfirmRejection(Guid.Empty, new ConfirmRejectionViewModel
            {
                PossibleValues = new[] { ConfirmSchemeRejectionOptions.Yes, ConfirmSchemeRejectionOptions.No },
                SelectedValue = ConfirmSchemeRejectionOptions.No
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("EditScheme", routeValues["action"]);
        }

        [Fact]
        public async void PostEditScheme_SchemeIsWithdrawn_RedirectsToWithdrawnConfirmation_WithSchemeId()
        {
            var controller = SchemeController();
            var schemeId = Guid.NewGuid();
            var result = await controller.EditScheme(schemeId, new SchemeViewModel
            {
                Status = SchemeStatus.Withdrawn,
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ConfirmWithdrawn", routeValues["action"]);
            Assert.Equal(schemeId, routeValues["schemeId"]);
        }

        [Fact]
        public async void PostEditScheme_ConfirmWithdrawnWithYesOption_SendsSetStatusRequest_WithWithdrawnStatus_AndRedirectsToManageSchemes()
        {
            var status = SchemeStatus.Approved;

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .Invokes((string t, IRequest<Guid> s) => status = ((SetSchemeStatus)s).Status)
                .Returns(Guid.NewGuid());

            var result = await SchemeController().ConfirmWithdrawn(Guid.Empty, new ConfirmWithdrawnViewModel
            {
                PossibleValues = new[] { ConfirmSchemeWithdrawOptions.Yes, ConfirmSchemeWithdrawOptions.No },
                SelectedValue = ConfirmSchemeWithdrawOptions.Yes
            });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<Guid>>._))
                .MustHaveHappened(Repeated.Exactly.Once);
            Assert.Equal(SchemeStatus.Withdrawn, status);
            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("ManageSchemes", routeValues["action"]);
        }

        [Fact]
        public async void PostEditScheme_ConfirmWithdrawnWithNoOption_AndRedirectsToEditScheme()
        {
            var result = await SchemeController().ConfirmWithdrawn(Guid.Empty, new ConfirmWithdrawnViewModel
            {
                PossibleValues = new[] { ConfirmSchemeWithdrawOptions.Yes, ConfirmSchemeWithdrawOptions.No },
                SelectedValue = ConfirmSchemeWithdrawOptions.No
            });

            Assert.IsType<RedirectToRouteResult>(result);

            var routeValues = ((RedirectToRouteResult)result).RouteValues;

            Assert.Equal("EditScheme", routeValues["action"]);
        }

        [Fact]
        public async Task GetManageContactDetails_WithValidOrganisationId_ShouldReturnsDataAndDefaultView()
        {
            var organisationData = new OrganisationData
            {
                Contact = new ContactData(),
                OrganisationAddress = new AddressData()
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(organisationData);

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            var schemeController = SchemeController();

            new HttpContextMocker().AttachToController(schemeController);

            ActionResult result = await schemeController.ManageContactDetails(Guid.NewGuid(), Guid.NewGuid());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public async Task PostManageContactDetails_WithModelErrors_GetsCountriesAndReturnsDefaultView()
        {
            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            var schemeController = SchemeController();

            new HttpContextMocker().AttachToController(schemeController);

            schemeController.ModelState.AddModelError("SomeProperty", "IsInvalid");

            var manageContactDetailsViewModel = new ManageContactDetailsViewModel
            {
                Contact = new ContactData(),
                OrganisationAddress = new AddressData(),
                SchemeId = Guid.NewGuid(),
                OrgId = Guid.NewGuid()
            };
            ActionResult result = await schemeController.ManageContactDetails(manageContactDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.Equal(countries, manageContactDetailsViewModel.OrganisationAddress.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);

            ViewResult viewResult = (ViewResult)result;

            Assert.Equal(string.Empty, viewResult.ViewName);
            Assert.Equal(manageContactDetailsViewModel, viewResult.Model);
        }

        [Fact]
        public async Task PostManageContactDetails_WithNoModelErrors_UpdatesDetailsAndRedirectsToEditScheme()
        {
            var manageContactDetailsViewModel = new ManageContactDetailsViewModel
            {
                Contact = new ContactData(),
                OrganisationAddress = new AddressData(),
                SchemeId = Guid.NewGuid(),
                OrgId = Guid.NewGuid()
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationContactDetails>._))
                .Returns(true);
            var schemeController = SchemeController();
            new HttpContextMocker().AttachToController(schemeController);

            ActionResult result = await schemeController.ManageContactDetails(manageContactDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateOrganisationContactDetails>._))
                .MustHaveHappened(Repeated.Exactly.Once);

            Assert.NotNull(result);
            Assert.IsType(typeof(RedirectToRouteResult), result);

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;
            Assert.Equal("EditScheme", redirectResult.RouteValues["Action"]);
        }

        [Fact]
        public async void GetViewOrganisationDetails_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await SchemeController().ViewOrganisationDetails(A<Guid>._, A<Guid>._);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(((ViewResult)result).ViewName, "ViewOrganisationDetails");
        }

        private SchemeController SchemeController()
        {
            return new SchemeController(() => weeeClient, A.Fake<IWeeeCache>(), A.Fake<BreadcrumbService>());
        }
    }
}
