namespace EA.Weee.Domain.Tests.Unit.Producer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Weee.Domain.Producer;
    using Xunit;

    public class SICCodeTests
    {
        [Fact]
        public void SICCode_EqualsNullParameter_ReturnsFalse()
        {
            var sicCode = SICCodeBuilder.NewSICCode;

            Assert.NotEqual(sicCode, null);
        }

        [Fact]
        public void SICCode_EqualsObjectParameter_ReturnsFalse()
        {
            var sicCode = SICCodeBuilder.NewSICCode;

            Assert.NotEqual(sicCode, new object());
        }

        [Fact]
        public void SICCode_EqualsSameInstance_ReturnsTrue()
        {
            var sicCode = SICCodeBuilder.NewSICCode;

            Assert.Equal(sicCode, sicCode);
        }

        [Fact]
        public void SICCode_EqualsSICCodeSameDetails_ReturnsTrue()
        {
            var sicCode = SICCodeBuilder.NewSICCode;
            var sicCode2 = SICCodeBuilder.NewSICCode;

            Assert.Equal(sicCode, sicCode2);
        }

        [Fact]
        public void SICCode_EqualsSICCodeDifferentName_ReturnsFalse()
        {
            var sicCode = SICCodeBuilder.NewSICCode;
            var sicCode2 = SICCodeBuilder.WithName("test name");

            Assert.NotEqual(sicCode, sicCode2);
        }

        private class SICCodeBuilder
        {
            private string name = "name";

            private SICCodeBuilder()
            {
            }

            private SICCode Build()
            {
                return new SICCode(name);
            }

            public static SICCode NewSICCode
            {
                get { return new SICCodeBuilder().Build(); }
            }

            public static SICCode WithName(string name)
            {
                var builder = new SICCodeBuilder();
                builder.name = name;

                return builder.Build();
            }
        }
    }
}
