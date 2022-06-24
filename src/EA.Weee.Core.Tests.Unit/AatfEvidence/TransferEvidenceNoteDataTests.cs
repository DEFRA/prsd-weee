namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Helpers;
    using DataReturns;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferEvidenceNoteDataTests : SimpleUnitTestBase
    {
        [Fact]
        public void TransferEvidenceNoteData_ShouldBeDerivedFromEvidenceNoteDataBase()
        {
            typeof(TransferEvidenceNoteData).Should().BeDerivedFrom<EvidenceNoteDataBase>();
        }

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

        [Fact]
        public void CategoryIds_GivenTransferEvidenceNoteTonnageDataWithDuplicateCategories_CorrectCategoriesShouldBeReturned()
        {
            //arrange
            var model = new TransferEvidenceNoteData()
            {
                TransferEvidenceNoteTonnageData = new List<TransferEvidenceNoteTonnageData>()
                {
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(t => t.EvidenceTonnageData, new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, null, null, null, null)).Create(),
                    TestFixture.Build<TransferEvidenceNoteTonnageData>()
                        .With(t => t.EvidenceTonnageData, new EvidenceTonnageData(TestFixture.Create<Guid>(), WeeeCategory.ITAndTelecommsEquipment, null, null, null, null)).Create()
                }
            };

            //act
            var categories = model.CategoryIds;

            //assert
            categories.Count.Should().Be(1);
            categories.Should().Contain(WeeeCategory.ITAndTelecommsEquipment.ToInt());
        }
    }
}
