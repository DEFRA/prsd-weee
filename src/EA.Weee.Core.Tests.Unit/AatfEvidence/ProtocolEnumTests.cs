namespace EA.Weee.Core.Tests.Unit.AatfEvidence
{
    using System;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Xunit;

    public class ProtocolEnumTests
    {
        [Fact]
        public void Protocol_ShouldHaveSerializableAttribute()
        {
            typeof(Protocol).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void Protocol_ShouldHaveValues()
        {
            var values = EnumHelper.GetValues(typeof(Protocol));
            values.Count.Should().Be(6);

            values.Should().Contain(c => c.Key.Equals(1) && c.Value.Equals("Actual"));
            values.Should().Contain(c => c.Key.Equals(2) && c.Value.Equals("LDA protocol"));
            values.Should().Contain(c => c.Key.Equals(3) && c.Value.Equals("SMW protocol"));
            values.Should().Contain(c => c.Key.Equals(4) && c.Value.Equals("Site specific protocol"));
            values.Should().Contain(c => c.Key.Equals(5) && c.Value.Equals("Reuse network PWP"));
            values.Should().Contain(c => c.Key.Equals(6) && c.Value.Equals("Light iron protocol"));
        }
    }
}
