namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf.Details;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class PartnershipDetailsViewModelTests
    {
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void CheckBusinessTradingNameIsRequired(bool populateTradingName, bool result)
        {
            var model = ValidPartnershipDetailsViewModel();
            if (!populateTradingName)
            {
                model.BusinessTradingName = null;
            }

            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.Equal(result, isValid);
        }

        private PartnershipDetailsViewModel ValidPartnershipDetailsViewModel()
        {
            return new PartnershipDetailsViewModel
            {
                OrganisationType = "RegisteredCompany",
                BusinessTradingName = "Trading name",
                Address = new AddressData()
            };
        }
    }
}
