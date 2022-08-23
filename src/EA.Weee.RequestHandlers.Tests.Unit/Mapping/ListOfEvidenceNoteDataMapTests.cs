namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Core.AatfEvidence;
    using System.Collections.Generic;
    using EA.Weee.RequestHandlers.Mappings;
    using FluentAssertions;
    using Xunit;

    public class ListOfEvidenceNoteDataMapTests
    {
        public ListOfEvidenceNoteDataMapTests()
        {
        }

        [Fact]
        public void Constructor_ShouldCreateAnEmptyListOfEvidenceNoteData()
        {
            // arrange
            var evidences = new List<EvidenceNoteData>();

            // act
            var result = new ListOfEvidenceNoteDataMapperObject();

            // assert
            result.ListOfEvidenceNoteData.Should().BeEquivalentTo(evidences);
        }
    }
}
