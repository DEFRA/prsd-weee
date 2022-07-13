namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using System;
    using System.Linq;
    using Weee.Tests.Core;
    using Xunit;

    public class ListOfNotesMapTests : SimpleUnitTestBase
    {
        public ListOfNotesMapTests()
        {
        }

        [Fact]
        public void Constructor_GivenListIsNull_ArgumentNullExceptionExpected()
        {
            // act
            Action action = () => new ListOfNotesMap(null, TestFixture.Create<bool>());

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_GivenListIsNotNull_ShouldContainTheListOfNotes()
        {
            // arrange
            var notes = TestFixture.CreateMany<Note>().ToList();
            var includeTonnage = TestFixture.Create<bool>();
            // act
            var result = new ListOfNotesMap(notes, includeTonnage);

            // assert
            result.ListOfNotes.Should().BeEquivalentTo(notes);
            result.IncludeTonnage.Should().Be(includeTonnage);
        }
    }
}
