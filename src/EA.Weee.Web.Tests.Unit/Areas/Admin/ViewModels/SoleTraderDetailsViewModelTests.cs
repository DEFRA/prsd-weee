namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class SoleTraderDetailsViewModelTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CheckCompanyNameIsRequired(bool populateCompanyName, bool result)
        {
            var model = ValidSoleTraderDetailsViewModel();
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
            var model = ValidSoleTraderDetailsViewModel();
            if (!populateTradingName)
            {
                model.BusinessTradingName = null;
            }

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.Equal(result, isValid);
        }

        private SoleTraderDetailsViewModel ValidSoleTraderDetailsViewModel()
        {
            return new SoleTraderDetailsViewModel
            {
                OrganisationType = "RegisteredCompany",
                CompanyName = "Company name",
                BusinessTradingName = "Trading name",
                Address = new AddressData()
            };
        }
    }
}
