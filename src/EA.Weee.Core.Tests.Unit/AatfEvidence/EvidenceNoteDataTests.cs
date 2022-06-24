namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using Core.AatfEvidence;
    using FluentAssertions;
    using Xunit;

    public class EvidenceNoteDataTests
    {
        [Fact]
        public void EvidenceNoteData_ShouldBeDerivedFromEvidenceNoteDataBase()
        {
            typeof(EvidenceNoteData).Should().BeDerivedFrom<EvidenceNoteDataBase>();
        }
    }
}
