namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Mapping
{
    using AutoFixture;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.Mappings.ToViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class EvidenceNoteHistoryDataViewModelMapTests
    {
        private readonly EvidenceNoteHistoryDataViewModelMap map;
        private readonly Fixture fixture;

        public EvidenceNoteHistoryDataViewModelMapTests()
        {
            map = new EvidenceNoteHistoryDataViewModelMap();
            fixture = new Fixture();
        }

        [Fact]
        public void Map_ShouldCreateAndReturnEvidenceNoteHistoryDataViewModel()
        {
            //arrange
            var data = new List<EvidenceNoteHistoryData>()
            {
                fixture.Create<EvidenceNoteHistoryData>(),
            };

            var transfer = new EvidenceNoteHistoryDataViewModelMapTransfer(data);

            //act
            var result = map.Map(transfer);

            //assert
            result.First().Id.Should().Be(data.First().Id);
            result.First().Reference.Should().Be(data.First().Reference);
            result.First().Status.Should().Be(data.First().Status);
            result.First().Type.Should().Be(data.First().Type);
            result.First().TransferredTo.Should().Be(data.First().TransferredTo);
            result.First().SubmittedDate.Should().Be(data.First().SubmittedDate);
        }
    }
}
