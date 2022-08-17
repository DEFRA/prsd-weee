namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using Api.Client;
    using Core.Organisations;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Security;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.AatfReturn;
    using System.Web.Routing;
    using TestHelpers;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Controllers.Base;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Web.Areas.Admin.ViewModels.Scheme;
    using Web.Areas.Admin.ViewModels.Scheme.Overview;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.ContactDetails;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.MembersData;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.OrganisationDetails;
    using Web.Areas.Admin.ViewModels.Scheme.Overview.PcsDetails;
    using Web.Areas.Admin.ViewModels.Shared;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    using Weee.Requests.Scheme.MemberRegistration;
    using Weee.Requests.Shared;
    using Xunit;
    
    using AddressData = Core.Shared.AddressData;

    public class SchemeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IMapper mapper;
        private readonly SchemeController controller;
        private readonly Fixture fixture;

        public SchemeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            weeeCache = A.Fake<IWeeeCache>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            fixture = new Fixture();

            controller = new SchemeController(() => weeeClient, weeeCache, breadcrumbService, mapper);

            // By default all mappings will return a concrete instance, rather than faked
            A.CallTo(() => mapper.Map<PcsDetailsOverviewViewModel>(A<SchemeData>._))
                .Returns(new PcsDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<SoleTraderDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new SoleTraderDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<PartnershipDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new PartnershipDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<RegisteredCompanyDetailsOverviewViewModel>(A<OrganisationData>._))
                .Returns(new RegisteredCompanyDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<ContactDetailsOverviewViewModel>(A<SchemeData>._))
                .Returns(new ContactDetailsOverviewViewModel());
            A.CallTo(() => mapper.Map<MembersDataOverviewViewModel>(A<SchemeData>._))
                .Returns(new MembersDataOverviewViewModel());
        }

        [Fact]
        public void Controller_ShouldInheritFromAdminBaseController()
        {
            typeof(SchemeController).Should().BeDerivedFrom<AdminController>();
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
            SetUpControllerContext(false);
            controller.ModelState.AddModelError(string.Empty, "Some error");

            var result = await controller.ManageSchemes(new ManageSchemesViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ManageSchemesGet_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            SetUpControllerContext(userHasInternalAdminClaims);

            ViewResult result = await controller.ManageSchemes() as ViewResult;

            ManageSchemesViewModel viewModel = result.Model as ManageSchemesViewModel;

            Assert.Equal(userHasInternalAdminClaims, viewModel.CanAddPcs);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ManagesSchemesPost_InvalidModel_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            SetUpControllerContext(userHasInternalAdminClaims);
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            ManageSchemesViewModel viewModel = new ManageSchemesViewModel();

            ViewResult result = await controller.ManageSchemes(viewModel) as ViewResult;

            ManageSchemesViewModel resultViewModel = result.Model as ManageSchemesViewModel;

            Assert.Equal(userHasInternalAdminClaims, resultViewModel.CanAddPcs);
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

            var scheme = new SchemeData
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

            var scheme = new SchemeData
            {
                CanEdit = true
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);

            var result = await controller.EditScheme(schemeId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;

            Assert.IsType<SchemeViewModelBase>(model);
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
            var model = (SchemeViewModelBase)((ViewResult)result).Model;

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
            var model = (SchemeViewModelBase)((ViewResult)result).Model;

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
            var model = (SchemeViewModelBase)((ViewResult)result).Model;

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
               .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_AndCurrentSchemeStatusIsPending_ExecuteGetSchemeByIdReturnsViewWithApprovedRejectedAndPendingStatuses()
        {
            var status = SchemeStatus.Pending;
            var controller = SchemeController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var schemeId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .Returns(new SchemeData
               {
                   SchemeStatus = status
               });

            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
            {
                Status = status,
            });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .MustHaveHappened(1, Times.Exactly);

            var model = ((ViewResult)result).Model as SchemeViewModelBase;

            Assert.NotNull(model);

            Assert.True(model.StatusSelectList.Count() == 3);
            Assert.Contains(model.StatusSelectList, item => (item.Text == SchemeStatus.Approved.ToString()
            || item.Text == SchemeStatus.Rejected.ToString()
            || item.Text == SchemeStatus.Pending.ToString()));

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_AndCurrentSchemeStatusIsApproved_ExecuteGetSchemeByIdReturnsViewWithApprovedAndWithdrawnStatuses()
        {
            var status = SchemeStatus.Approved;
            var controller = SchemeController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var schemeId = Guid.NewGuid();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .Returns(new SchemeData
               {
                   SchemeStatus = status
               });

            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
            {
                Status = status,
            });

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._))
               .MustHaveHappened(1, Times.Exactly);

            var model = ((ViewResult)result).Model as SchemeViewModelBase;

            Assert.NotNull(model);

            Assert.True(model.StatusSelectList.Count() == 2);
            Assert.Contains(model.StatusSelectList, item => (item.Text == SchemeStatus.Approved.ToString()
            || item.Text == SchemeStatus.Withdrawn.ToString()));

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void PostEditScheme_ModelWithError_ButSchemeIsRejected_RedirectsToRejectionConfirmation_WithSchemeId()
        {
            var controller = SchemeController();
            var schemeId = Guid.NewGuid();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
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
            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
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
            var apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.Success
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            var controller = SchemeController();

            // Act
            var model = new SchemeViewModelBase
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = Guid.NewGuid()
            };
            var schemeId = Guid.NewGuid();

            var result = await controller.EditScheme(schemeId, model);

            A.CallTo(() => weeeCache.InvalidateSchemeCache(schemeId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeCache.InvalidateSchemePublicInfoCache(model.OrganisationId)).MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);

            // Assert
            var redirectResult = result as RedirectToRouteResult;
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
            var apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            var controller = SchemeController();

            // Act
            var model = new SchemeViewModelBase
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
            };

            var result = await controller.EditScheme(A.Dummy<Guid>(), model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "EditScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["ApprovalNumber"].Errors.Count);
            Assert.Equal("Approval number already exists", controller.ModelState["ApprovalNumber"].Errors[0].ErrorMessage);
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
            var apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure,
                IbisCustomerReferenceUniquenessFailure = new CreateOrUpdateSchemeInformationResult.IbisCustomerReferenceUniquenessFailureInfo()
                {
                    IbisCustomerReference = "WEE1234567",
                    OtherSchemeName = "Big Waste Co.",
                    OtherSchemeApprovalNumber = "WEE/AB1234CD/SCH"
                }
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(apiResult);

            var controller = SchemeController();

            // Act
            var model = new SchemeViewModelBase
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
            };

            var result = await controller.EditScheme(A.Dummy<Guid>(), model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "EditScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["IbisCustomerReference"].Errors.Count);
            Assert.Equal(
                "Billing reference \"WEE1234567\" already exists for scheme \"Big Waste Co.\" (WEE/AB1234CD/SCH)",
                controller.ModelState["IbisCustomerReference"].Errors[0].ErrorMessage);
        }

        [Fact]
        public async void GetAddScheme_ReturnsView_WithManageSchemeModel()
        {
            Guid organisationId = Guid.NewGuid();

            SchemeController controller = SchemeController();

            ActionResult result = await controller.AddScheme(organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.IsType<ViewResult>(result);
            var model = ((ViewResult)result).Model;

            Assert.IsType<AddSchemeViewModel>(model);
        }

        [Fact]
        public async void PostAddScheme_ModelWithError_ReturnModel()
        {
            var status = SchemeStatus.Pending;
            var controller = SchemeController();
            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");
            var organisationId = Guid.NewGuid();

            var competentAuthority = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var complianceYears = fixture.CreateMany<int>().ToList();
            var countryData = fixture.CreateMany<CountryData>().ToList();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(competentAuthority);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>.That.Matches(g => g.PcsId == organisationId))).Returns(complianceYears);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(g => g.UKRegionsOnly == false))).Returns(countryData);

            var result = await controller.AddScheme(new AddSchemeViewModel
            {
                Status = status,
                OrganisationId = organisationId,
                OrganisationAddress = new AddressData()
            });

            var model = ((ViewResult)result).Model as AddSchemeViewModel;
            model.OrganisationAddress.Countries.Should().BeSameAs(countryData);
            model.ComplianceYears.Should().BeSameAs(complianceYears);
            model.CompetentAuthorities.Should().BeSameAs(competentAuthority);

            Assert.IsType<ViewResult>(result);
        }

        [Fact] public async Task PostAddScheme_WithValidVewModel_ApuShouldBeCalledWithValidRequest()
        {
            var controller = SchemeController();

            var model = new AddSchemeViewModel
            {
                OrganisationId = fixture.Create<Guid>(),
                SchemeName = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                IbisCustomerReference = fixture.Create<string>(),
                ObligationType = fixture.Create<ObligationType>(),
                CompetentAuthorityId = fixture.Create<Guid>()
            };

            await controller.AddScheme(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>.That.Matches(c => 
                c.OrganisationId == model.OrganisationId &&
                c.ApprovalNumber == model.ApprovalNumber &&
                c.CompetentAuthorityId == model.CompetentAuthorityId &&
                c.IbisCustomerReference == model.IbisCustomerReference &&
                c.ObligationType == model.ObligationType &&
                c.SchemeName == model.SchemeName &&
                c.Status == SchemeStatus.Approved))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task PostAddScheme_WithApiReturningSuccess_RedirectsToManageSchemes()
        {
            // Arrange
            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.Success
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved
            };

            ActionResult result = await controller.AddScheme(model);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);

            // Assert
            RedirectToRouteResult redirectResult = result as RedirectToRouteResult;
            Assert.NotNull(redirectResult);

            Assert.Equal("ManageSchemes", redirectResult.RouteValues["action"]);
        }

        [Fact]
        public async Task PostAddScheme_WithApiReturningApprovalNumberUniquenessFailure_ReturnsViewWithModelError()
        {
            // Arrange

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = A.Dummy<Guid>(),
                OrganisationAddress = A.Dummy<AddressData>()
            };

            model.OrganisationAddress.Countries = countries;

            ActionResult result = await controller.AddScheme(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "AddScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["ApprovalNumber"].Errors.Count);
            Assert.Equal("Approval number already exists", controller.ModelState["ApprovalNumber"].Errors[0].ErrorMessage);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task PostAddScheme_WithApiReturnsWithSchemeAlreadyExists_ReturnsViewWithModelError()
        {
            // Arrange

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.SchemeAlreadyExists
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = A.Dummy<Guid>(),
                OrganisationAddress = A.Dummy<AddressData>()
            };

            model.OrganisationAddress.Countries = countries;

            ActionResult result = await controller.AddScheme(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "AddScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["SchemeExists"].Errors.Count);
            Assert.Equal("Scheme has already been created for the organisation", controller.ModelState["SchemeExists"].Errors[0].ErrorMessage);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task PostAddScheme_WithApiReturningIbisCustomerReferenceUniquenessFailure_ReturnsViewWithModelError()
        {
            // Arrange

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure,
                IbisCustomerReferenceUniquenessFailure = new CreateOrUpdateSchemeInformationResult.IbisCustomerReferenceUniquenessFailureInfo()
                {
                    IbisCustomerReference = "WEE1234567",
                    OtherSchemeName = "Big Waste Co.",
                    OtherSchemeApprovalNumber = "WEE/AB1234CD/SCH"
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = A.Dummy<Guid>(),
                OrganisationAddress = A.Dummy<AddressData>()
            };

            model.OrganisationAddress.Countries = countries;

            ActionResult result = await controller.AddScheme(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "AddScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["IbisCustomerReference"].Errors.Count);
            Assert.Equal(
                "Billing reference \"WEE1234567\" already exists for scheme \"Big Waste Co.\" (WEE/AB1234CD/SCH)",
                controller.ModelState["IbisCustomerReference"].Errors[0].ErrorMessage);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async Task PostAddScheme_WithApiReturningIbisCustomerReferenceMandatoryForEAFailure_ReturnsViewWithModelError()
        {
            // Arrange

            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure,
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = A.Dummy<Guid>(),
                OrganisationAddress = A.Dummy<AddressData>()
            };

            model.OrganisationAddress.Countries = countries;

            ActionResult result = await controller.AddScheme(model);

            // Assert
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.True(string.IsNullOrEmpty(viewResult.ViewName) || string.Equals(viewResult.ViewName, "AddScheme", StringComparison.InvariantCultureIgnoreCase));

            Assert.Equal(1, controller.ModelState["IbisCustomerReference"].Errors.Count);
            Assert.Equal(
                "Enter a customer billing reference", controller.ModelState["IbisCustomerReference"].Errors[0].ErrorMessage);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
              .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public async void GetAddScheme_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
    
            var result = await controller.AddScheme(organisationId);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
        }

        [Fact]
        public async void GetAddScheme_GivenOrganisation_ViewModelPropertiesShouldBeSet()
        {
            var organisationId = Guid.NewGuid();
            var competentAuthority = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var complianceYears = fixture.CreateMany<int>().ToList();
            var organisationName = fixture.Create<string>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetUKCompetentAuthorities>._)).Returns(competentAuthority);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetComplianceYears>.That.Matches(g => g.PcsId == organisationId))).Returns(complianceYears);
            A.CallTo(() => weeeCache.FetchOrganisationName(organisationId)).Returns(organisationName);
            var result = await controller.AddScheme(organisationId) as ViewResult;

            var model = result.Model as AddSchemeViewModel;
            model.CompetentAuthorities.Should().BeSameAs(competentAuthority);
            model.ComplianceYears.Should().BeSameAs(complianceYears);
            model.OrganisationId.Should().Be(organisationId);
            model.OrganisationName.Should().Be(organisationName);
            model.IsChangeableStatus = true;
            model.OrganisationAddress.Should().NotBeNull();
        }

        [Theory]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure)]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure)]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure)]
        public async void PostAddScheme__BreadcrumbShouldBeSet(CreateOrUpdateSchemeInformationResult.ResultType resultType)
        {
            // Arrange
            List<CountryData> countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            CreateOrUpdateSchemeInformationResult apiResult = new CreateOrUpdateSchemeInformationResult()
            {
                Result = resultType,
                IbisCustomerReferenceUniquenessFailure = new CreateOrUpdateSchemeInformationResult.IbisCustomerReferenceUniquenessFailureInfo()
                {
                    IbisCustomerReference = "WEE1234567",
                    OtherSchemeName = "Big Waste Co.",
                    OtherSchemeApprovalNumber = "WEE/AB1234CD/SCH"
                }
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<CreateScheme>._)).Returns(apiResult);

            SchemeController controller = SchemeController();

            // Act
            AddSchemeViewModel model = new AddSchemeViewModel
            {
                ObligationType = ObligationType.Both,
                Status = SchemeStatus.Approved,
                OrganisationId = A.Dummy<Guid>(),
                OrganisationAddress = A.Dummy<AddressData>()
            };

            model.OrganisationAddress.Countries = countries;

            ActionResult result = await controller.AddScheme(model);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
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
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);

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
            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
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
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => weeeCache.InvalidateOrganisationSearch()).MustHaveHappened(1, Times.Exactly);

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

            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .Returns(countries);

            var scheme = new SchemeData
            {
                CanEdit = false
            };
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);

            //Act
            var schemeController = SchemeController();
            new HttpContextMocker().AttachToController(schemeController);

            var result = await schemeController.ManageContactDetails(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsType<HttpForbiddenResult>(result);
        }

        [Fact]
        public async Task GetManageContactDetails_WithValidSchemeId_ShouldReturnsDataAndDefaultView()
        {
            var organisationId = Guid.NewGuid();
            var schemeId = Guid.NewGuid();

            var schemeData = new SchemeData()
            {
                Contact = new ContactData(),
                Address = new AddressData(),
                CanEdit = true
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(schemeData);

            var countries = new List<CountryData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countries);

            var schemeController = SchemeController();

            new HttpContextMocker().AttachToController(schemeController);

            var result = await schemeController.ManageContactDetails(schemeId, organisationId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>.That.Matches(c => c.SchemeId.Equals(schemeId))))
                .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).MustHaveHappened(1, Times.Exactly);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);
        }

        [Fact]
        public async Task PostManageContactDetails_WithModelErrors_GetsCountriesAndReturnsDefaultView()
        {
            var countries = new List<CountryData>();

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
            var result = await schemeController.ManageContactDetails(manageContactDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.Equal(countries, manageContactDetailsViewModel.OrganisationAddress.Countries);

            Assert.NotNull(result);
            Assert.IsType(typeof(ViewResult), result);

            var viewResult = (ViewResult)result;

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
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._))
                .Returns(true);
            var schemeController = SchemeController();
            new HttpContextMocker().AttachToController(schemeController);

            var result = await schemeController.ManageContactDetails(manageContactDetailsViewModel);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._))
                .MustHaveHappened(1, Times.Exactly);

            Assert.NotNull(result);
            Assert.IsType(typeof(RedirectToRouteResult), result);

            var redirectResult = (RedirectToRouteResult)result;
            Assert.Equal("Overview", redirectResult.RouteValues["Action"]);
        }

        [Fact]
        public async Task PostManageContactDetails_UpdatesDetailsAndDoesNotSendNotificationOnChange()
        {
            // Arrange
            var manageContactDetailsViewModel = new ManageContactDetailsViewModel
            {
                Contact = new ContactData(),
                OrganisationAddress = new AddressData(),
                SchemeId = Guid.NewGuid(),
                OrgId = Guid.NewGuid()
            };

            var schemeController = SchemeController();

            // Act
            await schemeController.ManageContactDetails(manageContactDetailsViewModel);

            // Assert
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeContactDetails>._))
                .WhenArgumentsMatch(a => ((UpdateSchemeContactDetails)a[1]).SendNotificationOnChange == false)
                .MustHaveHappened();
        }

        [Fact]
        public async void GetViewOrganisationDetails_ReturnsView()
        {
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<VerifyOrganisationExists>._))
                .Returns(true);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetOrganisationInfo>._))
                .Returns(new OrganisationData());

            var result = await SchemeController().ViewOrganisationDetails(A.Dummy<Guid>(), A.Dummy<Guid>());

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

        [Fact]
        public async void HttpGet_Overview_BreadcrumbShouldBeSet()
        {
            var schemeName = "schemeName";
            var schemeId = Guid.NewGuid();

            A.CallTo(() => weeeCache.FetchSchemeName(schemeId)).Returns(schemeName);

            var result = await SchemeController().Overview(schemeId);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
            breadcrumbService.InternalScheme.Should().Be(schemeName);
        }

        [Fact]
        public async void HttpGet_EditScheme_CanEditIsTrueBreadcrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            var schemeName = "schemeName";

            var controller = SchemeController();

            var scheme = new SchemeData
            {
                CanEdit = true
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);
            A.CallTo(() => weeeCache.FetchSchemeName(schemeId)).Returns(schemeName);

            var result = await controller.EditScheme(schemeId);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
            breadcrumbService.InternalScheme.Should().Be(schemeName);
        }

        [Fact]
        public async void HttpPost_EditScheme_ModelWithError_BreadcrumbIsSet()
        {
            var schemeId = Guid.NewGuid();
            var schemeName = "schemeName";

            var controller = SchemeController();

            controller.ModelState.AddModelError("ErrorKey", "Some kind of error goes here");

            var scheme = new SchemeData
            {
                CanEdit = true
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);
            A.CallTo(() => weeeCache.FetchSchemeName(schemeId)).Returns(schemeName);

            var result = await controller.EditScheme(schemeId, new SchemeViewModelBase
            {
                Status = SchemeStatus.Pending
            });

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
            breadcrumbService.InternalScheme.Should().Be(schemeName);
        }

        [Fact]
        public async void HttpPost_EditScheme_CanEditIsTrueBreadcrumbShouldBeSet()
        {
            var schemeId = Guid.NewGuid();
            var schemeName = "schemeName";

            var controller = SchemeController();

            var scheme = new SchemeData
            {
                CanEdit = true
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);
            A.CallTo(() => weeeCache.FetchSchemeName(schemeId)).Returns(schemeName);

            var result = await controller.EditScheme(schemeId);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
            breadcrumbService.InternalScheme.Should().Be(schemeName);
        }

        [Theory]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure)]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceMandatoryForEAFailure)]
        [InlineData(CreateOrUpdateSchemeInformationResult.ResultType.IbisCustomerReferenceUniquenessFailure)]
        public async void HttpPost_EditScheme_UpdateSchemeInformationResultFailures_BreadcrumbShouldBeSet(CreateOrUpdateSchemeInformationResult.ResultType resultType)
        {
            var schemeId = Guid.NewGuid();
            var schemeName = "schemeName";
            var infoResult = new CreateOrUpdateSchemeInformationResult();
            infoResult.Result = CreateOrUpdateSchemeInformationResult.ResultType.ApprovalNumberUniquenessFailure;

            var controller = SchemeController();

            var scheme = new SchemeData
            {
                CanEdit = true
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<UpdateSchemeInformation>._)).Returns(infoResult);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(scheme);
            A.CallTo(() => weeeCache.FetchSchemeName(schemeId)).Returns(schemeName);

            var result = await controller.EditScheme(schemeId);

            breadcrumbService.InternalActivity.Should().Be("Manage PCSs");
            breadcrumbService.InternalScheme.Should().Be(schemeName);
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

        [Fact]
        public void CheckAddOrEditSchemeMethodsHaveInternalAdminAttribute()
        {
            IEnumerable<MethodInfo> methods = typeof(SchemeController).GetMethods().Where(p => p.Name == "AddScheme" || p.Name == "EditScheme");

            foreach (MethodInfo mi in methods)
            {
                Attribute hasInternalAdminAttribue = mi.GetCustomAttribute(typeof(AuthorizeInternalClaimsAttribute), false);

                Assert.NotNull(hasInternalAdminAttribue);
            }
        }

        [Theory]
        [InlineData(OrganisationType.RegisteredCompany, typeof(RegisteredCompanyDetailsOverviewViewModel), "Overview/RegisteredCompanyDetailsOverview")]
        [InlineData(OrganisationType.Partnership, typeof(PartnershipDetailsOverviewViewModel), "Overview/PartnershipDetailsOverview")]
        [InlineData(OrganisationType.SoleTraderOrIndividual, typeof(SoleTraderDetailsOverviewViewModel), "Overview/SoleTraderDetailsOverview")]
        public async void HttpGet_Overview_WithOrganisationDetailsDisplayOption_AssociatedEntitiesShouldBeMapped(OrganisationType organisationType, Type expectedViewModelType, string expectedViewName)
        {
            var organisationId = fixture.Create<Guid>();
            var schemeId = fixture.Create<Guid>();
            var associatedAatfs = fixture.CreateMany<AatfDataList>().ToList();
            var associatedSchemes = fixture.CreateMany<SchemeData>().ToList();
            var associatedViewModel = fixture.Create<AssociatedEntitiesViewModel>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemeById>._)).Returns(new SchemeData() {Id = schemeId, OrganisationId = organisationId});
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<IRequest<OrganisationData>>._))
                .Returns(new OrganisationData
                {
                    OrganisationType = organisationType
                });
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfsByOrganisationId>.That.Matches(g => g.OrganisationId == organisationId)))
                .Returns(associatedAatfs);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesByOrganisationId>.That.Matches(g => g.OrganisationId == organisationId)))
                .Returns(associatedSchemes);
            A.CallTo(() => mapper.Map<AssociatedEntitiesViewModel>(A<AssociatedEntitiesViewModelTransfer>.That.Matches(a =>
                a.AssociatedAatfs == associatedAatfs &&
                a.AssociatedSchemes == associatedSchemes &&
                a.SchemeId == schemeId))).Returns(associatedViewModel);

            var result = await SchemeController().Overview(schemeId, OverviewDisplayOption.OrganisationDetails) as ViewResult;

            var model = result.Model as OrganisationDetailsOverviewViewModel;
            model.AssociatedEntities.Should().Be(associatedViewModel);
        }

        private SchemeController SchemeController()
        {
            return new SchemeController(() => weeeClient, weeeCache, breadcrumbService, mapper);
        }

        private void SetUpControllerContext(bool hasInternalAdminUserClaims)
        {
            var httpContextBase = A.Fake<HttpContextBase>();
            var principal = new ClaimsPrincipal(httpContextBase.User);
            var claimsIdentity = new ClaimsIdentity(httpContextBase.User.Identity);

            if (hasInternalAdminUserClaims)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.InternalAdmin));
            }
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => httpContextBase.User.Identity).Returns(claimsIdentity);

            var context = new ControllerContext(httpContextBase, new RouteData(), controller);
            controller.ControllerContext = context;
        }
    }
}
