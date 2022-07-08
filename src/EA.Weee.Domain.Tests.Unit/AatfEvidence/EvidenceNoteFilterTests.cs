namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Tests.Unit.Helpers;
    using Evidence;
    using FluentAssertions;
    using Xunit;

    public class EvidenceNoteFilterTests
    {
        [Fact]
        public void EvidenceNoteFilter_GivenConstructorArguments_PropertiesShouldBeSet()
        {
            //arrange
            var complianceYear = DateTime.Now.Year;

            //act
            var filter = new NoteFilter(complianceYear, int.MaxValue, 0);

            //assert
            filter.ComplianceYear.Should().Be(complianceYear);
            filter.PageNumber.Should().Be(0);
            filter.PageSize.Should().Be(int.MaxValue);
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedSearchRef_GivenSearchRefWithNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
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
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
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
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
            {
                SearchRef = $"{noteType}1"
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be($"{noteType}1");
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedSearchRef_GivenSearchRefWithWhiteSpaceAndNoteType_ShouldReturnFormattedReference(NoteType noteType)
        {
            //arrange
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
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
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
            {
                SearchRef = "1"
            };

            //act
            var result = filter.FormattedSearchRef;

            //assert
            result.Should().Be("1");
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData(null)]
        public void FormattedNoteType_GivenEmptySearchRef_ShouldReturnMinusOne(string searchRef)
        {
            //arrange
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
            {
                SearchRef = searchRef
            };

            //act
            var result = filter.FormattedNoteType;

            //assert
            result.Should().Be(-1);
        }

        [Theory]
        [ClassData(typeof(NoteTypeData))]
        public void FormattedNoteType_GivenSearchRefWithNoteType_ShouldReturnNoteTypeValue(NoteType noteType)
        {
            //arrange
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
            {
                SearchRef = $"{noteType.DisplayName}1"
            };

            //act
            var result = filter.FormattedNoteType;

            //assert
            result.Should().Be(noteType.Value);
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
        public void FormattedNoteType_GivenSearchRefWithUnknownNoteType_ShouldReturnZero(string noteType)
        {
            //arrange
            var filter = new NoteFilter(DateTime.Now.Year, int.MaxValue, 0)
            {
                SearchRef = $"{noteType}1"
            };

            //act
            var result = filter.FormattedNoteType;

            //assert
            result.Should().Be(0);
        }
    }
}
