namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class EditNoteViewModelMapTests
    {
        private readonly Fixture fixture;
        private readonly EditNoteViewModelMap map;

        public EditNoteViewModelMapTests()
        {
            fixture = new Fixture();
            map = new EditNoteViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => map.Map(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSourceWithNoteDataAndNullExistingModel_PropertiesShouldBeMapped()
        {
            //arrange
            var source = fixture.Create<EditNoteMapTransfer>();
            source.ExistingModel = null;

            //act
            var result = map.Map(source);

            //assert
            result.Id.Should().Be(source.NoteData.Id);
            result.Status.Should().Be(source.NoteData.Status);
            result.Reference.Should().Be(source.NoteData.Reference);
            result.Type.Should().Be(source.NoteData.Type);
            result.OrganisationId.Should().Be(source.OrganisationId);
            result.AatfId.Should().Be(source.AatfId);
            result.SchemeList.Should().BeEquivalentTo(source.Schemes);
            result.ProtocolList.Should()
                .BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"));
            result.WasteTypeList.Should()
                .BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(WasteType)), "Key", "Value"));
            result.ReceivedId.Should().Be(source.NoteData.RecipientId);
            result.StartDate.Should().Be(source.NoteData.StartDate);
            result.EndDate.Should().Be(source.NoteData.EndDate);
            result.WasteTypeValue.Should().Be(source.NoteData.WasteType);
            result.ProtocolValue.Should().Be(source.NoteData.Protocol);
            result.ReturnedReason.Should().Be(source.NoteData.ReturnedReason);
            result.RejectedReason.Should().Be(source.NoteData.RejectedReason);
            result.SelectedSchemeName.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Map_GivenSourceWithEvidenceTonnageDataAndNullExistingModel_CategoriesShouldBeMapped()
        {
            //arrange
            var source = fixture.Create<EditNoteMapTransfer>();
            source.ExistingModel = null;
            source.NoteData.EvidenceTonnageData = new List<EvidenceTonnageData>()
            {
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ConsumerEquipment, null, null),
                new EvidenceTonnageData(Guid.NewGuid(), WeeeCategory.ElectricalAndElectronicTools, 1, 2)
            };

            //act
            var result = map.Map(source);

            //assert
            var category1 = result.CategoryValues.FirstOrDefault(c => c.CategoryId.Equals(WeeeCategory.ConsumerEquipment.ToInt()));
            category1.Received.Should().Be(string.Empty);
            category1.Reused.Should().Be(string.Empty);
            category1.Id.Equals(source.NoteData.EvidenceTonnageData.ElementAt(0).Id);
            category1.CategoryId.Equals(source.NoteData.EvidenceTonnageData.ElementAt(0).CategoryId.ToInt());

            var category2 = result.CategoryValues.FirstOrDefault(c => c.CategoryId.Equals(WeeeCategory.ElectricalAndElectronicTools.ToInt()));

            category2.Received.Should().Be("1.000");
            category2.Reused.Should().Be("2.000");
            category2.Id.Equals(source.NoteData.EvidenceTonnageData.ElementAt(1).Id);
            category2.CategoryId.Equals(source.NoteData.EvidenceTonnageData.ElementAt(1).CategoryId.ToInt());

            var notInCategories = new List<int>() { WeeeCategory.ConsumerEquipment.ToInt(), WeeeCategory.ElectricalAndElectronicTools.ToInt() };

            foreach (var evidenceCategoryValue in result.CategoryValues.Where(c => !notInCategories.Contains(c.CategoryId)))
            {
                evidenceCategoryValue.Received.Should().BeNull();
                evidenceCategoryValue.Reused.Should().BeNull();
            }
        }

        [Fact]
        public void Map_GivenSourceWithExistingModel_PropertiesShouldBeMapped()
        {
            //arrange
            var source = fixture.Create<EditNoteMapTransfer>();

            //act
            var result = map.Map(source);

            //assert
            result.Id.Should().Be(source.ExistingModel.Id);
            result.Status.Should().Be(source.ExistingModel.Status);
            result.Reference.Should().Be(source.ExistingModel.Reference);
            result.Type.Should().Be(source.ExistingModel.Type);
            result.OrganisationId.Should().Be(source.ExistingModel.OrganisationId);
            result.AatfId.Should().Be(source.ExistingModel.AatfId);
            result.SchemeList.Should().BeEquivalentTo(source.Schemes);
            result.ProtocolList.Should()
                .BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(Protocol)), "Key", "Value"));
            result.WasteTypeList.Should()
                .BeEquivalentTo(new SelectList(EnumHelper.GetValues(typeof(WasteType)), "Key", "Value"));
            result.ReceivedId.Should().Be(source.ExistingModel.ReceivedId);
            result.StartDate.Should().Be(source.ExistingModel.StartDate);
            result.EndDate.Should().Be(source.ExistingModel.EndDate);
            result.WasteTypeValue.Should().Be(source.ExistingModel.WasteTypeValue);
            result.ProtocolValue.Should().Be(source.ExistingModel.ProtocolValue);
            result.CategoryValues.Should().BeEquivalentTo(source.ExistingModel.CategoryValues);
            result.ReturnedReason.Should().Be(source.ExistingModel.ReturnedReason);
            result.RejectedReason.Should().Be(source.ExistingModel.RejectedReason);
            result.SelectedSchemeName.Should().BeNullOrEmpty();
        }
        [Fact]
        public void Map_GivenSourceSchemesContainRecipientId_SelectedSchemeNameShouldBeMapped()
        {
            //arrange
            var source = fixture.Create<EditNoteMapTransfer>();
            source.ExistingModel = null;
            var recipientId = source.NoteData.RecipientId;
            source.Schemes[0].Id = recipientId;
           
            //act
            var result = map.Map(source);

            // assert
            result.SelectedSchemeName.Should().Be(source.Schemes[0].SchemeNameDisplay);
        }
    }
}
