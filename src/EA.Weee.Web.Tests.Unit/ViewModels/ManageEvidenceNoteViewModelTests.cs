namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Web.ViewModels.Shared;
    using FluentAssertions;
    using System;
    using Weee.Tests.Core;
    using Xunit;

    public class ManageEvidenceNoteViewModelTests : SimpleUnitTestBase
    {
        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseFilterViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.FilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseRecipientWasteStatusViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.RecipientWasteStatusFilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void ManageEvidenceNoteViewModel_Constructor_ShouldInitialiseSubmittedDatesViewModel()
        {
            //act
            var model = new ManageEvidenceNoteViewModel();

            //assert
            model.SubmittedDatesFilterViewModel.Should().NotBeNull();
        }

        [Fact]
        public void SearchPerformed_GivenNoSearchPerformed_FalseShouldBeReturned()
        {
            //arrange
            var model = new ManageEvidenceNoteViewModel();

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeFalse();
        }

        [Fact]
        public void SearchPerformed_GivenFilterSearchPerformed_TrueShouldBeReturned()
        {
            //arrange
            var model = new ManageEvidenceNoteViewModel()
            {
                FilterViewModel = new FilterViewModel()
                {
                    SearchRef = TestFixture.Create<string>()
                }
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }

        [Fact]
        public void SearchPerformed_GivenDateSearchPerformed_TrueShouldBeReturned()
        {
            //arrange
            var model = new ManageEvidenceNoteViewModel()
            {
                SubmittedDatesFilterViewModel = new SubmittedDatesFilterViewModel()
                {
                    StartDate = TestFixture.Create<DateTime>()
                }
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }

        [Fact]
        public void SearchPerformed_GivenRecipientPerformed_TrueShouldBeReturned()
        {
            //arrange
            var model = new ManageEvidenceNoteViewModel()
            {
                RecipientWasteStatusFilterViewModel = new RecipientWasteStatusFilterViewModel()
                {
                    WasteTypeValue = TestFixture.Create<WasteType>()
                }
            };

            //act
            var searchPerformed = model.SearchPerformed;

            //assert
            searchPerformed.Should().BeTrue();
        }
    }
}
