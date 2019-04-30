namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using Xunit;

    public class SelectReportOptionsViewModelTests
    {
        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public void SelectReportOptionsViewModel_GivenSelectedValueIsYesOrNo_IsValid(string selectedValue)
        {
            var viewModel = new SelectReportOptionsViewModel(Guid.NewGuid(), Guid.NewGuid(), A.Fake<List<ReportOnQuestion>>(), A.Dummy<Quarter>(), A.Fake<QuarterWindow>(), A.Dummy<int>()) { DcfSelectedValue = selectedValue };

            var context = new ValidationContext(viewModel, null, null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, context, results, true);

            Assert.True(isValid);
        }
    }
}
