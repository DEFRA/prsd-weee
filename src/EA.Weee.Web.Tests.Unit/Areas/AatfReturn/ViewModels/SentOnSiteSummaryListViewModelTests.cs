namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class SentOnSiteSummaryListViewModelTests
    {
        [Fact]
        public void CreateLongAddress_WithoutOptionalFields_ReturnsCorrectAddress()
        {
            AatfAddressData address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            string expected = "<span>Name,</span><span>Address 1,</span><span>Town,</span><span>Country</span>";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(address);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateLongAddress_WithOptionalAddress2_ReturnsCorrectAddress()
        {
            AatfAddressData address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                Address2 = "Address 2",
                TownOrCity = "Town",
                CountryName = "Country"
            };

            string expected = "<span>Name,</span><span>Address 1,</span><span>Address 2,</span><span>Town,</span><span>Country</span>";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(address);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateLongAddress_OperatorAddressIsNull_ReturnsNonBreakingWhiteSpace()
        {
            string expected = "&nbsp";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(null);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateLongAddress_WithOptionalCounty_ReturnsCorrectAddress()
        {
            AatfAddressData address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                CountryName = "Country"
            };

            string expected = "<span>Name,</span><span>Address 1,</span><span>Town,</span><span>County,</span><span>Country</span>";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(address);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateLongAddress_WithOptionalPostcode_ReturnsCorrectAddress()
        {
            AatfAddressData address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                TownOrCity = "Town",
                Postcode = "Postcode",
                CountryName = "Country"
            };

            string expected = "<span>Name,</span><span>Address 1,</span><span>Town,</span><span>Postcode,</span><span>Country</span>";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(address);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CreateLongAddress_WithAllOptionals_ReturnsCorrectAddress()
        {
            AatfAddressData address = new AatfAddressData()
            {
                Name = "Name",
                Address1 = "Address 1",
                Address2 = "Address 2",
                TownOrCity = "Town",
                CountyOrRegion = "County",
                Postcode = "Postcode",
                CountryName = "Country"
            };

            string expected = "<span>Name,</span><span>Address 1,</span><span>Address 2,</span><span>Town,</span><span>County,</span><span>Postcode,</span><span>Country</span>";

            SentOnSiteSummaryListViewModel viewModel = new SentOnSiteSummaryListViewModel();

            string result = viewModel.CreateLongAddress(address);

            Assert.Equal(expected, result);
        }
    }
}
