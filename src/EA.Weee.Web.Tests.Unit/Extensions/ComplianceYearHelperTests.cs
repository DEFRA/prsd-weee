namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Extensions;
    using FluentAssertions;
    using System;
    using Web.ViewModels.Shared;
    using Xunit;

    public class ComplianceYearHelperTests : SimpleUnitTestBase
    {
        [Theory]
        [InlineData("01/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/12/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/31/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("02/01/2020", new int[] { 2021, 2020 })]
        [InlineData("12/02/2020", new int[] { 2021, 2020 })]
        [InlineData("01/02/2021", new int[] { 2022, 2021, 2020 })]
        [InlineData("03/02/2021", new int[] { 2022, 2021 })]
        public void GetValidComplianceYear_BasedOnCurrentDate(DateTime currentDate, int[] yearList)
        {
            SystemTime.Freeze(new DateTime(currentDate.Year, currentDate.Month, currentDate.Day));
            var value = ComplianceYearHelper.FetchCurrentComplianceYears(currentDate);
            SystemTime.Unfreeze();
            Assert.Equal(yearList, value.ToArray());
        }

        [Theory]
        [InlineData("01/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/12/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/31/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("02/01/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("12/02/2020", new int[] { 2021, 2020, 2019 })]
        [InlineData("01/02/2021", new int[] { 2022, 2021, 2020 })]
        [InlineData("03/02/2021", new int[] { 2022, 2021, 2020 })]
        public void GetCurrentComplianceYear_BasedOnCurrentDate(DateTime currentDate, int[] yearList)
        {
            SystemTime.Freeze(new DateTime(currentDate.Year, currentDate.Month, currentDate.Day));
            var value = ComplianceYearHelper.FetchCurrentComplianceYears(currentDate, true);
            SystemTime.Unfreeze();
            Assert.Equal(yearList, value.ToArray());
        }

        [Fact]
        public void GetSelectedComplianceYear_GivenModelIsNullAndSelectedComplianceYearIsNull_ComplianceYearShouldBeCurrentDateYear()
        {
            //arrange
            var currentDate = new DateTime(2022, 1, 1);

            //act
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(null, null, currentDate);

            //assert
            complianceYear.Should().Be(2022);
        }

        [Fact]
        public void GetSelectedComplianceYear_GivenModelIsNullAndSelectedComplianceYearIsNotNull_ComplianceYearShouldBeSelectedYear()
        {
            //arrange
            var currentDate = new DateTime(2022, 1, 1);
            const int selectedYear = 2023;

            //act
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(null, selectedYear, currentDate);

            //assert
            complianceYear.Should().Be(2023);
        }

        [Fact]
        public void GetSelectedComplianceYear_GivenModelIsNotNullAndSelectedComplianceYearIsNull_ComplianceYearShouldBeModelSelectedYear()
        {
            //arrange
            var currentDate = new DateTime(2022, 1, 1);
            const int selectedYear = 2023;
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2024).Create();

            //act
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(model, selectedYear, currentDate);

            //assert
            complianceYear.Should().Be(2024);
        }
    }
}
