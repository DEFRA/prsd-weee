namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Xunit;

    public class EvidenceSummaryViewModelTests
    {
        private readonly EvidenceSummaryMap mapper;
        private ICategoryValueTotalCalculator categoryValueTotalCalculator;
        private ITonnageUtilities tonnageUtilities;
        private Fixture fixture;

        public EvidenceSummaryViewModelTests()
        {
            categoryValueTotalCalculator = A.Fake<ICategoryValueTotalCalculator>();
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            fixture = new Fixture();

            mapper = new EvidenceSummaryMap(categoryValueTotalCalculator, tonnageUtilities);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => mapper.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var source = fixture.Create<EvidenceSummaryMapTransfer>();

            //act
            var model = mapper.Map(source);

            //assert
            model.NumberOfApprovedNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfApprovedNotes.ToString());
            model.NumberOfSubmittedNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfSubmittedNotes.ToString());
            model.NumberOfDraftNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfDraftNotes.ToString());
            model.ManageEvidenceNoteViewModel.OrganisationId.Should().Be(source.OrganisationId);
            model.ManageEvidenceNoteViewModel.AatfId.Should().Be(source.AatfId);
        }

        [Fact]
        public void Map_GivenSource_TotalsShouldBeCalculated()
        {
            //arrange
            var listEvidenceSummaryTonnageData = new List<EvidenceSummaryTonnageData>
            {
                new EvidenceSummaryTonnageData(WeeeCategory.ConsumerEquipment, 1, 2),
                new EvidenceSummaryTonnageData(WeeeCategory.DisplayEquipment, 3, 4),
                new EvidenceSummaryTonnageData(WeeeCategory.ElectricalAndElectronicTools, null, 10),
            };

            var aatfSummaryData = new AatfEvidenceSummaryData(listEvidenceSummaryTonnageData, fixture.Create<int>(),
                fixture.Create<int>(), fixture.Create<int>());

            var source =
                new EvidenceSummaryMapTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), aatfSummaryData);

            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(1)).Returns("1");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(2)).Returns("2");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(3)).Returns("3");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(4)).Returns("4");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(null)).Returns("-");
            A.CallTo(() => tonnageUtilities.CheckIfTonnageIsNull(10)).Returns("10");

            A.CallTo(() => categoryValueTotalCalculator.Total(A<List<string>>.That.IsSameSequenceAs(
                new List<string>() {null, null, null, "1", null, "-", null, null, null, null, "3", null, null, null}))).Returns("4");
            A.CallTo(() => categoryValueTotalCalculator.Total(A<List<string>>.That.IsSameSequenceAs(
                new List<string>() { null, null, null, "2", null, "10", null, null, null, null, "4", null, null, null }))).Returns("16");

            //act
            var model = mapper.Map(source);

            //assert
            model.NumberOfApprovedNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfApprovedNotes.ToString());
            model.NumberOfSubmittedNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfSubmittedNotes.ToString());
            model.NumberOfDraftNotes.Should().Be(source.AatfEvidenceSummaryData.NumberOfDraftNotes.ToString());
            model.TotalReceivedEvidence.Should().Be("4");
            model.TotalReuseEvidence.Should().Be("16");
            model.ManageEvidenceNoteViewModel.OrganisationId.Should().Be(source.OrganisationId);
            model.ManageEvidenceNoteViewModel.AatfId.Should().Be(source.AatfId);
        }
    }
}
