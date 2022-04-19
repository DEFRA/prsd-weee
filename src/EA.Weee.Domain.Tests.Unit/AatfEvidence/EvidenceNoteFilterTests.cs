namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using EA.Weee.Core.Tests.Unit.Helpers;
    using Evidence;
    using FluentAssertions;
    using Xunit;

    public class EvidenceNoteFilterTests
    {
        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedSearchRef_GivenSearchRefWithNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{noteType.DisplayName}1"
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedSearchRef_GivenSearchRefWithNoteTypeAndIsLowerCase_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{noteType.DisplayName.ToLower()}1"
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [InlineData("X")]
        [InlineData("Y")]
        [InlineData("B")]
        [InlineData("*")]
        [InlineData("1")]
        public void FormattedSearchRef_GivenSearchRefWithUnknownNoteType_ShouldNotReturnFormattedReference(string noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{noteType}1"
            };

            //act
            var result = filter.SearchRef;

            //assert
            result.Should().Be($"{noteType}1");
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedSearchRef_GivenSearchRefWithWhiteSpaceAndNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $" {noteType.DisplayName}1 "
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be("1");
        }

        [Fact]
        public void FormattedSearchRef_GivenSearchRefWithNoNoteType_ShouldReturnFormattedReference()
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = "1"
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void NoteType_GivenEmptySearchRef_NullShouldBeReturned(string searchRef)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = searchRef
            };

            //act
            var result = filter.NoteType;

            //assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("X1234")]
        [InlineData("AB1234")]
        [InlineData(" ")]
        [InlineData("1234")]
        public void NoteType_GivenSearchRefWithInvalidNoteType_NullShouldBeReturned(string type)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{type}1"
            };

            //act
            var result = filter.NoteType;

            //assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("E")]
        [InlineData("e")]
        public void NoteType_GivenSearchRefWithEvidenceNoteType_NoteTypeValueShouldBeReturned(string type)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{type}1"
            };

            //act
            var result = filter.NoteType;

            //assert
            result.Should().Be(NoteType.EvidenceNote.Value);
        }

        [Theory]
        [InlineData("T")]
        [InlineData("t")]
        public void NoteType_GivenSearchRefWithTransferNoteType_NoteTypeValueShouldBeReturned(string type)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{type}1"
            };

            //act
            var result = filter.NoteType;

            //assert
            result.Should().Be(NoteType.TransferNote.Value);
        }
    }
}
