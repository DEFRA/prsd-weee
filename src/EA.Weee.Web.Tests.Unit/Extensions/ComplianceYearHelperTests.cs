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
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(null, currentDate);

            //assert
            complianceYear.Should().Be(2022);
        }

        [Fact]
        public void GetSelectedComplianceYear_GivenModelIsNotNullAndSelectedComplianceYearIsNull_ComplianceYearShouldBeModelSelectedYear()
        {
            //arrange
            var currentDate = new DateTime(2022, 1, 1);
            var model = TestFixture.Build<ManageEvidenceNoteViewModel>()
                .With(m => m.SelectedComplianceYear, 2024).Create();

            //act
            var complianceYear = ComplianceYearHelper.GetSelectedComplianceYear(model, currentDate);

            //assert
            complianceYear.Should().Be(2024);
        }

        [Fact]
        public void FetchCurrentComplianceYearsForEvidence_GivenEvidenceDateIsGreaterThanSystemDate_ShouldThrowAnError()
        {
            // arrange
            DateTime evidenceDate = new DateTime(2022, 01, 01);
            DateTime systemDate = new DateTime(2020, 01, 01);
            SystemTime.Freeze(new DateTime(evidenceDate.Year, evidenceDate.Month, evidenceDate.Day));
            SystemTime.Freeze(new DateTime(systemDate.Year, systemDate.Month, systemDate.Day));

            // act
            var exception = Record.Exception(() => ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(evidenceDate, systemDate));

            // assert
            SystemTime.Unfreeze();
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("01/01/2019", "01/01/2022", new int[] { 2022, 2021, 2020, 2019 })]
        [InlineData("01/12/2020", "01/10/2020", new int[] { 2020 })]
        [InlineData("01/31/2010", "01/01/2020", new int[] { 2020, 2019, 2018, 2017, 2016, 2015, 2014, 2013, 2012, 2011, 2010 })]
        public void FetchCurrentComplianceYearsForEvidence_GivenEvidenceDateAndSystemDateAreNotNull_ShouldReturnAListOfComplianceYears(DateTime evidenceDate, DateTime systemDateTime, int[] yearList)
        {
            // arrange
            SystemTime.Freeze(new DateTime(evidenceDate.Year, evidenceDate.Month, evidenceDate.Day));
            SystemTime.Freeze(new DateTime(systemDateTime.Year, systemDateTime.Month, systemDateTime.Day));

            // act
            var complianceYearList = ComplianceYearHelper.FetchCurrentComplianceYearsForEvidence(evidenceDate, systemDateTime);

            // assert
            SystemTime.Unfreeze();
            Assert.Equal(yearList, complianceYearList.ToArray());
        }
    }
}
