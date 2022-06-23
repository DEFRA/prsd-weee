namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using AutoFixture;
    using Evidence;
    using FluentAssertions;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Xunit;

    public class NoteStatusHistoryTests : SimpleUnitTestBase
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void NoteStatusHistory_GivenEmptyChangedBy_ArgumentExceptionExpected(string changedBy)
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(changedBy, NoteStatus.Approved, NoteStatus.Submitted, SystemTime.UtcNow));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullChangedBy_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(null, NoteStatus.Approved, NoteStatus.Submitted, SystemTime.UtcNow));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullFromStatus_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(TestFixture.Create<string>(), null, NoteStatus.Submitted, SystemTime.UtcNow));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenNullToStatus_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() =>
                new NoteStatusHistory(TestFixture.Create<string>(), NoteStatus.Approved, null, SystemTime.UtcNow));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void NoteStatusHistory_GivenValues_NoteStatusHistoryPropertiesShouldBeSet()
        {
            //act
            var changedBy = TestFixture.Create<string>();
            var fromStatus = NoteStatus.Submitted;
            var toStatus = NoteStatus.Approved;
            var date = TestFixture.Create<DateTime>();

            //arrange
            var result = new NoteStatusHistory(changedBy, fromStatus, toStatus, date);

            //assert
            result.ChangedById.Should().Be(changedBy);
            result.FromStatus.Should().Be(fromStatus);
            result.ToStatus.Should().Be(toStatus);
            result.ChangedDate.Should().Be(date);
        }

        [Fact]
        public void NoteStatusHistory_GivenValuesWithReason_NoteStatusHistoryPropertiesShouldBeSet()
        {
            //act
            var changedBy = TestFixture.Create<string>();
            var fromStatus = NoteStatus.Submitted;
            var toStatus = NoteStatus.Approved;
            var reason = TestFixture.Create<string>();
            var date = TestFixture.Create<DateTime>();

            //arrange
            var result = new NoteStatusHistory(changedBy, fromStatus, toStatus, date, reason);

            //assert
            result.ChangedById.Should().Be(changedBy);
            result.FromStatus.Should().Be(fromStatus);
            result.ToStatus.Should().Be(toStatus);
            result.Reason.Should().Be(reason);
            result.ChangedDate.Should().Be(date);
        }
    }
}
