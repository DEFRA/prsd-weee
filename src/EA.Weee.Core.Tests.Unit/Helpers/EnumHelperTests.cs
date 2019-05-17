namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using AutoFixture;
    using Core.Helpers;
    using Xunit;

    public class EnumHelperTests
    {
        private readonly Fixture fixture;

        public EnumHelperTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void GetEnumValueFromDisplayString_HasDisplayAttributeWhichIsProvided_ReturnsExpectedValues()
        {
            Assert.Equal(CoreEnumeration.Something, EnumHelper.GetEnumValueFromDisplayString<CoreEnumeration>("Something"));
            Assert.Equal(CoreEnumeration.SomethingElse, EnumHelper.GetEnumValueFromDisplayString<CoreEnumeration>("Something Else"));
        }

        [Fact]
        public void GetEnumValueFromDisplayString_NoDisplayAttributeFieldNameProvided_ReturnsExpectedValues()
        {
            Assert.Equal(CoreEnumeration.SomethingDifferent, EnumHelper.GetEnumValueFromDisplayString<CoreEnumeration>("SomethingDifferent"));
        }

        [Fact]
        public void GetEnumValueFromDisplayString_HasDisplayAttributeFieldNameProvided_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumHelper.GetEnumValueFromDisplayString<CoreEnumeration>("SomethingElse"));
        }

        [Fact]
        public void GetEnumValueFromDisplayString_NotFound_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => EnumHelper.GetEnumValueFromDisplayString<CoreEnumeration>(fixture.Create<string>()));
        }

        [Fact]
        public void GetEnumValueFromDisplayString_NotAnEnum_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => EnumHelper.GetEnumValueFromDisplayString<string>(fixture.Create<string>()));
        }
    }
}
