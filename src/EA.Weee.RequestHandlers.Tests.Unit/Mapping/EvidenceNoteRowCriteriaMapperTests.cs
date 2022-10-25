namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using FluentAssertions;
    using Mappings;
    using Xunit;

    public class EvidenceNoteRowCriteriaMapperTests
    {
        [Fact]
        public void EvidenceNoteRowCriteriaMapper_ShouldDeriveFrom_EvidenceNoteWithCriteriaMapperBase()
        {
            typeof(EvidenceNoteRowCriteriaMapper).Should().BeDerivedFrom<EvidenceNoteWitheCriteriaMapperBase>();
        }
    }
}
