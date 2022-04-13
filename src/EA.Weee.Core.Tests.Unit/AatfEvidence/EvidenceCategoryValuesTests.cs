namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using Aatf;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Xunit;

    public class EvidenceCategoryValuesTests
    {
        [Fact]
        public void EvidenceCategoryValues_ShouldInheritFromCategoryValues()
        {
            typeof(EvidenceCategoryValues).Should().BeDerivedFrom<CategoryValues<EvidenceCategoryValue>>();
        }
    }
}
