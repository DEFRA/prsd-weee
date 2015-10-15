namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using Xunit;

    public class GenericPhoneNumberAttributeTests
    {
        [Theory]
        [InlineData("1234 1233 123456&")]
        [InlineData("1234 %^")]
        [InlineData("+ext 1234#")]
        public void ValidatePhoneNumberRegex_ReturnsFalse(string phoneNumber)
        {
            //Arrange
            GenericPhoneNumberAttribute attribute = new GenericPhoneNumberAttribute();

            //Act
            var result = attribute.IsValid(phoneNumber);

            //Assert
            Assert.Equal(false, result);
        }

        [Theory]
        [InlineData("1234 1233 12345")]
        [InlineData("(1234) (12345) (5656)")]
        [InlineData("+65 123 4343 34343")]
        [InlineData("123.4343.343.43")]
        [InlineData("1245 5435435245")]
        [InlineData("++..()")]
        [InlineData("123-123243-34343")]
        public void ValidatePhoneNumberRegex_ReturnsTrue(string phoneNumber)
        {
            //Arrange
            GenericPhoneNumberAttribute attribute = new GenericPhoneNumberAttribute();

            //Act
            var result = attribute.IsValid(phoneNumber);

            //Assert
            Assert.Equal(true, result);
        }
    }
}
