namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using Xunit;

    public class GenericFaxNumberAttributeTests
    {
        [Theory]
        [InlineData("1234 1233 123456&")]
        [InlineData("1234 %^")]
        [InlineData("+ext 1234#")]
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
