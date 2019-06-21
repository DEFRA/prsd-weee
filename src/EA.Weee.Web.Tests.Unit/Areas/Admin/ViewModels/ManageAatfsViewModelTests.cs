namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using Xunit;

    public class ManageAatfsViewModelTests
    {
        [Theory]
        [InlineData(FacilityType.Aatf)]
        [InlineData(FacilityType.Ae)]
        public void Selected_NoSelected_ErrorMessageWithCorrectFacility(FacilityType type)
        {
            var model = new ManageAatfsViewModel()
            {
                FacilityType = type
            };

            ValidationContext validationContext = new ValidationContext(model, null, null);

            IList<ValidationResult> result = model.Validate(validationContext).ToList();

            Assert.True(result.Count() > 0);
            Assert.Equal(string.Format("You must select an {0} to manage", type.ToString().ToUpper()), result[0].ErrorMessage);
        }
    }
}
