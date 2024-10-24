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
    using Core.Tests.Unit.Helpers;
    using EA.Weee.Web.Areas.Admin.ViewModels.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core.DataHelpers;
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

        [Fact]
        public void EvidenceNoteViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(EvidenceNoteViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Theory]
        [InlineData("ReferenceDisplay", "Reference ID")]
        [InlineData("StartDate", "Start date")]
        [InlineData("EndDate", "End date")]
        [InlineData("WasteTypeValue", "Obligation type")]
        [InlineData("ProtocolValue", "Actual or protocol")]
        [InlineData("SubmittedDate", "Submitted date")]
        [InlineData("RejectedDate", "Date rejected")]
        [InlineData("ReturnedDate", "Date returned")]
        [InlineData("ApprovedDate", "Date approved")]
        [InlineData("ReturnedReason", "Reason")]
        [InlineData("RejectedReason", "Reason")]
        public void EvidenceNoteViewModel_Properties_ShouldHaveDisplayAttributes(string property, string display)
        {
            typeof(EvidenceNoteViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals(display));
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
        [InlineData(NoteStatus.Returned)]
        [InlineData(NoteStatus.Draft)]
        public void RedirectTab_GivenStatusIsDraftReturnRejected_EditDraftAndReturnedNotesTabShouldBeReturned(NoteStatus status)
        {
            model.Status = status;

            var tab = model.AatfRedirectTab;

            tab.Should().Be(ManageEvidenceOverviewDisplayOption.EditDraftAndReturnedNotes.ToDisplayString());
        }

        [Theory]
        [InlineData(NoteStatus.Void)]
        [InlineData(NoteStatus.Submitted)]
        [InlineData(NoteStatus.Approved)]
        [InlineData(NoteStatus.Rejected)]
        public void RedirectTab_GivenStatusIsNotDraftReturnRejected_ViewAllOtherEvidenceNotesShouldBeReturned(NoteStatus status)
        {
            model.Status = status;

            var tab = model.AatfRedirectTab;

            tab.Should().Be(ManageEvidenceOverviewDisplayOption.ViewAllOtherEvidenceNotes.ToDisplayString());
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void DisplayReturnedReason_GivenStatusIsNotReturnedAndHasReason_ShouldReturnFalse(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Returned))
            {
                return;
            }

            var model = new EvidenceNoteViewModel()
            {
                Status = status,
                ReturnedReason = "reason"
            };

            model.DisplayReturnedReason.Should().BeFalse();
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void DisplayReturnedReason_GivenStatusIsReturnedAndReasonIsEmpty_ShouldReturnFalse(string reason)
        {
            var model = new EvidenceNoteViewModel()
            {
                Status = NoteStatus.Returned,
                ReturnedReason = reason
            };

            model.DisplayReturnedReason.Should().BeFalse();
        }

        [Fact]
        public void DisplayReturnedReason_GivenStatusIsReturnedAndReasonIsNotEmpty_ShouldReturnTrue()
        {
            var model = new EvidenceNoteViewModel()
            {
                Status = NoteStatus.Returned,
                ReturnedReason = "reason"
            };

            model.DisplayReturnedReason.Should().BeTrue();
        }

        [Fact]
        public void InternalRedirectTab_GivenTypeIsEvidence_ViewAllEvidenceNotesTabShouldBeReturned()
        {
            model.Type = NoteType.Evidence;

            var tab = model.InternalUserRedirectTab;

            tab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceNotes.ToDisplayString());
        }

        [Fact]
        public void InternalRedirectTab_GivenTypeIsTransfer_ViewAllEvidenceTransferNotesTabShouldBeReturned()
        {
            model.Type = NoteType.Transfer;

            var tab = model.InternalUserRedirectTab;

            tab.Should().Be(ManageEvidenceNotesTabDisplayOptions.ViewAllEvidenceTransfers.ToDisplayString());
        }
    }
}
