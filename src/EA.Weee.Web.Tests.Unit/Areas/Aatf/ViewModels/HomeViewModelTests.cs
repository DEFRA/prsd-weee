namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class HomeViewModelTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Selected_NoSelected_ErrorMessageWithCorrectFacility(bool isAE)
        {
            var model = new HomeViewModel()
            {
                IsAE = isAE
            };

            ValidationContext validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Count() > 0);
            if (isAE)
            {
                result[0].ErrorMessage.Should().Be("Select an AE to perform activities");
            }
            else
            {
                result[0].ErrorMessage.Should().Be("Select an AATF to perform activities");
            }
        }
    }
}
