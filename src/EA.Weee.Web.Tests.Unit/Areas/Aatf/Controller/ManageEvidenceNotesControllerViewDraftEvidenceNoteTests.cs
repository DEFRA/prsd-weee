namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Controller
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Constant;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Controllers;
    using Xunit;

    public class ManageEvidenceNotesControllerViewDraftEvidenceNoteTests : ManageEvidenceNotesControllerTestsBase
    {
        [Fact]
        public void ViewDraftEvidenceNoteGet_ShouldHaveHttpGetAttribute()
        {
            typeof(ManageEvidenceNotesController).GetMethod("ViewDraftEvidenceNote", new[] { typeof(Guid), typeof(Guid), typeof(Guid) }).Should()
                .BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_DefaultViewShouldBeReturned()
        {
            //act
            var result = await Controller.ViewDraftEvidenceNote(OrganisationId, AatfId, EvidenceNoteId) as ViewResult;

            //assert
            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async Task ViewDraftEvidenceNoteGet_BreadcrumbShouldBeSet()
        {
            //arrange
            var organisationName = Faker.Company.Name();
            var organisationId = Guid.NewGuid();

            A.CallTo(() => Cache.FetchOrganisationName(organisationId)).Returns(organisationName);

            //act
            await Controller.ViewDraftEvidenceNote(organisationId, AatfId, EvidenceNoteId);

            //assert
            Breadcrumb.ExternalActivity.Should().Be(BreadCrumbConstant.AatfManageEvidence);
            Breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            Breadcrumb.OrganisationId.Should().Be(organisationId);
        }
    }
}