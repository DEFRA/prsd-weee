namespace EA.Weee.Core.Tests.Unit.Shared
{
    using EA.Weee.Core.Shared;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class CsvWriterTests
    {
        [Fact]
        public void Encode_SimpleValue_DoesntNeedEncoding()
        {
            // Arrage
            string rawValue = "Hello World";

            // Act
            string encodedValue = CsvWriter<object>.Encode(rawValue);

            // Assert
            Assert.Equal(rawValue, encodedValue);
        }

        [Fact]
        public void Encode_ValueContainingComma_SurroundedInDoubleQuotes()
        {
            // Arrage
            string doubleQuotes = "\"";
            string rawValue = "Hello, World";

            // Act
            string encodedValue = CsvWriter<object>.Encode(rawValue);

            // Assert
            string expectedValue = (doubleQuotes + rawValue + doubleQuotes);
            Assert.Equal(expectedValue, encodedValue);
        }

        [Fact]
        public void Encode_ValueContainingLineBreaks_LineBreaksAreReplacedBySpaces()
        {
            // Arrage
            string rawValue = @"
This string
is split
over multiple
lines.";

            // Act
            string encodedValue = CsvWriter<object>.Encode(rawValue);

            // Assert
            Assert.Equal("This string is split over multiple lines.", encodedValue);
        }

        [Fact]
        public void Encode_ValueWithWhitespace_WhitespaceIsTrimmed()
        {
            // Arrage
            string rawValue = "   Hello World   ";

            // Act
            string encodedValue = CsvWriter<object>.Encode(rawValue);

            // Assert
            Assert.Equal("Hello World", encodedValue);
        }

        [Fact]
        public void CsvWriter_WithThreeDefinedColumns_WritesValuesInOrder()
        {
            // Arrange
            CsvWriter<object> writer = new CsvWriter<object>();

            writer.DefineColumn("Column 1", x => "Value 1");
            writer.DefineColumn("Column 2", x => "Value 2");
            writer.DefineColumn("Column 3", x => "Value 3");

            object[] data = new object[2];

            // Act
            string csv = writer.Write(data);

            // Assert
            string expectedValue =
                "Column 1,Column 2,Column 3" + Environment.NewLine +
                "Value 1,Value 2,Value 3" + Environment.NewLine +
                "Value 1,Value 2,Value 3" + Environment.NewLine;

            Assert.Equal(expectedValue, csv);
        }

        [Fact]
        public void CsvWriter_WithFuncColumn_UsesFuncToRetrieveValues()
        {
            // Arrange
            CsvWriter<int> writer = new CsvWriter<int>();

            writer.DefineColumn("Column 1", x => x * x);

            List<int> data = new List<int>() { 1, 2, 3, 4 };

            // Act
            string csv = writer.Write(data);

            // Assert
            string expectedValue =
                "Column 1" + Environment.NewLine +
                "1" + Environment.NewLine +
                "4" + Environment.NewLine +
                "9" + Environment.NewLine +
                "16" + Environment.NewLine;

            Assert.Equal(expectedValue, csv);
        }

        [Fact]
        public void CsvWriter_WithExcelSanitizer_SanitizesValues()
        {
            // Arrange
            IExcelSanitizer sanitizer = A.Fake<IExcelSanitizer>();
            A.CallTo(() => sanitizer.IsThreat("Bad String")).Returns(true);
            A.CallTo(() => sanitizer.Sanitize("Bad String")).Returns("Sanitized Bad String");

            CsvWriter<string> writer = new CsvWriter<string>(sanitizer);

            writer.DefineColumn("Column 1", x => x);

            List<string> data = new List<string>() { "Good String", "Bad String" };

            // Act
            string csv = writer.Write(data);

            // Assert
            string expectedValue =
                "Column 1" + Environment.NewLine +
                "Good String" + Environment.NewLine +
                "Sanitized Bad String" + Environment.NewLine;

            Assert.Equal(expectedValue, csv);
        }
    }
}
