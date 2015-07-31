namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Core.Validation;
    using Xunit;

    public class InternalEmailAddressAttributeTests
    {
        [Theory]
        [InlineData("test@someotherdomain.com")]
        [InlineData("test@environment-agency.gov.uka")]
        [InlineData("test@aenvironment-agency.gov.uk")]
        public void EmailAddressDoesNotBelongToCompetantAuthority_IsInvalid(string emailAddress)
        {
            Assert.False(new InternalEmailAddressAttribute().IsValid(emailAddress));           
        }

        [Theory]
        [InlineData("test@environment-agency.gov.uk")]
        [InlineData("test@cyfoethnaturiolcymru.gov.uk")]
        [InlineData("test@naturalresourceswales.gov.uk")]
        [InlineData("test@sepa.org.uk")]
        [InlineData("test@doeni.gov.uk")]
        [InlineData(null)]
        [InlineData("")]
        public void EmailAddressDoesBelongToCompetantAuthority_IsValid(string emailAddress)
        {
            Assert.True(new InternalEmailAddressAttribute().IsValid(emailAddress));  
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid@emailaddress")]
        [InlineData("anotherinvalid.emailaddress")]
        public void IfEmailAddressIsInvalid_OrNull_OrEmpty_ShouldNotValidate(string emailAddress)
        {
            Assert.True(new InternalEmailAddressAttribute().IsValid(emailAddress));
        }
    }
}
