﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReturnAndAatfToSentOnSummaryListViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnSummaryListViewModelMap map;

        public ReturnAndAatfToSentOnSummaryListViewModelMapTests()
        {
            map = new ReturnAndAatfToSentOnSummaryListViewModelMap(A.Fake<ITonnageUtilities>());
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
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var aatfName = "Name";
            var weeeList = A.Fake<List<WeeeSentOnData>>();

            var transfer = new ReturnAndAatfToSentOnSummaryListViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                WeeeSentOnDataItems = weeeList,
                AatfName = aatfName
            };

            var result = map.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.AatfName.Should().Be(aatfName);
            result.Sites.Should().BeEquivalentTo(weeeList);
        }

        [Fact]
        public void Map_GivenNullOperatorAddress_PropertyShouldNotBeMapped()
        {
            var weeeSentOnList = new List<WeeeSentOnData>();

            var weeeSentOn = new WeeeSentOnData()
            {
                SiteAddress = new AatfAddressData() { Name = "SiteName", Address1 = "SiteAdd1", Address2 = "SiteAdd2", TownOrCity = "SiteTown", CountyOrRegion = "SiteCounty", Postcode = "GU22 7UT", CountryId = Guid.NewGuid(), CountryName = "Germany" }
            };

            weeeSentOnList.Add(weeeSentOn);

            var transfer = new ReturnAndAatfToSentOnSummaryListViewModelMapTransfer()
            {
                ReturnId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                AatfName = A.Dummy<string>(),
                OrganisationId = Guid.NewGuid(),
                WeeeSentOnDataItems = weeeSentOnList
            };

            var result = map.Map(transfer);

            foreach (var site in result.Sites)
            {
                site.OperatorAddressLong.Should().Be(null);
            }
        }

        [Fact]
        public void Map_GivenValidSource_LongAddressesShouldBeCorrect()
        {
            var weeeSentOnList = new List<WeeeSentOnData>();
            var operatorAddressLong = "OpName, OpAdd1, OpAdd2, OpTown, OpCounty, GU22 7UY, France";
            var siteAddressLong = "SiteName, SiteAdd1, SiteAdd2, SiteTown, SiteCounty, GU22 7UT, Germany";

            var weeeSentOn = new WeeeSentOnData()
            {
                OperatorAddress = new AatfAddressData() { Name = "OpName", Address1 = "OpAdd1", Address2 = "OpAdd2", TownOrCity = "OpTown", CountyOrRegion = "OpCounty", Postcode = "GU22 7UY", CountryId = Guid.NewGuid(), CountryName = "France" },
                SiteAddress = new AatfAddressData() { Name = "SiteName", Address1 = "SiteAdd1", Address2 = "SiteAdd2", TownOrCity = "SiteTown", CountyOrRegion = "SiteCounty", Postcode = "GU22 7UT", CountryId = Guid.NewGuid(), CountryName = "Germany" }
            };

            weeeSentOnList.Add(weeeSentOn);

            var transfer = new ReturnAndAatfToSentOnSummaryListViewModelMapTransfer()
            {
                ReturnId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                AatfName = A.Dummy<string>(),
                OrganisationId = Guid.NewGuid(),
                WeeeSentOnDataItems = weeeSentOnList
            };

            var result = map.Map(transfer);

            foreach (var site in result.Sites)
            {
                site.OperatorAddressLong.Should().Be(operatorAddressLong);
                site.SiteAddressLong.Should().Be(siteAddressLong);
            }
        }
    }
}
