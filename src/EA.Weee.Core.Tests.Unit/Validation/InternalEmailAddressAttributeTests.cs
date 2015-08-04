namespace EA.Weee.Core.Tests.Unit.Validation
{
    using Configuration;
    using Configuration.EmailRules;
    using Core.Validation;
    using FakeItEasy;
    using Xunit;

    public class InternalEmailAddressAttributeTests
    {
        private readonly IConfigurationManagerWrapper configurationManagerWrapper;

        public InternalEmailAddressAttributeTests()
        {
            configurationManagerWrapper = A.Fake<IConfigurationManagerWrapper>();
        }

        [Theory]
        [InlineData("test@someotherdomain.com")]
        [InlineData("test@environment-agency.gov.uka")]
        [InlineData("test@aenvironment-agency.gov.uk")]
        public void EmailAddressDoesNotBelongToCompetantAuthority_IsInvalid(string emailAddress)
        {
            Assert.False(InternalEmailAddressAttribute().IsValid(emailAddress));           
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
            Assert.True(InternalEmailAddressAttribute().IsValid(emailAddress));  
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid@emailaddress")]
        [InlineData("anotherinvalid.emailaddress")]
        public void IfEmailAddressIsInvalid_OrNull_OrEmpty_ShouldNotValidate(string emailAddress)
        {
            Assert.True(InternalEmailAddressAttribute().IsValid(emailAddress));
        }

        private InternalEmailAddressAttribute InternalEmailAddressAttribute()
        {
            var validEmailSuffixes = new[]
            {
                "^.*@environment-agency.gov.uk$",
                "^.*@cyfoethnaturiolcymru.gov.uk$",
                "^.*@naturalresourceswales.gov.uk$",
                "^.*@doeni.gov.uk$",
                "^.*@sepa.org.uk$"
            };

            var internalConfig = new RulesSection
            {
                DefaultAction = RuleAction.Deny
            };

            foreach (var validEmailSuffix in validEmailSuffixes)
            {
                internalConfig.Rules.Add(new RuleElement
                {
                    Type = RuleType.RegEx,
                    Value = validEmailSuffix,
                    Action = RuleAction.Allow
                });
            }

            A.CallTo(() => configurationManagerWrapper.InternalEmailRules)
                .Returns(internalConfig);

            return new InternalEmailAddressAttribute { Configuration = configurationManagerWrapper };
        }
    }
}
