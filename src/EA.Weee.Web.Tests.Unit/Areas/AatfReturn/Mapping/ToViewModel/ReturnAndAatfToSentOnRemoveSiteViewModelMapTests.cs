namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMapTests
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly ReturnAndAatfToSentOnRemoveSiteViewModelMap mapper;

        public ReturnAndAatfToSentOnRemoveSiteViewModelMapTests()
        {
            this.tonnageUtilities = A.Fake<ITonnageUtilities>();
            mapper = new ReturnAndAatfToSentOnRemoveSiteViewModelMap(A.Fake<IWeeeCache>(), tonnageUtilities);
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
            var orgId = Guid.NewGuid();
            var aatfId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var weeeSentOn = new WeeeSentOnData()
            {
                Tonnages = new List<WeeeObligatedData>()
            };
            var siteAddress = "SITE ADDRESS";
            var operatorAddress = "OPERATOR ADDRESS";
            var obligatedTonnage = new ObligatedCategoryValue()
            {
                B2B = "20",
                B2C = "30",
                CategoryId = 1
            };

            var transfer = new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                WeeeSentOn = weeeSentOn,
                SiteAddress = siteAddress,
                OperatorAddress = operatorAddress
            };

            A.CallTo(() => tonnageUtilities.SumObligatedValues(weeeSentOn.Tonnages)).Returns(obligatedTonnage);

            var result = mapper.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.SiteAddress.Should().Be(siteAddress);
            result.OperatorAddress.Should().Be(operatorAddress);
            result.WeeeSentOn.Should().BeEquivalentTo(weeeSentOn);
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

            var transfer = new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
            {
                ReturnId = Guid.NewGuid(),
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                WeeeSentOn = new WeeeSentOnData()
                {
                    Tonnages = new List<WeeeObligatedData>()
                },
                SiteAddress = "TEST",
                OperatorAddress = "TEST"
            };

            A.CallTo(() => tonnageUtilities.SumObligatedValues(transfer.WeeeSentOn.Tonnages)).Returns(obligatedTonnage);

            var result = mapper.Map(transfer);

            result.TonnageB2B.Should().Be("-");
            result.TonnageB2C.Should().Be("-");
        }
    }
}
