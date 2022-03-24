namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using Core.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
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
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ReceivedId")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveRequiredAttribute(string property)
        {
            typeof(EvidenceNoteViewModel).GetProperty(property).Should().BeDecoratedWith<RequiredAttribute>();
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
    }
}