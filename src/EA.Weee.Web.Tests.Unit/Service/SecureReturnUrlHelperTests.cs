namespace EA.Weee.Web.Tests.Unit.Service
{
    using System;
    using System.Linq;
    using AutoFixture;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

        public class SecureReturnUrlHelperTests
    {
        private readonly Fixture fixture;
        private readonly SecureReturnUrlHelper helper;

        public SecureReturnUrlHelperTests()
        {
            fixture = new Fixture();
            var configurationService = A.Fake<ConfigurationService>();
            A.CallTo(() => configurationService.CurrentConfiguration.GovUkPayTokenSecret)
                .Returns("TestSecret123!@#");
            helper = new SecureReturnUrlHelper(configurationService);
        }

        [Fact]
        public void GenerateSecureRandomString_ShouldReturnValidString()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = helper.GenerateSecureRandomString(guid);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Count(c => c == '.').Should().Be(2);
            result.Split('.').Length.Should().Be(3);
        }

        [Fact]
        public void GenerateSecureRandomString_ShouldReturnDifferentStringsForDifferentGuids()
        {
            // Arrange
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();

            // Act
            var result1 = helper.GenerateSecureRandomString(guid1);
            var result2 = helper.GenerateSecureRandomString(guid2);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void ValidateSecureRandomString_WithValidInput_ShouldReturnTrueAndCorrectGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var input = helper.GenerateSecureRandomString(guid);

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(input);

            // Assert
            isValid.Should().BeTrue();
            extractedGuid.Should().Be(guid);
        }

        [Fact]
        public void ValidateSecureRandomString_WithInvalidInput_ShouldReturnFalseAndEmptyGuid()
        {
            // Arrange
            var invalidInput = "invalid.input.string";

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(invalidInput);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithNullInput_ShouldReturnFalseAndEmptyGuid()
        {
            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(null);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithEmptyInput_ShouldReturnFalseAndEmptyGuid()
        {
            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(string.Empty);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithTamperedInput_ShouldReturnFalseButExtractCorrectGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var input = helper.GenerateSecureRandomString(guid);
            var tamperedInput = input.Substring(0, input.Length - 1) + "X"; // Change last character

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(tamperedInput);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(guid);
        }

        [Theory]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        public void GenerateSecureRandomString_WithDifferentLengths_ShouldReturnValidString(int length)
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = helper.GenerateSecureRandomString(guid, length);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Count(c => c == '.').Should().Be(2);
            result.Split('.').Length.Should().Be(3);
            result.Split('.')[0].Length.Should().BeGreaterOrEqualTo(length); // The actual length might be slightly longer due to Base64 encoding
        }
    }
}