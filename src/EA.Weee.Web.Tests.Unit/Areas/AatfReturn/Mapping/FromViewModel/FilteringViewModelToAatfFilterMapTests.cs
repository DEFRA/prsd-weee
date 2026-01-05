namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using System;
    using Xunit;

    public class FilteringViewModelToAatfFilterMapTests
    {
        private readonly FilteringViewModelToAatfFilterMap map;
        private readonly Fixture fixture;

        public FilteringViewModelToAatfFilterMapTests()
        {
            map = new FilteringViewModelToAatfFilterMap();
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var viewModel = fixture.Build<FilteringViewModel>().With(a => a.FacilityType, FacilityType.Aatf).Create();

            var result = map.Map(viewModel);

            Assert.Equal(viewModel.ApprovalNumber, result.ApprovalNumber);
            Assert.Equal(viewModel.Name, result.Name);
            Assert.Equal(viewModel.SelectedAuthority, result.SelectedAuthority);
            Assert.Equal(viewModel.SelectedStatus, result.SelectedStatus);
        }

        [Fact]
        public void Map_GivenValidSourceWithComplianceYear_ComplianceYearShouldBeMapped()
        {
            var complianceYear = 2024;
            var viewModel = fixture.Build<FilteringViewModel>()
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.SelectedComplianceYear, complianceYear)
                .Create();

            var result = map.Map(viewModel);

            result.ComplianceYear.Should().Be(complianceYear);
        }

        [Fact]
        public void Map_GivenValidSourceWithNullComplianceYear_ComplianceYearShouldBeNull()
        {
            var viewModel = fixture.Build<FilteringViewModel>()
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.SelectedComplianceYear, (int?)null)
                .Create();

            var result = map.Map(viewModel);

            result.ComplianceYear.Should().BeNull();
        }

        [Theory]
        [InlineData(2019)]
        [InlineData(2020)]
        [InlineData(2021)]
        [InlineData(2024)]
        [InlineData(2025)]
        public void Map_GivenVariousComplianceYears_ComplianceYearShouldBeMappedCorrectly(int year)
        {
            var viewModel = fixture.Build<FilteringViewModel>()
                .With(a => a.FacilityType, FacilityType.Aatf)
                .With(a => a.SelectedComplianceYear, year)
                .Create();

            var result = map.Map(viewModel);

            result.ComplianceYear.Should().Be(year);
        }
    }
}
