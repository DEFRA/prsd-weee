namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using Evidence;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Xunit;

    public class WasteTypeEnumTests
    {
        [Fact]
        public void WasteType_ShouldHaveValues()
        {
            var values = EnumHelper.GetValues(typeof(WasteType));
            values.Count.Should().Be(2);

            values.Should().Contain(c => c.Key.Equals(1) && c.Value.Equals("HouseHold"));
            values.Should().Contain(c => c.Key.Equals(2) && c.Value.Equals("NonHouseHold"));
        }
    }
}