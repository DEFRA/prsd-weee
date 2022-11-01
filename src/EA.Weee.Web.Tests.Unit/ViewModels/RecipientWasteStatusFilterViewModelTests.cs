namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class RecipientWasteStatusFilterViewModelTests : SimpleUnitTestBase
    {
        [Theory]
        [InlineData("ReceivedId", "Recipient")]
        [InlineData("WasteTypeValue", "Obligation type")]
        [InlineData("NoteStatusValue", "Status")]
        [InlineData("SubmittedBy", "Submitted by")]
        public void RecipientWasteStatusFilterViewModel_Properties_ShouldHaveDisplayAttributes(string property, string display)
        {
            typeof(RecipientWasteStatusFilterViewModel).GetProperty(property).Should()
                .BeDecoratedWith<DisplayAttribute>(a => a.Name.Equals(display));
        }

        [Fact]
        public void SearchPerformed_GivenNoSearchPerformed_FalseShouldBeReturned()
        {
            //arrange
            var model = new RecipientWasteStatusFilterViewModel();

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeFalse();
        }

        [Fact]
        public void SearchPerformed_GivenGivenNoteStatusIsSelected_TrueShouldBeReturned()
        {
            //arrange
            var model = new RecipientWasteStatusFilterViewModel()
            {
                NoteStatusValue = TestFixture.Create<NoteStatus>()
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }

        [Fact]
        public void SearchPerformed_GivenGivenWasteTypeIsSelected_TrueShouldBeReturned()
        {
            //arrange
            var model = new RecipientWasteStatusFilterViewModel()
            {
                WasteTypeValue = TestFixture.Create<WasteType>()
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }

        [Fact]
        public void SearchPerformed_GivenGivenRecipientIsSelected_TrueShouldBeReturned()
        {
            //arrange
            var model = new RecipientWasteStatusFilterViewModel()
            {
                ReceivedId = TestFixture.Create<Guid>()
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }
    }
}
