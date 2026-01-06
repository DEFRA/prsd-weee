namespace EA.Weee.Core.Tests.Unit.Validation
{
    using EA.Weee.Core.Validation;
    using Xunit;

    public class GenericEmailAddressAttributeTests
    {
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("john_doe123@company.org")]
        [InlineData("admin+support@my-site.io")]
        [InlineData("contact@sub.domain.com")]
        public void IsValidEmail_ShouldReturnTrue_ForValidEmails(string email)
        {
            //Arrange
            GenericEmailAddressAttribute attribute = new GenericEmailAddressAttribute();

            //Act
            var result = attribute.IsValid(email);

            //Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("@missinglocal.com")]
        [InlineData("user@.com")]
        [InlineData("user@domain")]
        [InlineData("user@domain.c")]
        [InlineData("")]
        [InlineData(null)]
        public void IsValidEmail_ShouldReturnFalse_ForInvalidEmails(string email)
        {
            //Arrange
            GenericEmailAddressAttribute attribute = new GenericEmailAddressAttribute();

            //Act
            var result = attribute.IsValid(email);

            //Assert
            Assert.False(result);
        }
    }
}
