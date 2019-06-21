namespace EA.Weee.Web.Tests.Unit.Areas.AeReturn.ViewModels
{
    using EA.Weee.Web.Areas.AeReturn.ViewModels;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class ExportedWholeWeeeViewModelTests
    {
        [Theory]
        [InlineData(YesNoEnum.Yes, true)]
        [InlineData(YesNoEnum.NotSelected, false)]
        public void SelectedValue_Validation_ReturnsResult(YesNoEnum selectedValue, bool result)
        {
            ExportedWholeWeeeViewModel viewModel = new ExportedWholeWeeeViewModel()
            {
                WeeeSelectedValue = selectedValue
            };

            var context = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            Assert.Equal(isValid, result);
        }
    }
}
