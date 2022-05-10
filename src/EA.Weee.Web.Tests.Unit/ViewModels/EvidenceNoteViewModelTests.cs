﻿namespace EA.Weee.Web.Tests.Unit.ViewModels
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
        [InlineData("StartDate", "Start date")]
        [InlineData("EndDate", "End date")]
        [InlineData("ProtocolValue", "Actual or protocol")]
        [InlineData("WasteTypeValue", "Type of waste")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveDisplayAttribute(string property, string description)
        {
            typeof(EvidenceNoteViewModel)
                .GetProperty(property)
                .Should()
                .BeDecoratedWith<DisplayAttribute>(d => d.Name.Equals(description));
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
        public void EvidenceNoteViewModel_ReceivedTotal_ShouldCallCalculator()
        {
            model.CategoryValues.Add(new EvidenceCategoryValue(WeeeCategory.ConsumerEquipment) { Received = "1" });

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
        [Theory]
        [InlineData(NoteStatus.Rejected)]
        [InlineData(NoteStatus.Returned)]
        [InlineData(NoteStatus.Draft)]
        public void RedirectTab_GivenStatusIsDraftReturnRejected_EditDraftAndReturnedNotesTabShouldBeReturned(NoteStatus status)
        {
            model.Status = status;

            var tab = model.RedirectTab;

            tab.Should().Be(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes);
        }

        [Theory]
        [InlineData(NoteStatus.Void)]
        [InlineData(NoteStatus.Submitted)]
        [InlineData(NoteStatus.Approved)]
        public void RedirectTab_GivenStatusIsNotDraftReturnRejected_ViewAllOtherEvidenceNotesShouldBeReturned(NoteStatus status)
        {
            model.Status = status;

            var tab = model.RedirectTab;

            tab.Should().Be(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes);
        }
    }
}
