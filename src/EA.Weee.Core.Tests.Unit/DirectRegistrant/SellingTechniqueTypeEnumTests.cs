namespace EA.Weee.Core.DirectRegistrant
{
    using System;
    using FluentAssertions;
    using Prsd.Core.Helpers;
    using Xunit;

    public class SellingTechniqueTypeEnumTests
    {
        [Fact]
        public void SellingTechniqueType_ShouldHaveSerializableAttribute()
        {
            typeof(SellingTechniqueType).Should().BeDecoratedWith<SerializableAttribute>();
        }

        [Fact]
        public void SellingTechniqueType_ShouldHaveValues()
        {
            var values = EnumHelper.GetValues(typeof(SellingTechniqueType));
            values.Count.Should().Be(3);

            values.Should().Contain(c => c.Key.Equals(1) && c.Value.Equals("Direct selling to end user (mail, order, internet etc)"));
            values.Should().Contain(c => c.Key.Equals(2) && c.Value.Equals("Indirect selling (other)"));
            values.Should().Contain(c => c.Key.Equals(3) && c.Value.Equals("Direct selling to end user (mail, order, internet etc) and Indirect selling (other)"));
        }
    }
}
