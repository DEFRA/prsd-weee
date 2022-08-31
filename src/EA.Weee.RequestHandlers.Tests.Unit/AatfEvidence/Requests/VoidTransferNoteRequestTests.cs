namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using Core.AatfEvidence;
    using EA.Weee.Requests.Shared;
    using FluentAssertions;
    using System;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class VoidTransferNoteRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void VoidTransferNoteRequest_ShouldDeriveFromSetNoteStatusBase()
        {
            typeof(VoidNoteRequest).Should().BeDerivedFrom<SetNoteStatusBase>();
        }

        [Fact]
        public void VoidTransferNoteRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var reason = TestFixture.Create<string>();

            //act
            var request = new VoidNoteRequest(id, reason);

            //assert
            request.NoteId.Should().Be(id);
            request.Reason.Should().Be(reason);
            request.Status.Should().Be(NoteStatus.Void);
        }

        [Fact]
        public void VoidTransferNoteRequest_GivenValuesWithNoReason_PropertiesShouldBeSet()
        {
            //arrange
            var id = TestFixture.Create<Guid>();

            //act
            var request = new VoidNoteRequest(id);

            //assert
            request.NoteId.Should().Be(id);
            request.Reason.Should().BeNull();
            request.Status.Should().Be(NoteStatus.Void);
        }
    }
}
