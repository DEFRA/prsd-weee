namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using System;
    using System.Linq;
    using Xunit;

    public class ListOfNotesMapTests
    {
        private readonly Fixture fixture;

        public ListOfNotesMapTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenListIsNull_ArgumentNullExceptionExpected()
        {
            // act
            Action action = () => new ListOfNotesMap(null);

            // assert
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenListIsNotNull_ShouldMapTheListOfNotes()
        {
            // arrange
            var notes = fixture.CreateMany<Note>().ToList();

            // act
            var result = new ListOfNotesMap(notes);

            // assert
            result.ListOfNotes.Should().BeEquivalentTo(notes);
        }
    }
}
