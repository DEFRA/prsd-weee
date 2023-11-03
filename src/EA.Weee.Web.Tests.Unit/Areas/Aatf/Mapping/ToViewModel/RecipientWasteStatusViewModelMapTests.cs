namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Aatf.Mappings;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class RecipientWasteStatusViewModelMapTests : SimpleUnitTestBase
    {
        private readonly RecipientWasteStatusViewModelMap mapper;

        public RecipientWasteStatusViewModelMapTests()
        {
            mapper = new RecipientWasteStatusViewModelMap();
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
        public void Map_GivenSchemeDataIsNullAndIsInternalIsSetToFalseAndAllStatusesIsSetToFalse_PropertiesShouldBeSet()
        {
            //arrange
            var source = TestFixture.Build<RecipientWasteStatusFilterBase>()
                .With(r => r.RecipientList, (List<EntityIdDisplayNameData>)null)
                .With(r => r.Internal, false)
                .With(r => r.AllStatuses, false)
                .Create();

            var emptyList = new List<EntityIdDisplayNameData>();

            //act
            var model = mapper.Map(source);

            //assert
            //model.NoteStatusList.Should().BeEquivalentTo(GetNoteStatusList());
            model.NoteStatusValue.Should().Be(source.NoteStatus);
            model.WasteTypeList.Should().BeEquivalentTo(GetWasteTypeValuesList());
            model.WasteTypeValue.Should().Be(source.WasteType);
            model.RecipientList.Should().BeEquivalentTo(GetSchemeListDropDownForm(emptyList));
            model.ReceivedId.Should().Be(source.ReceivedId);
        }

        [Fact]
        public void Map_GivenSchemeDataIsNullAndIsInternalIsSetToTrueAndAllStatusesIsSetToFalse_PropertiesShouldBeSet()
        {
            //arrange
            var source = TestFixture.Build<RecipientWasteStatusFilterBase>()
                .With(r => r.RecipientList, (List<EntityIdDisplayNameData>)null)
                .With(r => r.Internal, true)
                .With(r => r.AllStatuses, false)
                .Create();

            var emptyList = new List<EntityIdDisplayNameData>();

            //act
            var model = mapper.Map(source);

            //assert
            //model.NoteStatusList.Should().BeEquivalentTo(GetNoteStatusList_WithInternalTrue());
            model.NoteStatusValue.Should().Be(source.NoteStatus);
            model.WasteTypeList.Should().BeEquivalentTo(GetWasteTypeValuesList());
            model.WasteTypeValue.Should().Be(source.WasteType);
            model.RecipientList.Should().BeEquivalentTo(GetSchemeListDropDownForm(emptyList));
            model.ReceivedId.Should().Be(source.ReceivedId);
        }

        [Fact]
        public void Map_GivenSchemeDataIsNullAndIsInternalIsSetToFalseAndAllStatusesIsSetToTrue_PropertiesShouldBeSet()
        {
            //arrange
            var source = TestFixture.Build<RecipientWasteStatusFilterBase>()
                .With(r => r.RecipientList, (List<EntityIdDisplayNameData>)null)
                .With(r => r.Internal, false)
                .With(r => r.AllStatuses, true)
                .Create();

            var emptyList = new List<EntityIdDisplayNameData>();

            //act
            var model = mapper.Map(source);

            //assert
            //model.NoteStatusList.Should().BeEquivalentTo(GetNoteStatusList_WithAllStatusesTrue());
            model.NoteStatusValue.Should().Be(source.NoteStatus);
            model.WasteTypeList.Should().BeEquivalentTo(GetWasteTypeValuesList());
            model.WasteTypeValue.Should().Be(source.WasteType);
            model.RecipientList.Should().BeEquivalentTo(GetSchemeListDropDownForm(emptyList));
            model.ReceivedId.Should().Be(source.ReceivedId);
        }

        private IEnumerable<SelectListItem> GetNoteStatusList()
        {
            var statuses = new Dictionary<int, string>
            {
                { (int)NoteStatus.Submitted, NoteStatus.Submitted.ToString() },
                { (int)NoteStatus.Approved, NoteStatus.Approved.ToString() },
                { (int)NoteStatus.Rejected, NoteStatus.Rejected.ToString() },
                { (int)NoteStatus.Void, NoteStatus.Void.ToString() },
            };

            return new SelectList(statuses, "Key", "Value");
        }

        private IEnumerable<SelectListItem> GetNoteStatusList_WithInternalTrue()
        {
            var statuses = new Dictionary<int, string>
            {
                { (int)NoteStatus.Submitted, NoteStatus.Submitted.ToString() },
                { (int)NoteStatus.Approved, NoteStatus.Approved.ToString() },
                { (int)NoteStatus.Rejected, NoteStatus.Rejected.ToString() },
                { (int)NoteStatus.Returned, NoteStatus.Returned.ToString() },
                { (int)NoteStatus.Void, NoteStatus.Void.ToString() },
            };

            return new SelectList(statuses, "Key", "Value");
        }

        private IEnumerable<SelectListItem> GetNoteStatusList_WithAllStatusesTrue()
        {
            var statuses = new Dictionary<int, string>
            {
                { (int)NoteStatus.Draft, NoteStatus.Draft.ToString() },
                { (int)NoteStatus.Submitted, NoteStatus.Submitted.ToString() },
                { (int)NoteStatus.Approved, NoteStatus.Approved.ToString() },
                { (int)NoteStatus.Rejected, NoteStatus.Rejected.ToString() },
                { (int)NoteStatus.Returned, NoteStatus.Returned.ToString() },
                { (int)NoteStatus.Void, NoteStatus.Void.ToString() },
            };

            return new SelectList(statuses, "Key", "Value");
        }

        private IEnumerable<SelectListItem> GetWasteTypeValuesList()
        {
            return new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value");
        }

        private IEnumerable<SelectListItem> GetSchemeListDropDownForm(IList<EntityIdDisplayNameData> list)
        {
            return new SelectList(list, "Id", "DisplayName");
        }
    }
}
