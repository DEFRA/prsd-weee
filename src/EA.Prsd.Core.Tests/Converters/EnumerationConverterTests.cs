namespace EA.Prsd.Core.Tests.Converters
{
    using System.Text;
    using Core.Domain;
    using Core.Web.Converters;
    using Newtonsoft.Json;
    using Xunit;

    public class EnumerationConverterTests
    {
        private readonly EnumerationConverter converter = new EnumerationConverter();

        [Fact]
        public void CanRead_NonEnumerationType_ReturnsFalse()
        {
            var result = converter.CanConvert(typeof(StringBuilder));

            Assert.False(result);
        }

        [Fact]
        public void CanRead_EnumerationType_ReturnsTrue()
        {
            var result = converter.CanConvert(typeof(MyEnumeration));

            Assert.True(result);
        }

        [Fact]
        public void CanWrite_ReturnsFalse()
        {
            Assert.False(converter.CanWrite);
        }

        [Fact]
        public void CanRead_ReturnsTrue()
        {
            Assert.True(converter.CanRead);
        }

        [Fact]
        public void Converter_WithEnumeration_ReturnsCorrectObject()
        {
            var enumeration = MyEnumeration.Honda;

            string serializedData = JsonConvert.SerializeObject(enumeration);

            var returnedEnumeration = JsonConvert.DeserializeObject(serializedData, typeof(MyEnumeration), converter) as MyEnumeration;

            Assert.NotNull(returnedEnumeration);
            Assert.Equal(enumeration.Value, returnedEnumeration.Value);
            Assert.Equal(enumeration.DisplayName, returnedEnumeration.DisplayName);
        }

        [Fact]
        public void Converter_WithComplexEnumeration_ReturnsCorrectObject()
        {
            var enumeration = YourEnumeration.Audi;

            string serializedData = JsonConvert.SerializeObject(enumeration);

            var returnedEnumeration = JsonConvert.DeserializeObject(serializedData, typeof(YourEnumeration), converter) as YourEnumeration;

            Assert.NotNull(returnedEnumeration);
            Assert.Equal(enumeration.Value, returnedEnumeration.Value);
            Assert.Equal(enumeration.DisplayName, returnedEnumeration.DisplayName);
            Assert.Equal(enumeration.Make, returnedEnumeration.Make);
        }

        private class YourEnumeration : Enumeration
        {
            public string Make;
            public static YourEnumeration Bmw = new YourEnumeration(1, "BMW", "Fast Car");
            public static YourEnumeration Audi = new YourEnumeration(2, "Audi", "A9");

            protected YourEnumeration()
            {
            }

            public YourEnumeration(int value, string displayName, string make)
                : base(value, displayName)
            {
                this.Make = make;
            } 
        }

        private class MyEnumeration : Enumeration
        {
            public static MyEnumeration Toyota = new MyEnumeration(1, "Toyota");
            public static MyEnumeration Honda = new MyEnumeration(2, "Honda");

            protected MyEnumeration()
            {
            }

            public MyEnumeration(int value, string displayName)
                : base(value, displayName)
            {
            }
        }
    }
}
