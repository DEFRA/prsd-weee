namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using Xunit;

    public class EditSentOnAatfSiteRequestCreatorTests
    {
        private readonly IEditSentOnAatfSiteRequestCreator editRequestCreator;

        public EditSentOnAatfSiteRequestCreatorTests()
        {
            editRequestCreator = new EditSentOnAatfSiteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = new SentOnCreateSiteOperatorViewModel()
            {
                OrganisationId = Guid.NewGuid()
            };

            var request = editRequestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldBeMapped()
        {
            var organisationId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var weeeSentOnId = Guid.NewGuid();
            var siteAddress = CreateAatfAddressData("SiteTest", "SiteAddress1", "SiteAddress2", "SiteTown", "SiteCountry", "GU22 7UY");
            var operatorAddress = CreateOperatorAddressData("OperatorTest", "OperatorAddress1", "OperatorAddress2", "OperatorTown", "OperatorCountry", "GU22 8UY");

            var viewModel = new SentOnCreateSiteOperatorViewModel()
            {
                WeeeSentOnId = weeeSentOnId,
                OrganisationId = organisationId,
                ReturnId = returnId,
                AatfId = aatfId,
                SiteAddressData = siteAddress,
                OperatorAddressData = operatorAddress
            };

            var request = editRequestCreator.ViewModelToRequest(viewModel);

            request.WeeeSentOnId.Should().Be(weeeSentOnId);
            request.ReturnId.Should().Be(returnId);
            request.AatfId.Should().Be(aatfId);
            request.OrganisationId.Should().Be(organisationId);
            request.SiteAddressData.Should().Be(siteAddress);
            request.OperatorAddressData.Should().Be(operatorAddress);
        }

        public OperatorAddressData CreateOperatorAddressData(string name, string address1, string address2, string townorcity, string countryname, string postcode)
        {
            var addressData = new OperatorAddressData()
            {
                Name = name,
                Address1 = address1,
                Address2 = address2,
                TownOrCity = townorcity,
                CountryName = countryname,
                Postcode = postcode
            };

            return addressData;
        }

        public AatfAddressData CreateAatfAddressData(string name, string address1, string address2, string townorcity, string countryname, string postcode)
        {
            var addressData = new AatfAddressData()
            {
                Name = name,
                Address1 = address1,
                Address2 = address2,
                TownOrCity = townorcity,
                CountryName = countryname,
                Postcode = postcode
            };

            return addressData;
        }
    }
}
