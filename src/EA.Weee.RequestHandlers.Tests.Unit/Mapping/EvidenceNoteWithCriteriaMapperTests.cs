namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using FluentAssertions;
    using Mappings;
    using Xunit;

    internal class EvidenceNoteWithCriteriaMapperTests
    {
        [Fact]
        public void EvidenceNoteCriteriaMapper_ShouldDeriveFrom_EvidenceNoteWithCriteriaMapperBase()
        {
            typeof(EvidenceNoteWithCriteriaMapper).Should().BeDerivedFrom<EvidenceNoteWitheCriteriaMapperBase>();
        }
    }
}
