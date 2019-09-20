namespace EA.Prsd.Core.Tests.Validation
{
    using Core.Validation;
    using Xunit;

    public class UkPhoneNumberAttributeTests
    {
        [Theory]
        [InlineData("0123456789")] // 10 in length
        [InlineData("0123456789012")] // 12 in length
        [InlineData("12345678901")] // Does not start with zero
        [InlineData("abcdefghijk")] // Contains non numerical characters
        public void InvalidPhoneNumbersShouldBeEvaluatedAsInvalid(object phoneNumber)
        {
            var phoneNumberAttribute = new UkPhoneNumberAttribute();
            Assert.False(phoneNumberAttribute.IsValid(phoneNumber));
        }

        [Theory]
        [InlineData("01234567890")]
        [InlineData("01234-567-890")]
        [InlineData("01234 567 890")]
        public void ValidPhoneNumbersShouldBeEvaluatedAsValid(object phoneNumber)
        {
            var phoneNumberAttribute = new UkPhoneNumberAttribute();
            Assert.True(phoneNumberAttribute.IsValid(phoneNumber));
        }
    }
}
