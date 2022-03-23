namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using EA.Weee.Web.Extensions;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class SelectYourAatfViewModelTests
    {
        [Fact]
        public void Selected_NoSelected_ShowsErrorMessage()
        {
            var facilityType = FacilityType.Aatf;
            var model = new SelectYourAatfViewModel()
            {
                FacilityType = facilityType,
            };

            var validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Any());
            result[0].ErrorMessage.Should().Be($"Select the site you would like to manage");
        }
    }
}
