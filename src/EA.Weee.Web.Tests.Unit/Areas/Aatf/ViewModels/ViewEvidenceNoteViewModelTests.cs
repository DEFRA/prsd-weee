namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using Core.AatfEvidence;
    using Core.Tests.Unit.Helpers;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ViewEvidenceNoteViewModelTests
    {
        [Fact]
        public void ViewEvidenceNoteViewModel_ShouldBeDerivedFromEvidenceNoteViewModel()
        {
            typeof(ViewEvidenceNoteViewModel).Should().BeDerivedFrom<EvidenceNoteViewModel>();
        }

        [Theory]
        [InlineData("ReferenceDisplay", "Reference ID")]
        [InlineData("ProtocolDisplay", "Actual or protocol")]
        [InlineData("WasteDisplay", "Type of waste")]
        [InlineData("ComplianceYearDisplay", "Compliance year")]
        public void ViewEvidenceNoteViewModel_Properties_ShouldHaveDisplayAttributes(string property, string display)
        {
            typeof(ViewEvidenceNoteViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayNameAttribute>(a => a.DisplayName.Equals(display));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData(null)]
        public void DisplayMessage_GivenEmptySuccessMessage_ShouldReturnFalse(string value)
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                SuccessMessage = value
            };

            //act
            var result = model.DisplayMessage;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void DisplayMessage_GivenSuccessMessage_ShouldReturnTrue()
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                SuccessMessage = "a"
            };

            //act
            var result = model.DisplayMessage;

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ProtocolDisplay_GivenProtocolIsNull_EmptyStringShouldBeReturned()
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                ProtocolValue = null
            };

            //act
            var result = model.ProtocolDisplay;

            //assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void WasteDisplay_GivenWasteTypeIsNull_EmptyStringShouldBeReturned()
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                WasteTypeValue = null
            };

            //act
            var result = model.WasteDisplay;

            //assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void WasteDisplay_GivenWasteType_WasteTypeDisplayShouldBeReturned()
        {
            //arrange
            foreach (var waste in EnumHelper.GetValues(typeof(WasteType)))
            {
                var model = new ViewEvidenceNoteViewModel()
                {
                    WasteTypeValue = (WasteType)waste.Key
                };

                //act
                var result = model.WasteDisplay;
                
                //assert
                result.Should().Be(EnumHelper.GetDisplayName((WasteType)waste.Key));
            }
        }

        [Fact]
        public void ProtocolDisplay_GivenProtocol_ProtocolDisplayShouldBeReturned()
        {
            //arrange
            foreach (var protocol in EnumHelper.GetValues(typeof(Protocol)))
            {
                var model = new ViewEvidenceNoteViewModel()
                {
                    ProtocolValue = (Protocol)protocol.Key
                };

                //act
                var result = model.ProtocolDisplay;

                //assert
                result.Should().Be(EnumHelper.GetDisplayName((Protocol)protocol.Key));
            }
        }

        [Fact]
        public void DisplayEditButton_GivenDraft_ShouldBeTrue()
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = NoteStatus.Draft
            };

            //act
            var result = model.DisplayEditButton;

            //assert
            result.Should().BeTrue();
        }

        [Fact]
        public void DisplayEditButton_GivenReturned_ShouldBeTrue()
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = NoteStatus.Returned
            };

            //act
            var result = model.DisplayEditButton;

            //assert
            result.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void DisplayEditButton_GivenNotDraft_ShouldBeFalse(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Draft) || status.Equals(NoteStatus.Returned))
            {
                return;
            }

            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = status
            };

            //act
            var result = model.DisplayEditButton;

            //assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasSubmittedDate_GivenSubmittedDate_ShouldBeTrue()
        {
            var model = new ViewEvidenceNoteViewModel()
            {
                SubmittedDate = "Test",
            };

            var result = model.HasSubmittedDate;

            result.Should().BeTrue();
        }

        [Fact]
        public void HasSubmittedDate_GivenNoSubmittedDate_ShouldBeFalse()
        {
            var model = new ViewEvidenceNoteViewModel();

            var result = model.HasSubmittedDate;

            result.Should().BeFalse();
        }

        [Fact]
        public void HasApprovedDate_GivenApprovedDate_ShouldBeTrue()
        {
            var model = new ViewEvidenceNoteViewModel()
            {
                ApprovedDate = "Test",
            };

            var result = model.HasApprovedDate;

            result.Should().BeTrue();
        }

        [Fact]
        public void HasApprovedDate_GivenNoApprovedDate_ShouldBeFalse()
        {
            var model = new ViewEvidenceNoteViewModel();

            var result = model.HasApprovedDate;

            result.Should().BeFalse();
        }

        [Fact]
        public void HasReturnedDate_GivenStatusIsReturned_ShouldReturnTrue()
        {
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = NoteStatus.Returned
            };

            model.HasBeenReturned.Should().BeTrue();
        }

        [Fact]
        public void HasRejectedDate_GivenStatusIsRejected_ShouldReturnTrue()
        {
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = NoteStatus.Rejected
            };

            model.HasRejectedDate.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void HasReturnedDate_GivenStatusIsNotReturned_ShouldReturnFalse(NoteStatus status)
        {
            if (status.Equals(NoteStatus.Returned))
            {
                return;
            }

            var model = new ViewEvidenceNoteViewModel()
            {
                Status = status
            };

            model.HasBeenReturned.Should().BeFalse();
        }
    }
}
