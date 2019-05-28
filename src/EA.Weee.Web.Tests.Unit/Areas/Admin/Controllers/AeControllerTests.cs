namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Internal;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Web.Areas.Admin.Controllers.Base;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Ae;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.Controllers;
    using Xunit;

    public class AeControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly BreadcrumbService breadcrumbService;
        private readonly AeController controller;
        private readonly UrlHelper urlHelper;

        public AeControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumbService = A.Fake<BreadcrumbService>();
            urlHelper = A.Fake<UrlHelper>();
            this.mapper = A.Fake<IMapper>();

            controller = new AeController(() => weeeClient, breadcrumbService, mapper);
        }

        [Fact]
        public void ManageAesControllerInheritsAdminController()
        {
            typeof(AeController).BaseType.Name.Should().Be(typeof(AdminController).Name);
        }

        [Fact]
        public async Task ManageAes_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var result = await controller.ManageAes();

            breadcrumbService.InternalActivity.Should().Be("Manage AEs");
        }

        [Fact]
        public async void ManageAesGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.ManageAes() as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void ManageAesGet_AesShouldBeRetrieved()
        {
            var result = await controller.ManageAes();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfs>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_BreadcrumbShouldBeSet()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData()
            {
                BusinessAddress = new Core.Shared.AddressData()
                {
                    Address1 = "Site address 1",
                    Address2 = "Site address 2",
                    TownOrCity = "Site town",
                    CountyOrRegion = "Site county",
                    Postcode = "GU22 7UY",
                    CountryId = Guid.NewGuid(),
                    CountryName = "Site country",
                    Telephone = "9367282",
                    Email = "test@test.com"
                }
            };

            var @operator = new OperatorData(Guid.NewGuid(), "TEST", organisationData, organisationData.Id);

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = @operator
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            Assert.Equal(breadcrumbService.InternalActivity, InternalUserActivity.ManageAes);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_AatfsByOperatorIdShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData();
            organisationData.BusinessAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country",
                Telephone = "9367282",
                Email = "test@test.com"
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = new OperatorData(Guid.NewGuid(), "Operator", organisationData, organisationData.Id)
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfsByOperatorId>.That.Matches(a => a.OperatorId == aatfData.Operator.Id))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_MapperShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData();
            organisationData.BusinessAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country",
                Telephone = "9367282",
                Email = "test@test.com"
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = new OperatorData(Guid.NewGuid(), "Operator", organisationData, organisationData.Id)
            };

            var associatedAatfs = new List<AatfDataList>();
            var associatedSchemes = new List<Core.Scheme.SchemeData>();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfsByOperatorId>.That.Matches(a => a.OperatorId == aatfData.Operator.Id))).Returns(associatedAatfs);
            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesByOrganisationId>._)).Returns(associatedSchemes);

            await controller.Details(aatfId);

            A.CallTo(() => mapper.Map<AeDetailsViewModel>(A<AatfDataToAeDetailsViewModelMapTransfer>.That.Matches(a => a.AssociatedAatfs == associatedAatfs
            && a.AssociatedSchemes == associatedSchemes
            && a.OrganisationString == controller.GenerateAddress(aatfData.Operator.Organisation.BusinessAddress)))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_SchemesByOrganisationIdShouldBeCalled()
        {
            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData();
            organisationData.BusinessAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country",
                Telephone = "9367282",
                Email = "test@test.com"
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = new OperatorData(Guid.NewGuid(), "Operator", organisationData, organisationData.Id)
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            await controller.Details(aatfId);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetSchemesByOrganisationId>.That.Matches(a => a.OrganisationId == aatfData.Operator.OrganisationId))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfId_ViewModelShouldBeCreatedWithApprovalDate()
        {
            AeDetailsViewModel viewModel = A.Fake<AeDetailsViewModel>();

            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData();
            organisationData.BusinessAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country",
                Telephone = "9367282",
                Email = "test@test.com"
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = new OperatorData(Guid.NewGuid(), "Operator", organisationData, organisationData.Id)
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            var result = await controller.Details(aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }

        [Fact]
        public async void DetailsGet_GivenValidAatfIdButNoApprovalDate_ViewModelShouldBeCreatedWithNullApprovalDate()
        {
            AeDetailsViewModel viewModel = A.Fake<AeDetailsViewModel>();
            viewModel.ApprovalDate = null;

            var aatfId = Guid.NewGuid();
            var organisationData = new OrganisationData();
            organisationData.BusinessAddress = new Core.Shared.AddressData()
            {
                Address1 = "Site address 1",
                Address2 = "Site address 2",
                TownOrCity = "Site town",
                CountyOrRegion = "Site county",
                Postcode = "GU22 7UY",
                CountryId = Guid.NewGuid(),
                CountryName = "Site country",
                Telephone = "9367282",
                Email = "test@test.com"
            };

            var aatfData = new AatfData(Guid.NewGuid(), "name", "approval number", A.Dummy<Core.Shared.UKCompetentAuthorityData>(), Core.AatfReturn.AatfStatus.Approved, A.Dummy<AatfAddressData>(), Core.AatfReturn.AatfSize.Large, DateTime.Now)
            {
                Organisation = organisationData,
                Operator = new OperatorData(Guid.NewGuid(), "Operator", organisationData, organisationData.Id),
                ApprovalDate = default(DateTime)
            };

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetAatfById>.That.Matches(a => a.AatfId == aatfId))).Returns(aatfData);

            var result = await controller.Details(aatfId) as ViewResult;

            result.Model.Should().BeEquivalentTo(viewModel);
        }
    }
}
