namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using System.Collections.Generic;
    using Core.Helpers;
    using FluentAssertions;
    using Xunit;

    public class ExtensionsTests
    {
        [Theory]
        [InlineData(0, "0.00")]
        [InlineData(3.3454, "3.35")]
        [InlineData(3.4, "3.40")]
        [InlineData(354.0, "354")]
        [InlineData(1051, "1050")]
        [InlineData(1055, "1060")]
        public void DoubleTo3SignificantFigures_ReturnsExpectedString(double input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToStringWithXSignificantFigures(3));
        }

        [Fact]
        public void DomainEnumerationToCoreEnumeration_ReturnsExpectedValues()
        {
            Assert.Equal(CoreEnumeration.Something, DomainEnumeration.Something.ToCoreEnumeration<CoreEnumeration>());
            Assert.Equal(CoreEnumeration.SomethingElse, DomainEnumeration.SomethingElse.ToCoreEnumeration<CoreEnumeration>());
        }

        [Fact]
        public void DomainEnumerationToAnInvalidType_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => DomainEnumeration.Something.ToCoreEnumeration<string>()); // String is not an enum
        }

        [Fact]
        public void CoreEnumerationToDomainEnumeration_ReturnsExpectedValues()
        {
            Assert.Equal(DomainEnumeration.Something, CoreEnumeration.Something.ToDomainEnumeration<DomainEnumeration>());
        }

        [Fact]
        public void MakeEmptyStringsNull_ObjectContainsEmptyStringInRoot_ConvertsToNull()
        {
            var simpleObject = new SimpleObject { MyString = string.Empty };

            var result = simpleObject.MakeEmptyStringsNull();

            Assert.Null(result.MyString);
        }

        [Fact]
        public void MakeEmptyStringsNull_ObjectContainsNonEmptyStringInRoot_DoesNotChangeXml()
        {
            var myString = "something";
            var simpleObject = new SimpleObject { MyString = myString };

            var result = simpleObject.MakeEmptyStringsNull();

            Assert.Equal(myString, result.MyString);
        }

        [Fact]
        public void MakeEmptyStringsNull_ObjectContainsEmptyStringInNestedObject_ConvertsToNull()
        {
            var complexObject = new ComplexObject
            {
                InnerObject = new SimpleObject
                {
                    MyString = string.Empty
                }
            };

            var result = complexObject.MakeEmptyStringsNull();

            Assert.Null(result.InnerObject.MyString);
        }

        [Fact]
        public void MakeEmptyStringsNull_ObjectContainsNullNestedObject_DoesNotThrowException()
        {
            var complexObject = new ComplexObject
            {
                InnerObject = null
            };

            var exception = Record.Exception(() => complexObject.MakeEmptyStringsNull());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(typeof(string))]
        [InlineData(typeof(int))]
        [InlineData(typeof(decimal))]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        [InlineData(typeof(byte))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(char))]
        public void PrimitiveTypesAreNotCustom(Type type)
        {
            Assert.False(type.IsCustom());
        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { 0.0M },
                new object[] { 0.00M },
                new object[] { 0.000M },
                new object[] { null }
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void ToTonnageDisplay_GivenNullOrZeroValue_DisplayShouldBeCorrect(decimal? value)
        {
            value.ToTonnageDisplay().Should().Be("0.000");
        }

        [Fact]
        public void ToTonnageDisplay_GivenValueWithNoDecimalPlace_DisplayShouldBeCorrect()
        {
            const decimal value = 1;
            value.ToTonnageDisplay().Should().Be("1.000");
        }

        [Fact]
        public void ToTonnageDisplay_GivenValueWithSingleDecimalPlace_DisplayShouldBeCorrect()
        {
            const decimal value = 1.1m;
            value.ToTonnageDisplay().Should().Be("1.100");
        }

        [Fact]
        public void ToTonnageDisplay_GivenValueWithTwoDecimalPlace_DisplayShouldBeCorrect()
        {
            const decimal value = 1.11m;
            value.ToTonnageDisplay().Should().Be("1.110");
        }

        [Fact]
        public void ToTonnageDisplay_GivenValueWithThreeDecimalPlace_DisplayShouldBeCorrect()
        {
            const decimal value = 1.111m;
            value.ToTonnageDisplay().Should().Be("1.111");
        }

        [Fact]
        public void ToTonnageDisplay_GivenThousandsValue_DisplayValueShouldBeFormattedWithCommas()
        {
            const decimal value = 1000m;

            value.ToTonnageDisplay().Should().Be("1,000.000");
        }

        [Fact]
        public void ToTonnageDisplay_GivenMultipleThousandsValue_DisplayValueShouldBeFormattedWithCommas()
        {
            const decimal value = 10000m;

            value.ToTonnageDisplay().Should().Be("10,000.000");
        }

        [Fact]
        public void ToTonnageEditDisplay_GivenValueIsNull_EmptyStringShouldBeReturned()
        {
            var value = (decimal?)null;

            value.ToTonnageEditDisplay().Should().Be(string.Empty);
        }

        [Fact]
        public void ToEditTonnageDisplay_GivenThousandsValue_DisplayValueShouldNotBeFormattedWithCommas()
        {
            const decimal value = 1000m;

            value.ToTonnageEditDisplay().Should().Be("1000.000");
        }

        [Fact]
        public void ToEditTonnageDisplay_GivenMultipleThousandsValue_DisplayValueShouldNotBeFormattedWithCommas()
        {
            const decimal value = 10000m;

            value.ToTonnageEditDisplay().Should().Be("10000.000");
        }

        [Fact]
        public void GetPropertyValue_GivenPropertyName_ShouldReturnValue()
        {
            var obj = new { One = 1, None = (int?)null };
            obj.GetPropertyValue<int>("One").Should().Be(1);
        }

        [Fact]
        public void GetPropertyValue_GivenNameOfNullPropertyValue_ShouldReturnNullValue()
        {
            var obj = new { One = 1, None = (int?)null };
            obj.GetPropertyValue<int?>("None").Should().Be(null);
        }

        [Fact]
        public void GetPropertyValue_GivenIncorrectType_ShouldReturnNullValue()
        {
            var obj = new { One = 1, None = (int?)null };

            var exception = Record.Exception(() => obj.GetPropertyValue<string>("One"));

            exception.Should().BeOfType<InvalidCastException>();
        }

        [Fact]
        public void GetPropertyValue_GivenIncorrectProperyName_ShouldThrowAnException()
        {
            var incorrectName = "Uno";
            var obj = new { One = 1, None = (int?)null };

            var exception = Record.Exception(() => obj.GetPropertyValue<int>(incorrectName));

            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Contain(incorrectName);
        }
    }
}
