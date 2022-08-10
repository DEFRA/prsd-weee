namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using AutoFixture;
    using System;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class SetNoteStatusRequestTests : SimpleUnitTestBase
    {
        [Fact]
        public void SetNoteStatusRequest_ShouldDeriveFromSetNoteStatusBase()
        {
            typeof(SetNoteStatusRequest).Should().BeDerivedFrom<SetNoteStatusBase>();
        }

        [Fact]
        public void SetNoteStatusRequest_GivenValues_PropertiesShouldBeSet()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var reason = TestFixture.Create<string>();
            var status = TestFixture.Create<NoteStatus>();

            //act
            var request = new SetNoteStatusRequest(id, status, reason);

            //assert
            request.NoteId.Should().Be(id);
            request.Reason.Should().Be(reason);
            request.Status.Should().Be(status);
        }

        [Fact]
        public void SetNoteStatusRequest_GivenValuesWithNoReason_PropertiesShouldBeSet()
        {
            //arrange
            var id = TestFixture.Create<Guid>();
            var status = TestFixture.Create<NoteStatus>();

            //act
            var request = new SetNoteStatusRequest(id, status);

            //assert
            request.NoteId.Should().Be(id);
            request.Reason.Should().BeNull();
            request.Status.Should().Be(status);
        }
    }
}