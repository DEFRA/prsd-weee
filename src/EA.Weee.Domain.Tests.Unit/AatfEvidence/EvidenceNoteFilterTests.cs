﻿namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
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
        [InlineData("EE1")]
        [InlineData("TT1")]
        [InlineData("ET1")]
        [InlineData("Ee1")]
        [InlineData("eE1")]
        [InlineData("ee1")]
        [InlineData("TE1")]
        [InlineData("Tt1")]
        [InlineData("tt1")]
        [InlineData("tT1")]
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
    }
}
