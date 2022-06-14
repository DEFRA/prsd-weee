namespace EA.Weee.Domain.Tests.Unit.Admin
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Lookup;
    using Obligation;
    using Xunit;

    public class ObligationSchemeAmountTests
    {
        public static IEnumerable<object[]> ChangedData =>
            new List<object[]>
            {
                new object[] { 1M, null },
                new object[] { null, 1M },
                new object[] { 1.1M, null },
                new object[] { null, 1.1M },
                new object[] { 1.1M, 1.2M },
                new object[] { 1.2M, 1.1M }
            };

        [Theory]
        [MemberData(nameof(ChangedData))]
        public void UpdateObligation_GivenChangedAmounts_TrueShouldBeReturned(decimal? existing, decimal? updated)
        {
            //arrange
            var obligationSchemeAmount = new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, existing);

            //act
            var result = obligationSchemeAmount.UpdateObligation(updated);

            //assert
            result.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ChangedData))]
        public void UpdateObligation_GivenChangedAmounts_AmountShouldBeUpdated(decimal? existing, decimal? updated)
        {
            //arrange
            var obligationSchemeAmount = new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, existing);

            //act
            obligationSchemeAmount.UpdateObligation(updated);
            var result = obligationSchemeAmount.Obligation;

            //assert
            result.Should().Be(updated);
        }

        public static IEnumerable<object[]> NonChangedData =>
            new List<object[]>
            {
                new object[] { 1M, 1M },
                new object[] { null, null },
                new object[] { 1.1M, 1.10M },
                new object[] { 1.10M, 1.1M }
            };

        [Theory]
        [MemberData(nameof(NonChangedData))]
        public void UpdateObligation_GivenNotChangedAmounts_TrueShouldBeReturned(decimal? existing, decimal? updated)
        {
            //arrange
            var obligationSchemeAmount = new ObligationSchemeAmount(WeeeCategory.ITAndTelecommsEquipment, existing);

            //act
            var result = obligationSchemeAmount.UpdateObligation(updated);

            //assert
            result.Should().BeFalse();
        }
    }
}
