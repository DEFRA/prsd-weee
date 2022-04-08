namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using System;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetEvidenceNoteRequestTests
    {
        [Fact]
        public void GetEvidenceNoteRequest_ShouldHaveSerializableAttribute()
        {
            typeof(GetEvidenceNoteRequest).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void GetEvidenceNoteRequest_Constructor_GivenEmptyEvidenceNote()
        {
            var result = Record.Exception(() => new GetEvidenceNoteRequest(Guid.Empty));

                result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNoteRequest_GivenEvidenceNoteId_PropertiesShouldBeSet()
        {
            //arrange
            var id = Guid.NewGuid();

            //act
            var result = new GetEvidenceNoteRequest(id);

            //assert
            result.EvidenceNoteId.Should().Be(id);
        }
    }
}