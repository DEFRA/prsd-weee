namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using Xunit;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMapTests
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly IAddressUtilities addressUtilities;
        private readonly ReturnAndAatfToSentOnRemoveSiteViewModelMap mapper;

        public ReturnAndAatfToSentOnRemoveSiteViewModelMapTests()
        {
            tonnageUtilities = A.Fake<ITonnageUtilities>();
            addressUtilities = A.Fake<IAddressUtilities>();

            mapper = new ReturnAndAatfToSentOnRemoveSiteViewModelMap(tonnageUtilities, addressUtilities);
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => mapper.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenValidSource_PropertiesShouldBeMapped()
        {
            var obligatedTonnage = new ObligatedCategoryValue()
            {
                B2B = "20",
                B2C = "30",
                CategoryId = 1
            };

            var transfer = new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
            {
                ReturnId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                WeeeSentOn = WeeeSentOn()
            };

            A.CallTo(() => tonnageUtilities.SumObligatedValues(transfer.WeeeSentOn.Tonnages)).Returns(obligatedTonnage);

            var result = mapper.Map(transfer);

            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.ReturnId.Should().Be(transfer.ReturnId);
            result.AatfId.Should().Be(transfer.AatfId);
            result.WeeeSentOn.Should().BeEquivalentTo(transfer.WeeeSentOn);
            result.TonnageB2B.Should().Be(obligatedTonnage.B2B);
            result.TonnageB2C.Should().Be(obligatedTonnage.B2C);
        }

        [Fact]
        public void Map_GivenNoTonnages_TonnagesShouldBeSetToDash()
        {
            var obligatedTonnage = new ObligatedCategoryValue()
            {
                B2B = "-",
                B2C = "-",
                CategoryId = 1
            };

            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();

            A.CallTo(() => tonnageUtilities.SumObligatedValues(transfer.WeeeSentOn.Tonnages)).Returns(obligatedTonnage);

            var result = mapper.Map(transfer);

            result.TonnageB2B.Should().Be("-");
            result.TonnageB2C.Should().Be("-");
        }

        [Fact]
        public void Map_GivenWeeeSentOnSiteAddress_SiteAddressShouldBeMapped()
        {
            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();

            var result = mapper.Map(transfer);

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.SiteAddress, true)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenWeeeSentOnSiteAddress_MappedSiteAddressShouldBeReturned()
        {
            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();
            const string address = "address";

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.SiteAddress, true)).Returns(address);

            var result = mapper.Map(transfer);

            result.SiteAddress.Should().Be(address);
        }

        [Fact]
        public void Map_GivenWeeeSentOnSiteAddress_MappedSiteAddressDataShouldBeReturned()
        {
            var siteAddressLong = "Site name<br/>Site address 1<br/>Site address 2<br/>Site town<br/>Site county<br/>GU22 7UY<br/>Site country";

            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.SiteAddress, true)).Returns(siteAddressLong);

            var result = mapper.Map(transfer);

            result.SiteAddress.Should().Be(siteAddressLong);
            result.SiteAddressData.Should().Be(transfer.WeeeSentOn.SiteAddress);
        }

        [Fact]
        public void Map_GivenWeeeSentOnOperatorAddress_OperatorAddressDataIsMapped()
        {
            var operatorAddrAddressLong = "Operator name<br/>Operator address 1<br/>Operator address 2<br/>Operator town<br/>Operator county<br/>GU22 7UY<br/>Operator country";

            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.OperatorAddress, true)).Returns(operatorAddrAddressLong);

            var result = mapper.Map(transfer);

            result.OperatorAddress.Should().Be(operatorAddrAddressLong);
            result.OperatorAddressData.Should().Be(transfer.WeeeSentOn.OperatorAddress);
        }

        [Fact]
        public void Map_GivenWeeeSentOnOperatorAddress_OperatorAddressShouldBeMapped()
        {
            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();

            var result = mapper.Map(transfer);

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.OperatorAddress, true)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenWeeeSentOnOperatorAddress_MappedOperatorAddressShouldBeReturned()
        {
            var transfer = ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer();
            const string address = "address";

            A.CallTo(() => addressUtilities.FormattedAddress(transfer.WeeeSentOn.OperatorAddress, true)).Returns(address);

            var result = mapper.Map(transfer);

            result.OperatorAddress.Should().Be(address);
        }

        private WeeeSentOnData WeeeSentOn()
        {
            var siteCoutryId = Guid.Empty;
            var operatorCoutryId = Guid.Empty;

            var siteAddress = new AatfAddressData("Site name", "Site address 1", "Site address 2", "Site town", "Site county", "GU22 7UY", siteCoutryId, "Site country");
            var operatorAddress = new AatfAddressData("Operator name", "Operator address 1", "Operator address 2", "Operator town", "Operator county", "GU22 7UY", operatorCoutryId, "Operator country");

            return new WeeeSentOnData()
            {
                Tonnages = new List<WeeeObligatedData>(),
                SiteAddress = siteAddress,
                OperatorAddress = operatorAddress,
            };
        }

        private ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
        {
            var transfer = new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
            {
                ReturnId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                WeeeSentOn = WeeeSentOn()
            };
            return transfer;
        }
    }
}
