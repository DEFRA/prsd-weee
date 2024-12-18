namespace EA.Weee.Core.Tests.Unit.Validation
{
    using System;
    using System.Linq;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Validation;
    using FluentAssertions;
    using Xunit;

    public class ExternalAddressValidatorTests
    {
        [Fact]
        public void Validate_WhenCountryIdIsEmpty_ShouldReturnCountryValidationError()
        {
            // Arrange
            var countryId = Guid.Empty;
            var postCode = "SW1A 1AA";
            var countryIdName = "CountryId";
            var postcodeName = "Postcode";

            // Act
            var result = ExternalAddressValidator.Validate(countryId, postCode, countryIdName, postcodeName).ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].ErrorMessage.Should().Be("Enter country");
            result[0].MemberNames.Should().ContainSingle().Which.Should().Be(countryIdName);
        }

        [Theory]
        [InlineData("SW1A 1AA", true)]
        [InlineData("SW1A1AA", true)]
        [InlineData("sw1a 1aa", true)]
        [InlineData("12345", false)]
        [InlineData("", true)]
        [InlineData(null, true)]
        public void Validate_WhenUkCountry_ShouldValidatePostcode(string postCode, bool isValid)
        {
            // Arrange
            var countryId = UkCountry.ValidIds.ElementAt(0);
            var countryIdName = "CountryId";
            var postcodeName = "Postcode";

            // Act
            var result = ExternalAddressValidator.Validate(countryId, postCode, countryIdName, postcodeName).ToList();

            // Assert
            if (isValid)
            {
                result.Should().BeEmpty();
            }
            else
            {
                result.Should().HaveCount(1);
                result[0].ErrorMessage.Should().Be("Enter a full UK postcode");
                result[0].MemberNames.Should().ContainSingle().Which.Should().Be(postcodeName);
            }
        }

        [Fact]
        public void Validate_WhenNonUkCountry_ShouldNotValidatePostcode()
        {
            // Arrange
            var countryId = Guid.NewGuid(); // Assuming this is not in UkCountryList.ValidCountryIds
            var postCode = "12345"; // Invalid UK postcode
            var countryIdName = "CountryId";
            var postcodeName = "Postcode";

            // Act
            var result = ExternalAddressValidator.Validate(countryId, postCode, countryIdName, postcodeName).ToList();

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("SW1")]
        [InlineData("W1")]
        [InlineData("B33")]
        [InlineData("M1")]
        [InlineData("G1")]
        [InlineData("EH1")]
        [InlineData("CF10")]
        [InlineData("BT1")]
        public void ValidPartialPostcodes_ShouldReturnTrue(string partialPostcode)
        {
            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(partialPostcode);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("123")]
        [InlineData("AAA")]
        [InlineData("A")]
        [InlineData("99AA")]
        [InlineData("A*1")]
        public void InvalidPartialPostcodes_ShouldReturnFalse(string partialPostcode)
        {
            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(partialPostcode);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("sw1")]
        [InlineData("w1")] 
        [InlineData("b33")]
        [InlineData("m1")] 
        public void LowercasePartialPostcodes_ShouldReturnTrue(string partialPostcode)
        {
            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(partialPostcode);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void LongInput_ShouldReturnFalse()
        {
            // Arrange
            var longInput = new string('A', 30) + "1";

            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(longInput);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("SW12")]
        [InlineData("SW123")]
        public void ValidExtendedPartialPostcodes_ShouldReturnTrue(string partialPostcode)
        {
            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(partialPostcode);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(" SW1")]
        public void PostcodesWithLeadingSpaces_ShouldReturnFalse(string partialPostcode)
        {
            // Act
            var result = ExternalAddressValidator.IsValidPartialPostcode(partialPostcode);

            // Assert
            result.Should().BeFalse();
        }
    }
}