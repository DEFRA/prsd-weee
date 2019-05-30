﻿namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using System;
    using Xunit;

    public class AatfDetailsViewModelTests
    {
        [Fact]
        public void ViewModelHasApprovalDate_OnlyDisplaysDateNotTime_DDMMYYYY()
        {
            AatfDetailsViewModel model = new AatfDetailsViewModel()
            {
                ApprovalDate = new DateTime(2019, 12, 22, 13, 10, 10)
            };

            Assert.Equal("22/12/2019", model.ApprovalDateString);
        }

        [Fact]
        public void ViewModelHasNoApprovalDate_DisplaysHyphen()
        {
            AatfDetailsViewModel model = new AatfDetailsViewModel();

            Assert.Equal("-", model.ApprovalDateString);
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "AATF")]
        [InlineData(FacilityType.Aatf, "AATF")]
        public void ViewModel_GetFacilityTypeString(FacilityType type, string expected)
        {
            AatfDetailsViewModel model = new AatfDetailsViewModel()
            {
                FacilityType = type
            };

            Assert.Equal(expected, model.FacilityTypeString);
        }
    }
}
