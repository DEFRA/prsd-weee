﻿namespace EA.Weee.Core.Tests.Unit.Shared
{
    using EA.Weee.Core.Shared;
    using Xunit;

    public class ErrorDataTests
    {
        [Fact]
        public void Equals_TheSameObject_ReturnsTrue()
        {
            // Arrage
            ErrorData error = new ErrorData("A Warning", ErrorLevel.Warning);

            // Act
            bool isEqual = error.Equals(error);

            // Assert
            Assert.True(isEqual);
        }

        [Fact]
        public void Equals_ANullObject_ReturnsFalse()
        {
            // Arrage
            ErrorData error = new ErrorData("A Warning", ErrorLevel.Warning);

            // Act
            bool isEqual = error.Equals(null);

            // Assert
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_ObjectWithTheSameDescriptionAndErrorLevel_ReturnsTrue()
        {
            // Arrage
            ErrorData error1 = new ErrorData("A Warning", ErrorLevel.Warning);
            ErrorData error2 = new ErrorData("A Warning", ErrorLevel.Warning);

            // Act
            bool isEqual = error1.Equals(error2);

            // Assert
            Assert.True(isEqual);
        }

        [Fact]
        public void Equals_ObjectWithTheSameDescriptionAndDifferentErrorLevel_ReturnsFalse()
        {
            // Arrage
            ErrorData error1 = new ErrorData("Some Description", ErrorLevel.Warning);
            ErrorData error2 = new ErrorData("Some Description", ErrorLevel.Error);

            // Act
            bool isEqual = error1.Equals(error2);

            // Assert
            Assert.False(isEqual);
        }

        [Fact]
        public void Equals_ObjectWithDifferentDescriptionAndSameErrorLevel_ReturnsFalse()
        {
            // Arrage
            ErrorData error1 = new ErrorData("First Warning", ErrorLevel.Warning);
            ErrorData error2 = new ErrorData("Second Warning", ErrorLevel.Warning);

            // Act
            bool isEqual = error1.Equals(error2);

            // Assert
            Assert.False(isEqual);
        }
    }
}
