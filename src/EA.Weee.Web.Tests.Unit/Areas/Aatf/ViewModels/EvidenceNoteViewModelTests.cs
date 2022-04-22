namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Xunit;

    public class EvidenceNoteViewModelTests
    {
        private readonly EvidenceNoteViewModel model;
        private readonly ICategoryValueTotalCalculator calculator;

        public EvidenceNoteViewModelTests()
        {
            calculator = A.Fake<ICategoryValueTotalCalculator>();

            model = new EvidenceNoteViewModel(calculator);
        }

        [Theory]
        [InlineData("ReferenceDisplay", "Reference ID")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveDisplayAttributes(string property, string display)
        {
            typeof(EvidenceNoteViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals(display));
        }

        [Theory]
        [InlineData("ProtocolValue", "Select actual or protocol")]
        [InlineData("WasteTypeValue", "Select a type of waste")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveRequiredSubmitActionAttribute(string property, string description)
        {
            typeof(EvidenceNoteViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<RequiredSubmitActionAttribute>(d => d.ErrorMessage.Equals(description));
        }

        [Theory]
        [InlineData("StartDate", "Start date")]
        [InlineData("EndDate", "End date")]
        [InlineData("ReceivedId", "Recipient")]
        [InlineData("ProtocolValue", "Actual or protocol")]
        [InlineData("WasteTypeValue", "Type of waste")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveDisplayAttribute(string property, string description)
        {
            typeof(EvidenceNoteViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<DisplayAttribute>(d => d.Name.Equals(description));
        }

        [Theory]
        [InlineData("StartDate", "Enter a start date")]
        [InlineData("EndDate", "Enter an end date")]
        [InlineData("ReceivedId", "Select a receiving PCS")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveRequiredAttribute(string property, string message)
        {
            typeof(EvidenceNoteViewModel).GetProperty(property).Should().BeDecoratedWith<RequiredAttribute>(r => r.ErrorMessage.Equals(message));
        }

        [Theory]
        [InlineData("StartDate", DataType.Date)]
        [InlineData("EndDate", DataType.Date)]
        public void EvidenceNoteViewModel_Properties_ShouldHaveDataTypeAttribute(string property, DataType type)
        {
            typeof(EvidenceNoteViewModel).GetProperty(property).Should().BeDecoratedWith<DataTypeAttribute>(d => d.DataType.Equals(type));
        }

        [Fact]
        public void EvidenceNoteViewModel_Constructor_ShouldPopulateEvidenceCategoryValues()
        {
            var evidenceCategoryValues = new EvidenceCategoryValues();
            for (var count = 0; count < evidenceCategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Should().BeEquivalentTo(evidenceCategoryValues.ElementAt(count));
            }
        }

        [Fact]
        public void EvidenceNoteViewModel_Edit_GivenSavedCategoryValuesExist_EditShouldBeTrue()
        {
            for (var count = 0; count < new EvidenceCategoryValues().Count; count++)
            {
                var local = new EvidenceNoteViewModel();
                local.CategoryValues.ElementAt(0).Id = Guid.NewGuid();
                local.Edit.Should().BeTrue();
            }
        }

        [Fact]
        public void EvidenceNoteViewModel_StartDate_ShouldHaveStartDateAttribute()
        {
            typeof(EvidenceNoteViewModel).GetProperty("StartDate").Should().BeDecoratedWith<EvidenceNoteStartDateAttribute>();
        }

        [Fact]
        public void EvidenceNoteViewModel_EndDate_ShouldHaveStartDateAttribute()
        {
            typeof(EvidenceNoteViewModel).GetProperty("EndDate").Should().BeDecoratedWith<EvidenceNoteEndDateAttribute>();
        }

        [Fact]
        public void EvidenceNoteViewModel_ReceivedTotal_ShouldCallCalculator()
        {
            model.CategoryValues.Add(new EvidenceCategoryValue(WeeeCategory.ConsumerEquipment) { Received = "1"});

            var total = model.ReceivedTotal;

            A.CallTo(() => calculator.Total(A<List<string>>.That.IsSameSequenceAs(model.CategoryValues.Select(c => c.Received).ToList()))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteViewModel_ReusedTotal_ShouldCallCalculator()
        {
            model.CategoryValues.Add(new EvidenceCategoryValue(WeeeCategory.ConsumerEquipment) { Reused = "2" });

            var total = model.ReusedTotal;

            A.CallTo(() => calculator.Total(A<List<string>>.That.IsSameSequenceAs(model.CategoryValues.Select(c => c.Reused).ToList()))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EvidenceNoteViewModel_ReferenceDisplay_ShouldFormatCorrectly()
        {
            var types = EnumHelper.GetValues(typeof(NoteType));

            foreach (var type in types)
            {
                var model = new EvidenceNoteViewModel()
                {
                    Type = (NoteType)type.Key,
                    Reference = 1
                };

                model.ReferenceDisplay.Should().Be($"{type.Value}1");
            }
        }
    }
}