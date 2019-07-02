namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.Dsl;
    using Core.DataReturns;
    using Xunit;

    public class ReturnAndAatfToSentOnCreateSiteViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnCreateSiteViewModelMap map;
        private readonly Fixture fixture;

        public ReturnAndAatfToSentOnCreateSiteViewModelMapTests()
        {
            fixture = new Fixture();
            map = new ReturnAndAatfToSentOnCreateSiteViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenReturn_DefaultPropertiesShouldBeMapped()
        {
            var source = CreateDefaultTransferObject().Without(s => s.WeeeSentOnData).Create();

            var result = map.Map(source);

            result.AatfId.Should().Be(source.AatfId);
            result.ReturnId.Should().Be(source.Return.Id);
            result.OrganisationId.Should().Be(source.Return.OrganisationData.Id);
        }

        [Fact]
        public void Map_GivenNullWeeeSentOn_SiteAddressShouldNotBeNullAndContainCountryData()
        {
            var source = CreateDefaultTransferObject().Without(s => s.WeeeSentOnData).Create();
            
            var result = map.Map(source);

            result.SiteAddressData.Should().NotBeNull();
            result.SiteAddressData.Countries.Should().BeEquivalentTo(source.CountryData);
            result.SiteAddressData.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Map_GivenNullWeeeSentOn_OperatorAddressShouldNotBeNullAndContainCountryData()
        {
            var source = CreateDefaultTransferObject().Without(s => s.WeeeSentOnData).Create();

            var result = map.Map(source);

            result.OperatorAddressData.Should().NotBeNull();
            result.OperatorAddressData.Countries.Should().BeEquivalentTo(source.CountryData);
            result.OperatorAddressData.Id.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Map_GivenWeeeSentOn_OperatorAddressShouldBeMapped()
        {
            var source = CreateDefaultTransferObject().Create();

            var result = map.Map(source);

            result.OperatorAddressData.Id.Should().Be(source.WeeeSentOnData.OperatorAddressId);
            result.OperatorAddressData.Name.Should().Be(source.WeeeSentOnData.OperatorAddress.Name);
            result.OperatorAddressData.Address1.Should().Be(source.WeeeSentOnData.OperatorAddress.Address1);
            result.OperatorAddressData.Address2.Should().Be(source.WeeeSentOnData.OperatorAddress.Address2);
            result.OperatorAddressData.TownOrCity.Should().Be(source.WeeeSentOnData.OperatorAddress.TownOrCity);
            result.OperatorAddressData.CountyOrRegion.Should().Be(source.WeeeSentOnData.OperatorAddress.CountyOrRegion);
            result.OperatorAddressData.Postcode.Should().Be(source.WeeeSentOnData.OperatorAddress.Postcode);
            result.OperatorAddressData.CountryId.Should().Be(source.WeeeSentOnData.OperatorAddress.CountryId);
            result.OperatorAddressData.CountryName.Should().Be(source.WeeeSentOnData.OperatorAddress.CountryName);
        }

        [Fact]
        public void Map_GivenWeeeSentOn_SiteAddressShouldBeMapped()
        {
            var source = CreateDefaultTransferObject().Create();

            var result = map.Map(source);

            result.SiteAddressData.Id.Should().Be(source.WeeeSentOnData.SiteAddressId);
            result.SiteAddressData.Name.Should().Be(source.WeeeSentOnData.SiteAddress.Name);
            result.SiteAddressData.Address1.Should().Be(source.WeeeSentOnData.SiteAddress.Address1);
            result.SiteAddressData.Address2.Should().Be(source.WeeeSentOnData.SiteAddress.Address2);
            result.SiteAddressData.TownOrCity.Should().Be(source.WeeeSentOnData.SiteAddress.TownOrCity);
            result.SiteAddressData.CountyOrRegion.Should().Be(source.WeeeSentOnData.SiteAddress.CountyOrRegion);
            result.SiteAddressData.Postcode.Should().Be(source.WeeeSentOnData.SiteAddress.Postcode);
            result.SiteAddressData.CountryId.Should().Be(source.WeeeSentOnData.SiteAddress.CountryId);
            result.SiteAddressData.CountryName.Should().Be(source.WeeeSentOnData.SiteAddress.CountryName);
        }

        private IPostprocessComposer<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer> CreateDefaultTransferObject()
        {
            var returnData = fixture.Build<ReturnData>()
                .With(r => r.Quarter, new Quarter(2019, QuarterType.Q1))
                .Create();

            var source = fixture.Build<ReturnAndAatfToSentOnCreateSiteViewModelMapTransfer>()
                .With(r => r.Return, returnData);

            return source;
        }
    }
}
