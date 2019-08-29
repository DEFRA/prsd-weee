namespace EA.Prsd.Core.Tests.Mvc
{
    using System;
    using System.Web.Mvc;
    using Core.Web.Mvc.RazorHelpers;
    using Xunit;

    public class GdsDateHelperTests
    {
        Gds<TestViewModel> gds;

        public GdsDateHelperTests()
        {
            var viewDataContainer = new FakeViewDataContainer();
            var viewContext = new ViewContext();
            viewContext.HttpContext = new FakeHttpContext();
            viewDataContainer.ViewData.Model = new TestViewModel() 
            { 
                JanFirst2016 = new DateTime(2016, 1, 1),
                MarchThirtyFirst2016 = new DateTime(2016, 3, 31),
                MarchThirtyFirst2017 = new DateTime(2017, 3, 31)
            };
            gds = new Gds<TestViewModel>(new HtmlHelper<TestViewModel>(viewContext, viewDataContainer));
        }

        [Fact]
        public void DisplayDateFor_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayDateFor(m => m.JanFirst2016);
            Assert.Equal("1 January 2016", result.ToString());
        }

        [Fact]
        public void DisplayShortDateFor_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayShortDateFor(m => m.JanFirst2016);
            Assert.Equal("1 Jan 2016", result.ToString());
        }

        [Fact]
        public void DisplayDateRangeFor_ShowsBothYearsIfDifferent()
        {
            MvcHtmlString result = gds.DisplayDateRangeFor(m => m.JanFirst2016, m => m.MarchThirtyFirst2017);
            Assert.Equal("1 January 2016 to 31 March 2017", result.ToString());
        }

        [Fact]
        public void DisplayDateRangeFor_OmitsFirstYearIfSame()
        {
            MvcHtmlString result = gds.DisplayDateRangeFor(m => m.JanFirst2016, m => m.MarchThirtyFirst2016);
            Assert.Equal("1 January to 31 March 2016", result.ToString());
        }

        [Fact]
        public void DisplayShortDateRangeFor_ShowsBothYearsIfDifferent()
        {
            MvcHtmlString result = gds.DisplayShortDateRangeFor(m => m.JanFirst2016, m => m.MarchThirtyFirst2017);
            Assert.Equal("1 Jan 2016 to 31 Mar 2017", result.ToString());
        }

        [Fact]
        public void DisplayShortDateRangeFor_OmitsFirstYearIfSame()
        {
            MvcHtmlString result = gds.DisplayShortDateRangeFor(m => m.JanFirst2016, m => m.MarchThirtyFirst2016);
            Assert.Equal("1 Jan to 31 Mar 2016", result.ToString());
        }

        [Fact]
        public void DisplayDateFor_ExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayDateFor(null);
            Assert.Throws<ArgumentNullException>("expression", action);
        }

        [Fact]
        public void DisplayShortDateFor_ExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayShortDateFor(null);
            Assert.Throws<ArgumentNullException>("expression", action);
        }

        [Fact]
        public void DisplayDateRangeFor_FromDateExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayDateRangeFor(null, m => m.JanFirst2016);
            Assert.Throws<ArgumentNullException>("fromDateExpression", action);
        }

        [Fact]
        public void DisplayDateRangeFor_ToDateExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayDateRangeFor(m => m.JanFirst2016, null);
            Assert.Throws<ArgumentNullException>("toDateExpression", action);
        }

        [Fact]
        public void DisplayShortDateRangeFor_FromDateExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayShortDateRangeFor(null, m => m.JanFirst2016);
            Assert.Throws<ArgumentNullException>("fromDateExpression", action);
        }

        [Fact]
        public void DisplayShortDateRangeFor_ToDateExpressionIsNull_ThrowsArgumentNullException()
        {
            Action action = () => gds.DisplayShortDateRangeFor(m => m.JanFirst2016, null);
            Assert.Throws<ArgumentNullException>("toDateExpression", action);
        }

        [Fact]
        public void DisplayDate_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayDate(new DateTime(2016, 1, 1));
            Assert.Equal("1 January 2016", result.ToString());
        }

        [Fact]
        public void DisplayShortDate_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayShortDate(new DateTime(2016, 1, 1));
            Assert.Equal("1 Jan 2016", result.ToString());
        }

        [Fact]
        public void DisplayDateRange_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayDateRange(new DateTime(2016, 1, 1), new DateTime(2017, 3, 31));
            Assert.Equal("1 January 2016 to 31 March 2017", result.ToString());
        }

        [Fact]
        public void DisplayShortDateRange_FormatsCorrectly()
        {
            MvcHtmlString result = gds.DisplayShortDateRange(new DateTime(2016, 1, 1), new DateTime(2017, 3, 31));
            Assert.Equal("1 Jan 2016 to 31 Mar 2017", result.ToString());
        }

        private class TestViewModel
        {
            public DateTime JanFirst2016 { get; set; }
            public DateTime MarchThirtyFirst2016 { get; set; }
            public DateTime MarchThirtyFirst2017 { get; set; }
        }
    }
}
