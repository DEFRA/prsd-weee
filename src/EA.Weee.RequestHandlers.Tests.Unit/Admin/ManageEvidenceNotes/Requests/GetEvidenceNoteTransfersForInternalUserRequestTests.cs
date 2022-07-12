namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using Xunit;

    public class GetEvidenceNoteTransfersForInternalUserRequestTests
    {
        [Fact]
        public void GetEvidenceNoteTransfersForInternalUserRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetEvidenceNoteTransfersForInternalUserRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetEvidenceNoteTransfersForInternalUserRequest_Constructor_GivenEmptyEvidenceNote()
        {
            var result = Record.Exception(() => new GetEvidenceNoteTransfersForInternalUserRequest(Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNoteTransfersForInternalUserRequest_GivenEvidenceNoteId_PropertiesShouldBeSet()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            var result = new GetEvidenceNoteTransfersForInternalUserRequest(id);

            //assert
            result.EvidenceNoteId.Should().Be(id);
        }
    }
}
