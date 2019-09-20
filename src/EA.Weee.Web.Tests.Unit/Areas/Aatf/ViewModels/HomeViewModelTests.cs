namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.ViewModels
{
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using FluentAssertions;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfReturn;
    using Xunit;

    public class HomeViewModelTests
    {
        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Ae, "AE")]
        public void Selected_NoSelected_ErrorMessageWithCorrectFacility(FacilityType facilityType, string expected)
        {
            var model = new HomeViewModel()
            {
                FacilityType = facilityType
            };

            var validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Any());
            result[0].ErrorMessage.Should().Be($"Select an {expected} to perform activities");
        }
    }
}
