namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfEvidence;
    using DataReturns;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Xunit;

    public class WasteTypeEnumTests
    {
        [Fact]
        public void WasteType_ShouldHaveSerializableAttribute()
        {
            typeof(WasteType).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void WasteType_ShouldHaveValues()
        {
            var values = EnumHelper.GetValues(typeof(WasteType));
            values.Count.Should().Be(2);

            values.Should().Contain(c => c.Key.Equals(1) && c.Value.Equals("Household"));
            values.Should().Contain(c => c.Key.Equals(2) && c.Value.Equals("Non-household"));
        }
    }
}