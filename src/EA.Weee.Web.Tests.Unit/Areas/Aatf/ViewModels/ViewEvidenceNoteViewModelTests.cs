namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Scheme.ViewModels;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.ViewModels.Shared;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class ViewEvidenceNoteViewModelTests
    {
        private readonly Fixture fixture;

        public ViewEvidenceNoteViewModelTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ViewEvidenceNoteViewModel_ShouldHaveSerializableAttribute()
        {
            typeof(ViewEvidenceNoteViewModel).Should().BeDecoratedWith<SerializableAttribute>();
        }

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

        [Theory]
        [InlineData(2021)]
        [InlineData(2020)]
        [InlineData(2022)]
        public void ComplianceYearDisplay_GivenComplianceYear_ComplianceYearShouldBeDisplayedCorrectly(short year)
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                ComplianceYear = year
            };

            //act
            var result = model.ComplianceYearDisplay;

            //assert
            result.Should().Be(year.ToString());
        }

        [Theory]
        [InlineData(NoteStatus.Draft, "Draft evidence note")]
        [InlineData(NoteStatus.Rejected, "Rejected evidence note")]
        [InlineData(NoteStatus.Approved, "Approved evidence note")]
        [InlineData(NoteStatus.Returned, "Returned evidence note")]
        [InlineData(NoteStatus.Submitted, "Submitted evidence note")]
        [InlineData(NoteStatus.Void, "Voided evidence note")]
        public void TabName_GivenNoteStatus_ShouldHaveCorrectTabName(NoteStatus status, string expected)
        {
            //arrange
            var model = new ViewEvidenceNoteViewModel()
            {
                Status = status
            };

            //act
            var result = model.TabName;

            //assert
            result.Should().Be(expected);
        }

        [Fact]
        public void DisplayEvidenceNoteHistoryData_ShouldBeTrue_IfEvidenceNoteHistoryViewModel_IsPresent()
        {
            //Act
            var model = new ViewEvidenceNoteViewModel()
            {
                EvidenceNoteHistoryData = fixture.Create<IList<EvidenceNoteHistoryViewModel>>(),
            };

            //Assert
            model.DisplayEvidenceNoteHistoryData.Should().BeTrue();
        }

        [Fact]
        public void DisplayEvidenceNoteHistoryData_ShouldBeFalse_IfEvidenceNoteHistoryViewModel_IsNotPresent()
        {
            //Act
            var model = new ViewEvidenceNoteViewModel()
            {
                EvidenceNoteHistoryData = null,
            };

            //Assert
            model.DisplayEvidenceNoteHistoryData.Should().BeFalse();
        }
    }
}
