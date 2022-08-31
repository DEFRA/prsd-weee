namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using System;
    using AutoFixture;
    using Core.Scheme;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ViewEvidenceSummaryViewModelMapTransferTests : SimpleUnitTestBase
    {
        private ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; set; }

        private SchemePublicInfo Scheme { get; set; }

        private ManageEvidenceNoteViewModel ManageEvidenceNoteViewModel { get; set; }

        public ViewEvidenceSummaryViewModelMapTransferTests()
        {
            ObligationEvidenceSummaryData = TestFixture.Create<ObligationEvidenceSummaryData>();
            Scheme = TestFixture.Create<SchemePublicInfo>();
            ManageEvidenceNoteViewModel = TestFixture.Create<ManageEvidenceNoteViewModel>();
        }

        [Fact]
        public void ViewEvidenceSummaryViewModelMapTransferModel_Constructor_ShouldImplementIObligationEvidenceSummaryBase()
        {
            //arrange
            typeof(ViewEvidenceSummaryViewModelMapTransfer).Should()
                .Implement<IObligationEvidenceSummaryBase>();
        }

        [Fact]
        public void ViewEvidenceSummaryViewModelMapTransferModel_Constructor_GivenOrganisationIdIsDefault_ShouldThrowAnException()
        {
            // arrange
            var currentDate = DateTime.Now;
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            //act
            var result = Record.Exception(() => new ViewEvidenceSummaryViewModelMapTransfer(Guid.Empty, 
                ObligationEvidenceSummaryData, ManageEvidenceNoteViewModel, Scheme, currentDate, complianceYear));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewEvidenceSummaryViewModelMapTransferModel_Constructor_PropertiesShouldBeSet()
        {
            // arrange
            var currentDate = DateTime.Now;
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            // act
            var model = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, ObligationEvidenceSummaryData, ManageEvidenceNoteViewModel, Scheme, currentDate, complianceYear);

            // assert
            model.Should().NotBeNull();
            model.ObligationEvidenceSummaryData.Should().BeEquivalentTo(ObligationEvidenceSummaryData);
            model.Scheme.Should().BeEquivalentTo(Scheme);
            model.ComplianceYear.Should().Be(complianceYear);
            model.CurrentDate.Should().Be(currentDate);
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(ManageEvidenceNoteViewModel);
            model.OrganisationId.Should().Be(organisationId);        
        }

        [Fact]
        public void ViewEvidenceSummaryViewModelMapTransferModel_Constructor_GivenManageEvidenceNoteViewModelIsNull_PropertiesShouldBeSet()
        {
            // arrange
            var currentDate = DateTime.Now;
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            // act
            var model = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, ObligationEvidenceSummaryData, null, Scheme, currentDate, complianceYear);

            // assert
            model.Should().NotBeNull();
            model.ObligationEvidenceSummaryData.Should().BeEquivalentTo(ObligationEvidenceSummaryData);
            model.Scheme.Should().BeEquivalentTo(Scheme);
            model.ComplianceYear.Should().Be(complianceYear);
            model.CurrentDate.Should().Be(currentDate);
            model.ManageEvidenceNoteViewModel.Should().BeNull();
            model.OrganisationId.Should().Be(organisationId);
        }

        [Fact]
        public void ViewEvidenceSummaryViewModelMapTransferModel_Constructor_GivenObligationEvidenceSummaryDataIsNull_PropertiesShouldBeSet()
        {
            // arrange
            var currentDate = DateTime.Now;
            var organisationId = TestFixture.Create<Guid>();
            var complianceYear = TestFixture.Create<int>();

            // act
            var model = new ViewEvidenceSummaryViewModelMapTransfer(organisationId, null, ManageEvidenceNoteViewModel, Scheme, currentDate, complianceYear);

            // assert
            model.Should().NotBeNull();
            model.ObligationEvidenceSummaryData.Should().BeNull();
            model.Scheme.Should().BeEquivalentTo(Scheme);
            model.ComplianceYear.Should().Be(complianceYear);
            model.CurrentDate.Should().Be(currentDate);
            model.ManageEvidenceNoteViewModel.Should().BeEquivalentTo(ManageEvidenceNoteViewModel);
            model.OrganisationId.Should().Be(organisationId);
        }
    }
}
