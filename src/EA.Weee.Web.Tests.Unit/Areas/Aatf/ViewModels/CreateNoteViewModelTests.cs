﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class CreateNoteViewModelTests
    {
        private readonly CreateNoteViewModel model;

        public CreateNoteViewModelTests()
        {
            model = new CreateNoteViewModel();
        }

        [Theory]
        [InlineData("StartDate", "Start date")]
        [InlineData("EndDate", "End date")]
        [InlineData("ReceivedId", "Recipient")]
        [InlineData("ProtocolValue", "Actual or protocol")]
        [InlineData("WasteTypeValue", "Type of waste")]
        public void CreateNoteViewModel_Properties_ShouldHaveDisplayAttribute(string property, string description)
        {
            typeof(CreateNoteViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<DisplayAttribute>(d => d.Name.Equals(description));
        }

        [Theory]
        [InlineData("StartDate")]
        [InlineData("EndDate")]
        [InlineData("ReceivedId")]
        public void CreateNoteViewModel_Properties_ShouldHaveRequiredAttribute(string property)
        {
            typeof(CreateNoteViewModel).GetProperty(property).Should().BeDecoratedWith<RequiredAttribute>();
        }

        [Theory]
        [InlineData("StartDate", DataType.Date)]
        [InlineData("EndDate", DataType.Date)]
        public void CreateNoteViewModel_Properties_ShouldHaveDataTypeAttribute(string property, DataType type)
        {
            typeof(CreateNoteViewModel).GetProperty(property).Should().BeDecoratedWith<DataTypeAttribute>(d => d.DataType.Equals(type));
        }

        [Fact]
        public void CreateNoteViewModel_Constructor_ShouldPopulateEvidenceCategoryValues()
        {
            var evidenceCategoryValues = new EvidenceCategoryValues();
            for (var count = 0; count < evidenceCategoryValues.Count; count++)
            {
                model.CategoryValues.ElementAt(count).Should().BeEquivalentTo(evidenceCategoryValues.ElementAt(count));
            }
        }

        [Fact]
        public void Edit_GivenSavedCategoryValuesExist_EditShouldBeTrue()
        {
            for (var count = 0; count < new EvidenceCategoryValues().Count; count++)
            {
                var local = new CreateNoteViewModel();
                local.CategoryValues.ElementAt(0).Id = Guid.NewGuid();
                local.Edit.Should().BeTrue();
            }
        }

        [Fact]
        public void CreateNoteViewModel_StartDate_ShouldHaveStartDateAttribute()
        {
            typeof(CreateNoteViewModel).GetProperty("StartDate").Should().BeDecoratedWith<EvidenceNoteStartDateAttribute>();
        }

        [Fact]
        public void CreateNoteViewModel_EndDate_ShouldHaveStartDateAttribute()
        {
            typeof(CreateNoteViewModel).GetProperty("EndDate").Should().BeDecoratedWith<EvidenceNoteEndDateAttribute>();
        }
    }
}