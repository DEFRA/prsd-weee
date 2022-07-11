namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes.Requests
{
    using System;
    using EA.Weee.Requests.Admin;
    using FluentAssertions;
    using Xunit;

    public class GetEvidenceNoteForInternalUserRequestTests
    {
        [Fact]
        public void GetEvidenceNoteForInternalUserRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetEvidenceNoteForInternalUserRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetEvidenceNoteForInternalUserRequest_Constructor_GivenEmptyEvidenceNote()
        {
            var result = Record.Exception(() => new GetEvidenceNoteForInternalUserRequest(Guid.Empty));

            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNoteForInternalUserRequest_GivenEvidenceNoteId_PropertiesShouldBeSet()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            var result = new GetEvidenceNoteForInternalUserRequest(id);

            //assert
            result.EvidenceNoteId.Should().Be(id);
        }
    }
}
