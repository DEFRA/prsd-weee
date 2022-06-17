namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using DataReturns;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNoteDataTests : SimpleUnitTestBase
    {
        [Fact]
        public void CategoryIds_GivenTransferEvidenceNoteTonnageDataIsNull_EmptyListShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteData();

            //act
            var categories = model.CategoryIds;

            //assert
            categories.Should().BeEmpty();
        }

        [Fact]
        public void CategoryIds_GivenTransferEvidenceNoteTonnageDataHasValues_CorrectCategoriesShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteData()
            {
                TransferEvidenceNoteTonnageData = TestFixture.CreateMany<TransferEvidenceNoteTonnageData>().ToList()
            };

            //act
            var categories = model.CategoryIds;

            //assert
            categories.Count.Should().BeGreaterThan(0);
            categories.Should().BeEquivalentTo(model.TransferEvidenceNoteTonnageData
                .Select(t => t.EvidenceTonnageData.CategoryId).Cast<int>().ToList());
        }
    }
}
