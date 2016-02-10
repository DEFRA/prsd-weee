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
    using Infrastructure;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.ViewModels.Scheme;
    using Web.Areas.Admin.ViewModels.Scheme.Overview;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.ContactDetails;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.MembersData;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.OrganisationDetails;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.PcsDetails;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
    using Xunit;

    public class SchemeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IMapper mapper;

        public SchemeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            weeeCache = A.Fake<IWeeeCache>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();

            // By default all mappings will return a concrete instance, rather than faked
            A.CallTo(() => mapper.Map<PcsDetailsOverviewViewModel>(A<SchemeData>._))
                .Returns(new PcsDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<SoleTraderDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new SoleTraderDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<PartnershipDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new PartnershipDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<RegisteredCompanyDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new RegisteredCompanyDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<ContactDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new ContactDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<MembersDataOverviewViewModel>(A<SchemeData>._))
                .Returns(new MembersDataOverviewViewModel());
        }

        [Fact]
        public async Task ManageSchemesPost_AllGood_ReturnsOverviewRedirect()
        {
            var selectedGuid = Guid.NewGuid();
            var controller = SchemeController();

            var result = await controller.ManageSchemes(new ManageSchemesViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("Overview", redirectValues["action"]);
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

        /// <summary>
        /// This test ensures that the Get for Edit Scheme action returns the HTTP Forbidden code
        /// when the current user is not allowed to edit pcs details.
        /// </summary>
        [Fact]
        public async void GetEditScheme_ReturnsHttpForbiddenResult_WhenCanEditPcsIsFalse()
        {
            // Arrange
            var schemeId = Guid.NewGuid();

            var controller = SchemeController();

            SchemeData scheme = new SchemeData
            {
                CanEdit = false
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);

            //Act
            var result = await controller.EditScheme(schemeId);
            
            // Assert
            Assert.IsType<HttpForbiddenResult>(result);
        }

        [Fact]
        public async void GetEditScheme_ReturnsView_WithManageSchemeModel()
        {
            var schemeId = Guid.NewGuid();

            var controller = SchemeController();

            SchemeData scheme = new SchemeData
            {
                CanEdit = true
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);              

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
        public async void GetEditScheme_StatusIsRejectedOrWithdrawn_IsChangeableStatusIsFalse(SchemeStatus status)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<SchemeData>>._))
                .Returns(new SchemeData
                {
                    CanEdit = true,
                    SchemeStatus = status
                });

            var result = await SchemeController().EditScheme(Guid.NewGuid());
            var model = (SchemeViewModel)((ViewResult)result).Model;

            Assert.Equal(false, model.IsChangeableStatus);
        }

        [Fact]
        public async void GetEditScheme_StatusIsApproved_AvailableStatusToChangeIsWithdrawn()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<SchemeData>>._))
                .Returns(new SchemeData
                {
                    CanEdit = true,
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
                    CanEdit = true,
                    SchemeStatus = SchemeStatus.Pending
                });

            var result = await SchemeController().EditScheme(Guid.NewGuid());
            var model = (SchemeViewModel)((ViewResult)result).Model;

            var statuses = model.StatusSelectList.ToList();

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

            var result = await SchemeController().GetProducerCsv(Guid.NewGuid(), 2016, "WEE/FA9999KE/SCH");

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

        /// <summary>
        /// This test ensures that the POST "EditScheme" action will redirect the user to the
        /// "Overview" action following a successful update.
        /// </summary>
        [Fact]
        public async Task PostEditScheme_WithApiReturningSuccess_RedirectsToSchemeOverview()
        {
            // Arrange
            UpdateSchemeInformationResult apiResult = new UpdateSchemeInformationResult()
            {
                Result = UpdateSchemeInformationResult.ResultType.Success
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            SchemeViewModel model = new SchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
            };

            ActionResult result = await controller.EditScheme(A.Dummy<Guid>(), model);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("Overview", redirectResult.RouteValues["action"]);
        }

        /// <summary>
        /// This test ensures that the POST "EditScheme" action will return the "EditScheme"
        /// view with a model error for the ApprovaNumber property of "Approval number already exists."
        /// if the API reports that the update failed due to a violation of the approval number
        /// uniqueness constraint.
        /// </summary>
        [Fact]
        public async Task PostEditScheme_WithApiReturningApprovalNumberUniquenessFailure_ReturnsViewWithModelError()
        {
            // Arrange
            UpdateSchemeInformationResult apiResult = new UpdateSchemeInformationResult()
            {
                Result = UpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            SchemeViewModel model = new SchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
            };

            ActionResult result = await controller.EditScheme(A.Dummy<Guid>(), model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "EditScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["ApprovalNumber"].Errors.Count);
            Assert.Equal("Approval number already exists.", controller.ModelState["ApprovalNumber"].Errors[0].ErrorMessage);
        }

        /// <summary>
        /// This test ensures that the POST "EditScheme" action will return the "EditScheme"
        /// view with a model error for the IbisCustomerReference property of:
        ///     Billing reference [1B1S customer reference] already exists for scheme "[Scheme name]" ([Approval number]).
        /// if the API reports that the update failed due to a violation of the 1B1S customer
        /// reference uniqueness constraint.
        /// </summary>
        [Fact]
        public async Task PostEditScheme_WithApiReturningIbisCustomerReferenceUniquenessFailure_ReturnsViewWithModelError()
        {
            // Arrange
            UpdateSchemeInformationResult apiResult = new UpdateSchemeInformationResult()
            {
                Result = UpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure,
                IbisCustomerReferenceUniquenessFailure = new UpdateSchemeInformationResult.IbisCustomerReferenceUniquenessFailureInfo()
                {
                    IbisCustomerReference = "WEE1234567",
                    OtherSchemeName = "Big Waste Co.",
                    OtherSchemeApprovalNumber = "WEE/AB1234CD/SCH"
                }
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            SchemeViewModel model = new SchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
            };

            ActionResult result = await controller.EditScheme(A.Dummy<Guid>(), model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "EditScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["IbisCustomerReference"].Errors.Count);
            Assert.Equal(
                "Billing reference \"WEE1234567\" already exists for scheme \"Big Waste Co.\" (WEE/AB1234CD/SCH).",
                controller.ModelState["IbisCustomerReference"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async void HttpPost_ConfirmRejectionWithYesOption_SendsSetStatusRequest_WithRejectedStatus_AndRedirectsToOverview()
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

            Assert.Equal("Overview", routeValues["action"]);
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

        /// <summary>
        /// This test ensures that the Get for Manage contact details action returns the HTTP Forbidden code
        /// when the current user is not allowed to edit pcs contact details.
        /// </summary>
        [Fact]
        public async void GetManageContactDetails_ReturnsHttpForbiddenResult_WhenCanEditIsFalse()
        {
            // Arrange
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

            SchemeData scheme = new SchemeData
            {
                CanEdit = false
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);

            //Act
            var schemeController = SchemeController();
            new HttpContextMocker().AttachToController(schemeController);

            ActionResult result = await schemeController.ManageContactDetails(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsType<HttpForbiddenResult>(result);
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

            SchemeData scheme = new SchemeData
            {
                CanEdit = true
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);

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
        public async Task PostManageContactDetails_WithNoModelErrors_UpdatesDetailsAndRedirectsToSchemeOverview()
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
            Assert.Equal("Overview", redirectResult.RouteValues["Action"]);
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

        [Fact]
        public async void HttpGet_Overview_WithNullOverviewDisplayOption_ShouldDefaultToPcsDetailsViewModel()
        {
            var result = await SchemeController().Overview(Guid.NewGuid());

            Assert.IsType<ViewResult>(result);

            var viewResult = ((ViewResult)result);

            Assert.IsType<PcsDetailsOverviewViewModel>(viewResult.Model);
            Assert.Equal("Overview/PcsDetailsOverview", viewResult.ViewName);
        }

        [Theory]
        [InlineData(OverviewDisplayOption.PcsDetails, typeof(PcsDetailsOverviewViewModel), "Overview/PcsDetailsOverview")]
        [InlineData(OverviewDisplayOption.ContactDetails, typeof(ContactDetailsOverviewViewModel), "Overview/ContactDetailsOverview")]
        [InlineData(OverviewDisplayOption.MembersData, typeof(MembersDataOverviewViewModel), "Overview/MembersDataOverview")]
        [InlineData(OverviewDisplayOption.OrganisationDetails, typeof(RegisteredCompanyDetailsOverviewViewModel), "Overview/RegisteredCompanyDetailsOverview") /* This is the expected default organisation type */]
        public async void HttpGet_Overview_WithSetDisplayOption_ShouldDirectToCorrectViewAndModel(OverviewDisplayOption displayOption, Type expectedViewModelType, string expectedViewName)
        {
            var result = await SchemeController().Overview(Guid.NewGuid(), displayOption);

            Assert.IsType<ViewResult>(result);

            var viewResult = ((ViewResult)result);

            Assert.IsType(expectedViewModelType, viewResult.Model);
            Assert.Equal(expectedViewName, viewResult.ViewName);
        }

        [Theory]
        [InlineData(OrganisationType.RegisteredCompany, typeof(RegisteredCompanyDetailsOverviewViewModel), "Overview/RegisteredCompanyDetailsOverview")]
        [InlineData(OrganisationType.Partnership, typeof(PartnershipDetailsOverviewViewModel), "Overview/PartnershipDetailsOverview")]
        [InlineData(OrganisationType.SoleTraderOrIndividual, typeof(SoleTraderDetailsOverviewViewModel), "Overview/SoleTraderDetailsOverview")]
        public async void HttpGet_Overview_WithOrganisationDetailsDisplayOption_ShouldDirectToCorrectOrganisationViewAndModel(OrganisationType organisationType, Type expectedViewModelType, string expectedViewName)
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<OrganisationData>>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = organisationType
                });

            var result = await SchemeController().Overview(Guid.NewGuid(), OverviewDisplayOption.OrganisationDetails);

            Assert.IsType<ViewResult>(result);

            var viewResult = ((ViewResult)result);

            Assert.IsType(expectedViewModelType, viewResult.Model);
            Assert.Equal(expectedViewName, viewResult.ViewName);
        }

        private SchemeController SchemeController()
        {
            return new SchemeController(() => weeeClient, weeeCache, breadcrumbService, mapper);
        }
    }
}
