namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using AutoFixture;
    using Evidence;
    using FluentAssertions;
    using Xunit;

    public class NoteStatusHistoryTests
    {
        private readonly Fixture fixture;

        public NoteStatusHistoryTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void NoteStatusHistory_GivenEmptyChangedBy_ArgumentExceptionExpected(string changedBy)
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(changedBy, NoteStatus.Approved, NoteStatus.Submitted));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullChangedBy_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(null, NoteStatus.Approved, NoteStatus.Submitted));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullFromStatus_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(fixture.Create<string>(), null, NoteStatus.Submitted));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullToStatus_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(fixture.Create<string>(), NoteStatus.Approved, null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenValues_NoteStatusHistoryPropertiesShouldBeSet()
        {
            //act
            var changedBy = fixture.Create<string>();
            var fromStatus = NoteStatus.Submitted;
            var toStatus = NoteStatus.Approved;

            //arrange
            var result = new NoteStatusHistory(changedBy, fromStatus, toStatus);

            //assert
            result.ChangedById.Should().Be(changedBy);
            result.FromStatus.Should().Be(fromStatus);
            result.ToStatus.Should().Be(toStatus);
        }

        [Fact]
        public void NoteStatusHistory_GivenValuesWithReason_NoteStatusHistoryPropertiesShouldBeSet()
        {
            //act
            var changedBy = fixture.Create<string>();
            var fromStatus = NoteStatus.Submitted;
            var toStatus = NoteStatus.Approved;
            var reason = fixture.Create<string>();

            //arrange
            var result = new NoteStatusHistory(changedBy, fromStatus, toStatus, reason);

            //assert
            result.ChangedById.Should().Be(changedBy);
            result.FromStatus.Should().Be(fromStatus);
            result.ToStatus.Should().Be(toStatus);
            result.Reason.Should().Be(reason);
        }
    }
}
