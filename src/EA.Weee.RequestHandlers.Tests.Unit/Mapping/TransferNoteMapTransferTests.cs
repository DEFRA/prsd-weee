namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using AutoFixture;
    using Domain.Evidence;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using System;
    using Weee.Tests.Core;
    using Xunit;

    public class TransferNoteMapTransferTests : SimpleUnitTestBase
    {
        [Fact]
        public void TransferNoteMapTransfer_GivenNullNote_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new TransferNoteMapTransfer(null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void TransferNoteMapTransfer_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var note = TestFixture.Create<Note>();

            //act
            var result = new TransferNoteMapTransfer(note);

            //assert
            result.Note.Should().Be(note);
        }
    }
}
