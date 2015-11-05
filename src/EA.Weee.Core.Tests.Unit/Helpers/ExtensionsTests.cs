﻿namespace EA.Weee.Core.Tests.Unit.Helpers
{
    using System;
    using Core.Helpers;
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
    }
}
