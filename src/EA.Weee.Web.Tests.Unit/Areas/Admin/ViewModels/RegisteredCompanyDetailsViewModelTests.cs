namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class RegisteredCompanyDetailsViewModelTests
    {
        [Theory]
        [InlineData("1234567", true)]
        [InlineData("123456", false)]
        [InlineData("1234567891234567", false)]
        public void CheckRegistrationCompanyNumberIsValidLength(string registrationNumber, bool result)
        {
            var model = ValidRegisteredCompanyDetailsViewModel();
            model.CompaniesRegistrationNumber = registrationNumber;

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.Equal(result, isValid);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CheckCompanyNameIsRequired(bool populateCompanyName, bool result)
        {
            var model = ValidRegisteredCompanyDetailsViewModel();
            if (!populateCompanyName)
            {
                model.CompanyName = null;
            }

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.Equal(result, isValid);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public void CheckBusinessTradingNameIsNotRequired(bool populateTradingName, bool result)
        {
            var model = ValidRegisteredCompanyDetailsViewModel();
            if (!populateTradingName)
            {
                model.BusinessTradingName = null;
            }

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.Equal(result, isValid);
        }

        private RegisteredCompanyDetailsViewModel ValidRegisteredCompanyDetailsViewModel()
        {
            return new RegisteredCompanyDetailsViewModel
            {
                OrganisationType = "RegisteredCompany",
                CompanyName = "Company name",
                BusinessTradingName = "Trading name",
                Address = new AddressData(),
                CompaniesRegistrationNumber = "7654321"
            };
        }
    }
}
