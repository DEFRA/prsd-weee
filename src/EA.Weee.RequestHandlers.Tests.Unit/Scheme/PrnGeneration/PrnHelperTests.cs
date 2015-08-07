namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.PrnGeneration
{
    using Core.Helpers.PrnGeneration;
    using Xunit;

    public class PrnGenerationTests
    {
        [Fact]
        public void PrnFromSeed_PassedZero_RecomputedSeedValueEqualsZero()
        {
            const uint Value = 0;

            PrnAsComponents prnAsComponents = new PrnAsComponents(Value);

            uint seedFromPrn = prnAsComponents.ToSeedValue();

            Assert.Equal(Value, seedFromPrn);
        }

        [Fact]
        public void PrnFromSeed_PassedValueOutOfRange_RecomputedSeedValueIsInRange()
        {
            const uint Value = 9967;
            const uint NextValueInRange = 65536;

            PrnAsComponents prnAsComponents = new PrnAsComponents(Value);

            uint seedFromPrn = prnAsComponents.ToSeedValue();

            Assert.Equal(NextValueInRange, seedFromPrn);
        }
    }
}
