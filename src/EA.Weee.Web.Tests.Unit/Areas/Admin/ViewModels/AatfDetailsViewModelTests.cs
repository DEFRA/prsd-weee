namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
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

            Assert.Equal(expected, model.FacilityType.ToDisplayString());
        }

        [Theory]
        [InlineData(FacilityType.Aatf, "Site address")]
        [InlineData(FacilityType.Ae, "AE address")]
        public void DetailViewModel_FacilityTypeSet_AddressLabelSetCorrectly(FacilityType type, string expected)
        {
            AatfDetailsViewModel model = new AatfDetailsViewModel();

            model.FacilityType = type;

            Assert.Equal(expected, model.AddressHeadingName);
        }
    }
}
