namespace EA.Weee.Migration.Organisation.Tests.Validation
{
    using FluentAssertions;
    using Model;
    using Organisation.Validation;
    using Xunit;

    public class AddressDataValidatorTests
    {
        private AddressDataValidator validator;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RuleFor_Address1IsNullOrEmpty_ErrorShouldOccur(string value)
        {
            var address = GenerateAddress();

            address.Address1 = value;

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_Address1GreaterThan60_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.Address1 = new string('a', 61);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_Address2GreaterThan60_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.Address2 = new string('a', 61);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RuleFor_TownOrCityIsNullOrEmpty_ErrorShouldOccur(string value)
        {
            var address = GenerateAddress();

            address.TownOrCity = value;

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_TownOrCityGreaterThan35_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.TownOrCity = new string('a', 36);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_CountyOrRegionGreaterThan35_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.CountyOrRegion = new string('a', 36);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_PostcodeGreaterThan10_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.Postcode = new string('a', 11);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RuleFor_TelephoneIsNullOrEmpty_ErrorShouldOccur(string value)
        {
            var address = GenerateAddress();

            address.Telephone = value;

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_TelephoneGreaterThan20_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.Telephone = new string('a', 21);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void RuleFor_EmailIsNullOrEmpty_ErrorShouldOccur(string value)
        {
            var address = GenerateAddress();

            address.Email = value;

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        [Fact]
        public void RuleFor_EmailGreaterThan256_ErrorShouldOccur()
        {
            var address = GenerateAddress();

            address.Email = new string('a', 257);

            validator = new AddressDataValidator();

            var validationResult = validator.Validate(address);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Count.Should().Be(1);
        }

        private Address GenerateAddress()
        {
            return new Address("Address1", "Address2", "Town", "County", "Postcode", "CountryName", "Telephone", "Email");
        }
    }
}
