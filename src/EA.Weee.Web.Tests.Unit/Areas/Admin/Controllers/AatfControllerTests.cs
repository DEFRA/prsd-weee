namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Requests.Users;
    using EA.Weee.Security;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Tests.Unit.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.Admin.Controllers;
    using Web.Areas.Admin.Requests;
    using Web.Areas.Admin.ViewModels.Aatf;
    using Xunit;

    public class AatfControllerTests
    {
        private readonly Fixture fixture;
        private readonly IWeeeClient weeeClient;
        private readonly BreadcrumbService breadcrumbService;
        private readonly IMapper mapper;
        private readonly IEditAatfDetailsRequestCreator detailsRequestCreator;
        private readonly IEditAatfContactRequestCreator contactRequestCreator;
        private readonly AatfController controller;

        public AatfControllerTests()
        {
            fixture = new Fixture();
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            mapper = A.Fake<IMapper>();
            detailsRequestCreator = A.Fake<IEditAatfDetailsRequestCreator>();
            contactRequestCreator = A.Fake<IEditAatfContactRequestCreator>();

            controller = new AatfController(() => weeeClient, breadcrumbService, mapper, detailsRequestCreator, contactRequestCreator);
        }

        [Fact]
        public async Task ManageSchemesPost_ModelError_ReturnsView()
        {
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            var result = await controller.ManageAatfs(new ManageAatfsViewModel());

            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ManageAatfsPost_ReturnsSelectedGuid()
        {
            var selectedGuid = Guid.NewGuid();

            var result = await controller.ManageAatfs(new ManageAatfsViewModel { Selected = selectedGuid });

            Assert.NotNull(result);
            Assert.IsType<RedirectToRouteResult>(result);

            var redirectValues = ((RedirectToRouteResult)result).RouteValues;
            Assert.Equal("Details", redirectValues["action"]);
            Assert.Equal(selectedGuid, redirectValues["Id"]);
        }

        [Fact]
        public async Task ManageAatfPost_ModelError_GetAatfsMustBeRun()
        {
            controller.ModelState.AddModelError(string.Empty, "Validation message");

            await controller.ManageAatfs(new ManageAatfsViewModel());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfs>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task GetAatfsList_Always_SetsInternalBreadcrumbToManageAATFs()
        {
            SetUpControllerContext(true);

            ActionResult result = await controller.ManageAatfs();

            Assert.Equal("Manage AATFs", breadcrumbService.InternalActivity);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetManageAatfs_ChecksUserIsAllowed_ViewModelSetCorrectly(bool userHasInternalAdminClaims)
        {
            SetUpControllerContext(userHasInternalAdminClaims);

            ViewResult result = await controller.ManageAatfs() as ViewResult;

            ManageAatfsViewModel viewModel = result.Model as ManageAatfsViewModel;

            Assert.Equal(userHasInternalAdminClaims, viewModel.CanAddAatf);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_BreadcrumbShouldBeSet()
        {
            var aatfData = A.Fake<AatfData>();
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            await controller.Details(A.Dummy<Guid>());

            Assert.Equal(breadcrumbService.InternalActivity, InternalUserActivity.ManageAatfs);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_ViewModelShouldBeCreatedWithApprovalDate()
        {
            AatfDetailsViewModel viewModel = A.Fake<AatfDetailsViewModel>();

            var aatfData = A.Fake<AatfData>();
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            var result = await controller.Details(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfIdButNoApprovalDate_ViewModelShouldBeCreatedWithNullApprovalDate()
        {
            AatfDetailsViewModel viewModel = A.Fake<AatfDetailsViewModel>();
            viewModel.ApprovalDate = null;

            var aatfData = A.Fake<AatfData>();
            aatfData.ApprovalDate = default(DateTime);
            A.CallTo(() => weeeClient.SendAsync(A.Dummy<string>(), A.Dummy<GetAatfById>())).Returns(aatfData);

            var result = await controller.Details(A.Dummy<Guid>()) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanNotEdit_ReturnsForbiddenResult()
        {
            var id = fixture.Create<Guid>();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, false).Create();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCall.Returns(aatf);

            var result = await controller.ManageAatfDetails(id);

            Assert.IsType<HttpForbiddenResult>(result);
            clientCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanEdit_BreadcrumbShouldBeSet()
        {
            var id = fixture.Create<Guid>();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            var aatfViewModel = fixture.Create<AatfEditDetailsViewModel>();

            var clientCall = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCall.Returns(aatf);
            var mapperCall = A.CallTo(() => mapper.Map<AatfEditDetailsViewModel>(aatf));
            mapperCall.Returns(aatfViewModel);

            await controller.ManageAatfDetails(id);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            clientCall.MustHaveHappenedOnceExactly();
            mapperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsGet_CanEdit_ViewModelShouldBeReturned()
        {
            var id = fixture.Create<Guid>();
            var countries = fixture.CreateMany<CountryData>().ToList();
            var authorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var aatf = fixture.Build<AatfData>().With(a => a.CanEdit, true).Create();
            var aatfViewModel = fixture.Create<AatfEditDetailsViewModel>();

            var clientCallAatf = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetAatfById>.That.Matches(a => a.AatfId == id)));
            clientCallAatf.Returns(aatf);
            var mapperCall = A.CallTo(() => mapper.Map<AatfEditDetailsViewModel>(aatf));
            mapperCall.Returns(aatfViewModel);
            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(authorities);
            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(countries);

            var result = await controller.ManageAatfDetails(id) as ViewResult;

            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(aatfViewModel);
            clientCallAatf.MustHaveHappenedOnceExactly();
            mapperCall.MustHaveHappenedOnceExactly();
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsPost_ValidViewModel_ApiSendAndRedirectToDetails()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            var viewModel = fixture.Build<AatfEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).Create();
            var request = fixture.Create<EditAatfDetails>();

            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var requestCall = A.CallTo(() => detailsRequestCreator.ViewModelToRequest(viewModel));
            requestCall.Returns(request);
            var helperCall = A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.Id)));
            helperCall.Returns(url);

            var result = await controller.ManageAatfDetails(viewModel) as RedirectResult;

            result.Url.Should().Be(url);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            requestCall.MustHaveHappenedOnceExactly();
            A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, request)).MustHaveHappenedOnceExactly();
            helperCall.MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void ManageAatfDetailsPost_InvalidViewModel_ApiShouldBeCalled()
        {
            IList<UKCompetentAuthorityData> competentAuthorities = fixture.CreateMany<UKCompetentAuthorityData>().ToList();
            IList<CountryData> countries = fixture.CreateMany<CountryData>().ToList();
            var siteAddress = fixture.Build<AatfAddressData>().With(sa => sa.Countries, countries).Create();
            var viewModel = fixture.Build<AatfEditDetailsViewModel>().With(a => a.CompetentAuthoritiesList, competentAuthorities).With(a => a.SiteAddress, siteAddress).Create();

            var clientCallAuthorities = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetUKCompetentAuthorities>.Ignored));
            clientCallAuthorities.Returns(Task.FromResult(competentAuthorities));
            var clientCallCountries = A.CallTo(() => weeeClient.SendAsync(A<string>.Ignored, A<GetCountries>.That.Matches(a => a.UKRegionsOnly == false)));
            clientCallCountries.Returns(Task.FromResult(countries));

            controller.ModelState.AddModelError("error", "error");

            var result = await controller.ManageAatfDetails(viewModel) as ViewResult;

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
            clientCallAuthorities.MustHaveHappenedOnceExactly();
            clientCallCountries.MustHaveHappenedOnceExactly();

            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            ContactDataAccessSetup(true);

            await controller.ManageContactDetails(aatfId);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenAction_DefaultViewShouldBeReturned()
        {
            ContactDataAccessSetup(true);
            var result = await controller.ManageContactDetails(A.Dummy<Guid>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenAatf_ContactShouldBeRetrieved()
        {
            var aatfId = Guid.NewGuid();

            var result = await controller.ManageContactDetails(aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfContact>.That.Matches(c => c.AatfId.Equals(aatfId)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenActionExecutes_CountriesShouldBeRetrieved()
        {
            ContactDataAccessSetup(true);
            var result = await controller.ManageContactDetails(A.Dummy<Guid>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>.That.Matches(c => c.UKRegionsOnly.Equals(false)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsGet_GivenUnauthorizedAccess_HttpForbiddenReturned()
        {
            var result = await controller.ManageContactDetails(A.Dummy<Guid>());

            Assert.IsType<HttpForbiddenResult>(result);
        }

        [Fact]
        public async void ManageContactDetailsPost_OnSubmit_PageRedirectsToSiteList()
        {
            var httpContext = new HttpContextMocker();
            httpContext.AttachToController(controller);

            var aatfId = Guid.NewGuid();

            var viewModel = new AatfEditContactAddressViewModel
            {
                AatfId = aatfId,
            };

            httpContext.RouteData.Values.Add("id", aatfId);

            var helper = A.Fake<UrlHelper>();
            controller.Url = helper;
            var url = fixture.Create<string>();

            A.CallTo(() => helper.Action("Details", A<object>.That.Matches(o => o.GetPropertyValue<string>("area") == "Admin" && o.GetPropertyValue<Guid>("Id") == viewModel.AatfId))).Returns(url);

            var result = await controller.ManageContactDetails(viewModel) as RedirectResult;

            result.Url.Should().Be($"{url}#contactDetails");
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new AatfEditContactAddressViewModel();
            var request = new EditAatfContact();

            A.CallTo(() => contactRequestCreator.ViewModelToRequest(model)).Returns(request);

            controller.Url = new UrlHelper(A.Fake<RequestContext>(), A.Fake<RouteCollection>());

            await controller.ManageContactDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_ApiShouldBeCalled()
        {
            var model = new AatfEditContactAddressViewModel() { ContactData = new AatfContactData() };
            controller.ModelState.AddModelError("error", "error");

            await controller.ManageContactDetails(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_CountriesShouldBeAttached()
        {
            var model = new AatfEditContactAddressViewModel() { ContactData = new AatfContactData() };
            controller.ModelState.AddModelError("error", "error");

            var countryGuid = Guid.NewGuid();
            var countryName = "MyCountryName";
            var countryList = new List<CountryData>() { new CountryData() { Id = countryGuid, Name = countryName } };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetCountries>._)).Returns(countryList);

            var result = await controller.ManageContactDetails(model) as ViewResult;
            var viewModel = result.Model as AatfEditContactAddressViewModel;
            viewModel.ContactData.AddressData.Countries.Should().NotBeNull();
            viewModel.ContactData.AddressData.Countries.Count().Should().Be(1);
            viewModel.ContactData.AddressData.Countries.ElementAt(0).Id.Should().Be(countryGuid);
            viewModel.ContactData.AddressData.Countries.ElementAt(0).Name.Should().Be(countryName);
        }

        [Fact]
        public async void ManageContactDetailsPost_GivenInvalidViewModel_BreadcrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var model = new AatfEditContactAddressViewModel() { AatfId = aatfId, ContactData = new AatfContactData() };
            controller.ModelState.AddModelError("error", "error");

            await controller.ManageContactDetails(model);

            breadcrumbService.InternalActivity.Should().Be(InternalUserActivity.ManageAatfs);
        }

        private void ContactDataAccessSetup(bool canEdit)
        {
            var contact = new AatfContactData()
            {
                CanEditContactDetails = canEdit
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfContact>._))
                .Returns(contact);
        }

        private void SetUpControllerContext(bool hasInternalAdminUserClaims)
        {
            HttpContextBase httpContextBase = A.Fake<HttpContextBase>();
            ClaimsPrincipal principal = new ClaimsPrincipal(httpContextBase.User);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(httpContextBase.User.Identity);
            
            if (hasInternalAdminUserClaims)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, Claims.InternalAdmin));
            }
            principal.AddIdentity(claimsIdentity);

            A.CallTo(() => httpContextBase.User.Identity).Returns(claimsIdentity);

            ControllerContext context = new ControllerContext(httpContextBase, new RouteData(), controller);
            controller.ControllerContext = context;
        }
    }
}
