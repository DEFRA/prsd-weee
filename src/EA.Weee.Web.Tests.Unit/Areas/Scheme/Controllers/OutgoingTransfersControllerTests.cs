namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Controllers
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Web.Areas.Scheme.Controllers;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Web.Areas.Scheme.Requests;
    using Weee.Tests.Core;
    using Xunit;

    public class OutgoingTransfersControllerTests : SimpleUnitTestBase
    {
        private readonly IWeeeClient weeeClient;
        private readonly IMapper mapper;
        private readonly OutgoingTransfersController outgoingTransferEvidenceController;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache cache;
        private readonly Guid organisationId;

        public OutgoingTransfersControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            breadcrumb = A.Fake<BreadcrumbService>();
            cache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            organisationId = Guid.NewGuid();

            outgoingTransferEvidenceController = new OutgoingTransfersController(mapper, breadcrumb, cache, () => weeeClient);
        }

        [Fact]
        public void OutgoingTransfersControllerInheritsCheckSchemeEvidenceBaseController()
        {
            typeof(OutgoingTransfersController).BaseType.Name.Should().Be(nameof(SchemeEvidenceBaseController));
        }

        [Fact]
        public void EditTonnagesGet_ShouldHaveHttpGetAttribute()
        {
            typeof(OutgoingTransfersController).GetMethod("EditTonnages", new[] { typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact(Skip = "TODO")]
        public async Task EditTonnagesGet_GivenValidOrganisation_BreadcrumbShouldBeSet()
        {
            // arrange 
            var organisationName = "OrganisationName";

            A.CallTo(() => cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            // act
            await outgoingTransferEvidenceController.EditTonnages(organisationId, TestFixture.Create<Guid>());

            // assert
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.SchemeManageEvidence);
        }
    }
}
