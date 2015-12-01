namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using Xunit;

    public class ExternalEmailAddressAttributeTests
    {
        [Theory]
        [InlineData("test@someotherdomain.com")]
        [InlineData("test@environment-agency.gov.uka")]
        [InlineData("test@aenvironment-agency.gov.uk")]
        public void IsValid_EmailAddressDoesNotBelongToAuthorisedAuthority_ReturnsTrue(string emailAddress)
        {
            // Arrange
            ExternalEmailAddressAttribute attribute = new ExternalEmailAddressAttribute();

            // Act
            bool result = attribute.IsValid(emailAddress);

            // Assert
            Assert.Equal(true, result);           
        }

        [Theory]
        [InlineData("test@environment-agency.gov.uk")]
        [InlineData("test@cyfoethnaturiolcymru.gov.uk")]
        [InlineData("test@naturalresourceswales.gov.uk")]
        [InlineData("test@sepa.org.uk")]
        [InlineData("test@doeni.gov.uk")]
        public void IsValid_EmailAddressDoesBelongToAuthorisedAuthority_ReturnsFalse(string emailAddress)
        {
            // Arrange
            ExternalEmailAddressAttribute attribute = new ExternalEmailAddressAttribute();

            // Act
            bool result = attribute.IsValid(emailAddress);

            // Assert
            Assert.Equal(false, result);
        }

        /// <summary>
        /// This test ensures that null, empty or invalid email addresses do not fail validation.
        /// This is important otherwise a user may be overloaded with several error messages
        /// about the same input field.
        /// The trade-off here is that the [ExternalEmailAddress] attribute MUST be used
        /// in conjection with [Required].
        /// </summary>
        /// <param name="emailAddress"></param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid@emailaddress")]
        [InlineData("anotherinvalid.emailaddress")]
        public void IsValid_EmailAddressIsNullEmptyOrInvalid_ReturnsTrue(string emailAddress)
        {
            // Arrange
            ExternalEmailAddressAttribute attribute = new ExternalEmailAddressAttribute();

            // Act
            bool result = attribute.IsValid(emailAddress);

            // Assert
            Assert.Equal(true, result);
        }
    }
}
