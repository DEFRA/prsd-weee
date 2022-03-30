namespace EA.Weee.Domain.Tests.Unit.AatfEvidence
{
    using System;
    using Evidence;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Xunit;

    public class ProtocolEnumTests
    {
        [Fact]
        public void Protocol_ShouldHaveValues()
        {
            var values = EnumHelper.GetValues(typeof(Protocol));
            values.Count.Should().Be(5);

            values.Should().Contain(c => c.Key.Equals(1) && c.Value.Equals("Actual"));
            values.Should().Contain(c => c.Key.Equals(2) && c.Value.Equals("LdaProtocol"));
            values.Should().Contain(c => c.Key.Equals(3) && c.Value.Equals("SmwProtocol"));
            values.Should().Contain(c => c.Key.Equals(4) && c.Value.Equals("SiteSpecificProtocol"));
            values.Should().Contain(c => c.Key.Equals(5) && c.Value.Equals("ReuseNetworkPwp"));
        }
    }
}
