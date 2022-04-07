namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using System.ComponentModel;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Web.Areas.Aatf.ViewModels;
    using Xunit;

    public class ViewEvidenceNoteViewModelTests
    {
        [Theory]
        [InlineData("ReferenceDisplay", "Reference ID")]
        [InlineData("ProtocolDisplay", "Protocol")]
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
        public void ReferenceDisplay_ShouldFormatCorrectly()
        {
            var types = EnumHelper.GetValues(typeof(NoteType));

            foreach (var type in types)
            {
                var model = new ViewEvidenceNoteViewModel()
                {
                    Type = (NoteType)type.Key,
                    Reference = 1
                };

                model.ReferenceDisplay.Should().Be($"{type.Value}1");
            }
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
    }
}
