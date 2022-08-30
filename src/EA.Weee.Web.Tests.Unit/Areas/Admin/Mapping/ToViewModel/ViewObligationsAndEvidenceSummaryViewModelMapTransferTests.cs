namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Core.Admin.Obligation;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weee.Tests.Core;
    using Xunit;

    public class ViewObligationsAndEvidenceSummaryViewModelMapTransferTests : SimpleUnitTestBase
    {
        private ObligationEvidenceSummaryData ObligationEvidenceSummaryData { get; set; }
        private List<SchemeData> SchemeData { get; set; }

        public ViewObligationsAndEvidenceSummaryViewModelMapTransferTests()
        {
            var obligationEvidenceValues = TestFixture.CreateMany<ObligationEvidenceTonnageData>().ToList();

            SchemeData = TestFixture.CreateMany<SchemeData>().ToList();
            ObligationEvidenceSummaryData = new ObligationEvidenceSummaryData(obligationEvidenceValues);
        }

        [Fact]
        public void ViewObligationsAndEvidenceSummaryViewModelMapModel_ShouldImplementIObligationEvidenceSummaryBase()
        {
            typeof(ViewObligationsAndEvidenceSummaryViewModelMapTransfer).Should()
                .Implement<IObligationEvidenceSummaryBase>();
        }

        [Fact]
        public void ViewObligationsAndEvidenceSummaryViewModelMapModel_Constructor_PropertiesShouldBeSet()
        {
            // arrange
            var complianceYearsList = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();

            // act
            var model = new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, ObligationEvidenceSummaryData, complianceYearsList, SchemeData);

            // assert
            model.Should().NotBeNull();
            model.ObligationEvidenceSummaryData.Should().BeEquivalentTo(ObligationEvidenceSummaryData);
            model.SchemeId.Should().Be(schemeId);
            model.SchemeData.SequenceEqual(SchemeData);
            model.ComplianceYears.SequenceEqual(complianceYearsList);
        }

        [Fact]
        public void ViewObligationsAndEvidenceSummaryViewModelMapModel_Constructor_GivenSchemeIdIsDefault_ShouldThrowAnException()
        {
            // arrange
            var complianceYearsList = TestFixture.CreateMany<int>().ToList();
            var schemeId = Guid.Empty;

            //act
            var result = Record.Exception(() => new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId,
                ObligationEvidenceSummaryData,
                complianceYearsList,
                SchemeData));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ViewObligationsAndEvidenceSummaryViewModelMapModel_Constructor_GivenObligationSummaryIsNull_PropertiesShouldBeSet()
        {
            // arrange
            var complianceYearsList = TestFixture.CreateMany<int>().ToList();
            var schemeId = TestFixture.Create<Guid>();

            // act
            var model = new ViewObligationsAndEvidenceSummaryViewModelMapTransfer(schemeId, null, complianceYearsList, SchemeData);

            // assert
            model.Should().NotBeNull();
            model.ObligationEvidenceSummaryData.Should().BeNull();
            model.SchemeId.Should().Be(schemeId);
            model.SchemeData.SequenceEqual(SchemeData);
            model.ComplianceYears.SequenceEqual(complianceYearsList);
        }
    }
}
