namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using System;
    using System.Globalization;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.Areas.AatfReturn.ViewModels;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnViewModelBaseTests
    {
        public ReturnViewModelBaseTests()
        {
        }

        [Fact]
        public void Constructor_GivenReturnDataIsNull_ArgumentNullExceptionExpected()
        {
            Action action = () =>
            {
                var model = new ReturnViewModelTest(null);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_GivenQuarterIsNull_ArgumentNullExceptionExpected()
        {
            var returnData = new ReturnData()
            {
                QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow()
            };

            Action action = () =>
            {
                var model = new ReturnViewModelTest(returnData);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_GivenQuarterWindowIsNull_ArgumentNullExceptionExpected()
        {
            var returnData = new ReturnData()
            {
                Quarter = new Quarter(2019, QuarterType.Q1)
            };

            Action action = () =>
            {
                var model = new ReturnViewModelTest(returnData);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_GivenCreatedBy_CreatedPropertiesShouldBeSet()
        {
            var returnData = new ReturnData() { CreatedBy = "createdBy", CreatedDate = new DateTime(2019, 1, 1, 11, 1, 2), Quarter = GetQuarter(), QuarterWindow = GetQuarterWindow() };

            var model = new ReturnViewModelTest(returnData);

            model.CreatedBy.Should().Be("createdBy");
            model.CreatedDate.Should().Be("01/01/2019 11:01:02");
        }

        [Fact]
        public void Constructor_GivenSubmittedBy_SubmittedPropertiesShouldBeSet()
        {
            var returnData = new ReturnData() { SubmittedBy = "submittedBy", SubmittedDate = new DateTime(2019, 1, 1, 11, 1, 2), Quarter = GetQuarter(), QuarterWindow = GetQuarterWindow() };

            var model = new ReturnViewModelTest(returnData);

            model.SubmittedBy.Should().Be("submittedBy");
            model.SubmittedDate.Should().Be("01/01/2019 11:01:02");
        }

        [Fact]
        public void Constructor_GivenQuarterDetails_QuarterPropertiesShouldBeSet()
        {
            var returnData = new ReturnData() { Quarter = GetQuarter(), QuarterWindow = GetQuarterWindow() };

            var model = new ReturnViewModelTest(returnData);

            model.Year.Should().Be(returnData.Quarter.Year.ToString());
            model.Quarter.Should().Be(returnData.Quarter.Q.ToString());
            model.Period.Should()
                .Be(
                    $"{returnData.Quarter.Q.ToString()} {returnData.QuarterWindow.QuarterStart.ToString("MMM", CultureInfo.CurrentCulture)} - {returnData.QuarterWindow.QuarterEnd.ToString("MMM", CultureInfo.CurrentCulture)} {returnData.Quarter.Year}");
        }

        [Fact]
        public void Constructor_GivenReturnStatus_ReturnStatusShouldBeSet()
        {
            foreach (var value in Enum.GetValues(typeof(ReturnStatus)))
            {
                var returnData = new ReturnData()
                {
                    Quarter = GetQuarter(), 
                    QuarterWindow = GetQuarterWindow(),
                    ReturnStatus = (ReturnStatus)value
                };

                var model = new ReturnViewModelTest(returnData);

                model.ReturnStatus.Should().Be((ReturnStatus)value);
            }
        }

        [Fact]
        public void Constructor_GivenSubmittedDateIsNull_SubmittedDateShouldBeFormattedAsDash()
        {
            var model = new ReturnViewModelTest(DefaultReturnData());

            model.SubmittedDate.Should().Be("-");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_GivenSubmittedDateEmpty_SubmittedDateShouldBeFormattedAsDash(string submittedBy)
        {
            var model = new ReturnViewModelTest(new ReturnData() { Quarter = GetQuarter(), QuarterWindow = GetQuarterWindow(), SubmittedBy = submittedBy });
           
            model.SubmittedBy.Should().Be("-");
        }

        [Fact]
        public void Constructor_GivenReturnDataId_ReturnIdShouldBeSet()
        {
            var returnData = DefaultReturnData();

            var model = new ReturnViewModelTest(returnData);

            model.ReturnId.Should().Be(returnData.Id);
        }

        private ReturnData DefaultReturnData()
        {
            return new ReturnData() { Quarter = GetQuarter(), QuarterWindow = GetQuarterWindow(), Id = Guid.NewGuid() };
        }

        private Quarter GetQuarter()
        {
            return new Quarter(2019, QuarterType.Q1);
        }

        private QuarterWindow GetQuarterWindow()
        {
            return QuarterWindowTestHelper.GetDefaultQuarterWindow();
        }
    }
}
