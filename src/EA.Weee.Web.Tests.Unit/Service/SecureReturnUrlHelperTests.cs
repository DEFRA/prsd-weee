namespace EA.Weee.Web.Tests.Unit.Service
{
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class SecureReturnUrlHelperTests
    {
        private readonly IAppConfiguration fakeConfig;
        private readonly SecureReturnUrlHelper helper;
        private const string TestSecret = "TestSecret123!@#";
        private const string TestSalt = "TestSalt123";

        public SecureReturnUrlHelperTests()
        {
            fakeConfig = A.Fake<IAppConfiguration>();
            A.CallTo(() => fakeConfig.GovUkPayTokenSecret).Returns(TestSecret);
            A.CallTo(() => fakeConfig.GovUkPayTokenSalt).Returns(TestSalt);
            A.CallTo(() => fakeConfig.GovUkPayTokenLifeTime).Returns(TimeSpan.FromHours(12));

            helper = new SecureReturnUrlHelper(fakeConfig);
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldThrowConditionException()
        {
            // Act
            Action act = () => new SecureReturnUrlHelper(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithInvalidTokenSecret_ShouldThrowConditionException(string invalidSecret)
        {
            // Arrange
            var invalidConfig = A.Fake<IAppConfiguration>();
            A.CallTo(() => invalidConfig.GovUkPayTokenSecret).Returns(invalidSecret);
            A.CallTo(() => invalidConfig.GovUkPayTokenSalt).Returns(TestSalt);
            A.CallTo(() => invalidConfig.GovUkPayTokenLifeTime).Returns(TimeSpan.FromHours(12));

            // Act
            Action act = () => new SecureReturnUrlHelper(invalidConfig);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_WithZeroTokenLifetime_ShouldThrowConditionException()
        {
            // Arrange
            var invalidConfig = A.Fake<IAppConfiguration>();
            A.CallTo(() => invalidConfig.GovUkPayTokenSecret).Returns(TestSecret);
            A.CallTo(() => invalidConfig.GovUkPayTokenSalt).Returns(TestSalt);
            A.CallTo(() => invalidConfig.GovUkPayTokenLifeTime).Returns(TimeSpan.Zero);

            // Act
            Action act = () => new SecureReturnUrlHelper(invalidConfig);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GenerateSecureRandomString_ShouldGenerateValidFormat()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = helper.GenerateSecureRandomString(guid);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Count(c => c == '.').Should().Be(2);
            result.Should().MatchRegex("^[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+$");
        }

        [Fact]
        public void ValidateSecureRandomString_WithValidToken_ShouldReturnTrueAndOriginalGuid()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var token = helper.GenerateSecureRandomString(guid);

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(token);

            // Assert
            isValid.Should().BeTrue();
            extractedGuid.Should().Be(guid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid")]
        [InlineData("invalid.token")]
        [InlineData("invalid.token.hmac.extra")]
        public void ValidateSecureRandomString_WithInvalidFormat_ShouldReturnFalseAndEmptyGuid(string invalidToken)
        {
            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(invalidToken);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithTamperedToken_ShouldReturnFalse()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var token = helper.GenerateSecureRandomString(guid);
            var tamperedToken = token.Substring(0, token.Length - 1) + "X";

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(tamperedToken);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithExpiredToken_ShouldReturnFalse()
        {
            // Arrange
            var config = A.Fake<IAppConfiguration>();
            A.CallTo(() => config.GovUkPayTokenSecret).Returns(TestSecret);
            A.CallTo(() => config.GovUkPayTokenSalt).Returns(TestSalt);
            A.CallTo(() => config.GovUkPayTokenLifeTime).Returns(TimeSpan.FromSeconds(1));

            var helper = new SecureReturnUrlHelper(config);

            var guid = Guid.NewGuid();
            var token = helper.GenerateSecureRandomString(guid);

            // Verify token is initially valid
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(token);
            isValid.Should().BeTrue("Token should be valid initially");
            extractedGuid.Should().Be(guid);

            // Wait for expiry
            Thread.Sleep(1500); // Wait 1.5 seconds to ensure expiry

            // Verify token is now expired
            var (isExpired, emptyGuid) = helper.ValidateSecureRandomString(token);
            isExpired.Should().BeFalse("Token should be invalid after expiry");
            emptyGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithDifferentSecret_ShouldReturnFalse()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var token = helper.GenerateSecureRandomString(guid);

            var differentConfig = A.Fake<IAppConfiguration>();
            A.CallTo(() => differentConfig.GovUkPayTokenSecret).Returns("DifferentSecret456!@#");
            A.CallTo(() => differentConfig.GovUkPayTokenSalt).Returns(TestSalt);
            A.CallTo(() => differentConfig.GovUkPayTokenLifeTime).Returns(TimeSpan.FromHours(12));

            var differentHelper = new SecureReturnUrlHelper(differentConfig);

            // Act
            var (isValid, extractedGuid) = differentHelper.ValidateSecureRandomString(token);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Theory]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        public void GenerateSecureRandomString_WithDifferentLengths_ShouldGenerateValidTokens(int length)
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var token = helper.GenerateSecureRandomString(guid, length);
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(token);

            // Assert
            isValid.Should().BeTrue();
            extractedGuid.Should().Be(guid);
        }

        [Fact]
        public void GenerateSecureRandomString_MultipleCalls_ShouldGenerateUniqueDifferentTokens()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var tokens = new HashSet<string>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var token = helper.GenerateSecureRandomString(guid);
                tokens.Add(token).Should().BeTrue("Each token should be unique");
            }

            // Assert
            tokens.Count.Should().Be(100);
        }

        [Fact]
        public void GenerateSecureRandomString_ShouldBeUrlSafe()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var token = helper.GenerateSecureRandomString(guid);

            // Assert
            token.Should().NotContain("+", "URL-safe Base64 should not contain +");
            token.Should().NotContain("/", "URL-safe Base64 should not contain /");
            token.Should().NotContain("=", "URL-safe Base64 should not contain padding");
            token.Should().MatchRegex("^[A-Za-z0-9._-]+$", "Should only contain URL-safe characters");
        }

        [Fact]
        public void ValidateSecureRandomString_WithSpecialCharacters_ShouldHandleGracefully()
        {
            // Arrange
            var specialCharsInput = "!@#$%^&*.{}[]|\\:;\"'<>,?/~`";

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(specialCharsInput);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }

        [Fact]
        public void ValidateSecureRandomString_WithVeryLongInput_ShouldHandleGracefully()
        {
            // Arrange
            var veryLongInput = new string('A', 10000) + "." + new string('B', 10000) + "." + new string('C', 10000);

            // Act
            var (isValid, extractedGuid) = helper.ValidateSecureRandomString(veryLongInput);

            // Assert
            isValid.Should().BeFalse();
            extractedGuid.Should().Be(Guid.Empty);
        }
    }
}