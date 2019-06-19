namespace EA.Weee.Web.Tests.Unit.Areas.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using EA.Weee.Core.Shared;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using EA.Weee.Web.Areas.Admin.ViewModels.AddAatf;
    using FluentAssertions;
    using Xunit;

    public class FacilityViewModelBaseTests
    {
        [Theory]
        [InlineData("2018/12/31")]
        [InlineData("2020/1/1")]
        public void RuleFor_ApprovalDateOutOfBounds_ErrorShouldOccur(string input)
        {
            var model = new OverriddenFacilityViewModelBase { ApprovalDate = Convert.ToDateTime(input), ComplianceYear = 2019 };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(1);
            validationResult.First().ErrorMessage.Should().Be("Approval date must be between 01/01/2019 and 31/12/2019");
        }

        [Theory]
        [InlineData("2019/1/1")]
        [InlineData("2019/12/31")]
        [InlineData("2019/7/1")]
        public void RuleFor_ApprovalDateWithinBounds_ErrorShouldNotOccur(string input)
        {
            var model = new OverriddenFacilityViewModelBase { ApprovalDate = Convert.ToDateTime(input), ComplianceYear = 2019 };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(0);
        }

        [Fact]
        public void RuleFor_CompetentAuthorityIsEA_LocalAreaIsNull_ErrorShouldOccur()
        {
            var model = new OverriddenFacilityViewModelBase { CompetentAuthorityId = UKCompetentAuthorityAbbreviationType.EA, LocalAreaId = null, PanAreaId = Guid.NewGuid() };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(1);
            validationResult.First().ErrorMessage.Should().Be("Enter EA area");
        }

        [Fact]
        public void RuleFor_CompetentAuthorityIsEA_PanAreaIsNull_ErrorShouldOccur()
        {
            var model = new OverriddenFacilityViewModelBase { CompetentAuthorityId = UKCompetentAuthorityAbbreviationType.EA, PanAreaId = null, LocalAreaId = Guid.NewGuid() };

            var validationResult = model.Validate(new ValidationContext(model));

            validationResult.Count().Should().Be(1);
            validationResult.First().ErrorMessage.Should().Be("Enter WROS PAT area");
        }

        [Fact]
        public void ValidationMessageDisplayOrder_IsAsExpected()
        {
            var expectedOrdering = new List<string>
            {
                "Name",
                "SiteAddressData.Address1",
                "SiteAddressData.Address2",
                "SiteAddressData.TownOrCity",
                "SiteAddressData.CountyOrRegion",
                "SiteAddressData.Postcode",
                "SiteAddressData.CountryId",
                "ApprovalNumber",
                "CompetentAuthorityId",
                "LocalAreaId",
                "PanAreaId",
                "StatusValue",
                "SizeValue",
                "ApprovalDate"
            };

            var actualOrdering = FacilityViewModelBase.ValidationMessageDisplayOrder.ToList();

            Assert.Equal(expectedOrdering.Count, actualOrdering.Count);
            Assert.Equal(expectedOrdering, actualOrdering);
        }
    }
}
