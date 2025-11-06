namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using Xunit;

    public class GenericFaxNumberAttributeTests
    {
        [Theory]
        [InlineData("15551234567")]        // Missing +
        [InlineData("+1-555-123-4567")]    // Contains invalid chars
        [InlineData("+12")]                // Too short
        [InlineData("+12345678901234567")] // Too long
        [InlineData("+1ABC12345")]         // Letters
        public void ValidateFaxNumberRegex_ReturnsFalse(string faxNumber)
        {
            //Arrange
            GenericFaxNumberAttribute attribute = new GenericFaxNumberAttribute();

            //Act
            var result = attribute.IsValid(faxNumber);

            //Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("+15551234567")]     // US
        [InlineData("+442079460958")]    // UK
        [InlineData("+81312345678")]     // Japan
        [InlineData("+61298765432")]     // Australia
        [InlineData("+4930123456")]      // Germany
        public void ValidateFaxNumberRegex_ReturnsTrue(string faxNumber)
        {
            //Arrange
            GenericFaxNumberAttribute attribute = new GenericFaxNumberAttribute();

            //Act
            var result = attribute.IsValid(faxNumber);

            //Assert
            Assert.True(result);
        }
    }
}
