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
    using Xunit;

    public class ReturnAndAatfToSentOnRemoveSiteViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnRemoveSiteViewModelMap mapper;

        public ReturnAndAatfToSentOnRemoveSiteViewModelMapTests()
        {
            mapper = new ReturnAndAatfToSentOnRemoveSiteViewModelMap(A.Fake<IWeeeCache>(), A.Fake<ITonnageUtilities>());
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
                Tonnages = A.Fake<List<WeeeObligatedData>>()
            };
            var siteAddress = "SITE ADDRESS";
            var operatorAddress = "OPERATOR ADDRESS";

            var transfer = new ReturnAndAatfToSentOnRemoveSiteViewModelMapTransfer()
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                WeeeSentOn = weeeSentOn,
                SiteAddress = siteAddress,
                OperatorAddress = operatorAddress
            };

            var result = mapper.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.SiteAddress.Should().Be(siteAddress);
            result.OperatorAddress.Should().Be(operatorAddress);
            result.WeeeSentOn.Should().BeEquivalentTo(weeeSentOn);
        }
    }
}
