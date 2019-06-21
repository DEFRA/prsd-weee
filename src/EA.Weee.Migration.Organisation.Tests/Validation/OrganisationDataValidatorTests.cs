namespace EA.Weee.Migration.Organisation.Tests.Validation
{
    using FakeItEasy;
    using FluentAssertions;
    using Model;
    using Organisation.Validation;
    using Xunit;

    public class OrganisationDataValidatorTests
    {
        private OrganisationDataValidator validator;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RuleFor_NameIsNullOrEmpty_ErrorShouldOccur(string value)
        {
            var organisation = GenerateOrganisation();

            organisation.Name = value;

            validator = new OrganisationDataValidator();

            var validationResult = validator.Validate(organisation);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_NameGreaterThan256_ErrorShouldOccur()
        {
            var organisation = GenerateOrganisation();

            organisation.Name = new string('a', 257);

            validator = new OrganisationDataValidator();

            var validationResult = validator.Validate(organisation);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_TradingNameGreaterThan256_ErrorShouldOccur()
        {
            var organisation = GenerateOrganisation();

            organisation.TradingName = new string('a', 257);

            validator = new OrganisationDataValidator();

            var validationResult = validator.Validate(organisation);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Theory]
        [InlineData("01234567890123456")]
        [InlineData("012345")]
        public void RuleFor_RegistrationNumberLengthOutsideLimit_ErrorShouldOccur(string value)
        {
            var organisation = GenerateOrganisation();

            organisation.RegistrationNumber = value;

            validator = new OrganisationDataValidator();

            var validationResult = validator.Validate(organisation);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        private Organisation GenerateOrganisation()
        {
            return new Organisation(1, "Name", null, OrganisationType.RegisteredCompany,
                "0123456789", A.Fake<Address>());
        }
    }
}
