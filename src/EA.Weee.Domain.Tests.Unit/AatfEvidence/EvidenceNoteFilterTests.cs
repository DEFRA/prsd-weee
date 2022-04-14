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
        public void SearchRef_GivenSearchRefWithNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{noteType.DisplayName}1"
            };

            //act
            var result = filter.SearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void SearchRef_GivenSearchRefWithNoteTypeAndIsLowerCase_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $"{noteType.DisplayName.ToLower()}1"
            };

            //act
            var result = filter.SearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [InlineData("X")]
        [InlineData("Y")]
        [InlineData("B")]
        [InlineData("*")]
        [InlineData("1")]
        public void SearchRef_GivenSearchRefWithUnknownNoteType_ShouldNotReturnFormattedReference(string noteType)
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
        public void SearchRef_GivenSearchRefWithWhiteSpaceAndNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = $" {noteType.DisplayName}1 "
            };

            //act
            var result = filter.SearchRef;

            //assert
            result.Should().Be("1");
        }

        [Fact]
        public void SearchRef_GivenSearchRefWithNoNoteType_ShouldReturnFormattedReference()
        {
            //arrange
            var filter = new EvidenceNoteFilter()
            {
                SearchRef = "1"
            };

            //act
            var result = filter.SearchRef;

            //assert
            result.Should().Be("1");
        }
    }
}
