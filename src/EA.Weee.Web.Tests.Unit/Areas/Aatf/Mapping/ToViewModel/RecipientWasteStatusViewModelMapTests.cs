namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using AutoFixture;
    using EA.Prsd.Core.Helpers;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.Aatf.Mappings;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using Xunit;

    public class RecipientWasteStatusViewModelMapTests
    {
        private readonly RecipientWasteStatusViewModelMap mapper;

        private Fixture fixture;

        public RecipientWasteStatusViewModelMapTests()
        {
            fixture = new Fixture();

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
        public void Map_GivenSchemeDataIsNull_PropertiesShouldBeSet()
        {
            //arrange
            var source = fixture.Build<RecipientWasteStatusFilterBase>().With(r => r.SchemeList, (List<SchemeData>)null).Create();
            var emptyList = new List<SchemeData>();
            //act
            var model = mapper.Map(source);

            //assert
            model.NoteStatusList.Should().BeEquivalentTo(GetNoteStatusList());
            model.NoteStatusValue.Should().Be(source.NoteStatus);
            model.WasteTypeList.Should().BeEquivalentTo(GetWasteTypeValuesList());
            model.WasteTypeValue.Should().Be(source.WasteType);
            model.SchemeListSL.Should().BeEquivalentTo(GetSchemeListDropDownForm(emptyList));
            model.ReceivedId.Should().Be(source.ReceivedId);
        }

        [Fact]
        public void Map_GivenSource_PropertiesShouldBeSet()
        {
            //arrange
            var source = fixture.Create<RecipientWasteStatusFilterBase>();

            //act
            var model = mapper.Map(source);

            //assert
            model.NoteStatusList.Should().BeEquivalentTo(GetNoteStatusList());
            model.NoteStatusValue.Should().Be(source.NoteStatus);
            model.WasteTypeList.Should().BeEquivalentTo(GetWasteTypeValuesList());
            model.WasteTypeValue.Should().Be(source.WasteType);
            model.SchemeListSL.Should().BeEquivalentTo(GetSchemeListDropDownForm(source.SchemeList));
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

        private IEnumerable<SelectListItem> GetWasteTypeValuesList()
        {
            return new SelectList(EnumHelper.GetOrderedValues(typeof(WasteType)), "Key", "Value");
        }

        private IEnumerable<SelectListItem> GetSchemeListDropDownForm(IList<SchemeData> list)
        {
            return new SelectList(list, "Id", "SchemeName");
        }
    }
}
