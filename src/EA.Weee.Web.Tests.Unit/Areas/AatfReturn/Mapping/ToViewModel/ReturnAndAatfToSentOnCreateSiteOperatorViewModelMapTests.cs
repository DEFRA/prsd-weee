namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTests
    {
        private readonly ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap map;

        public ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTests()
        {
            map = new ReturnAndAatfToSentOnCreateSiteOperatorViewModelMap(A.Fake<IWeeeCache>());
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
            var weeeSentOnId = Guid.NewGuid();
            var transfer = new ReturnAndAatfToSentOnCreateSiteOperatorViewModelMapTransfer(A.Fake<IList<Core.Shared.CountryData>>())
            {
                ReturnId = returnId,
                AatfId = aatfId,
                OrganisationId = orgId,
                WeeeSentOnId = weeeSentOnId
            };

            var result = map.Map(transfer);

            result.OrganisationId.Should().Be(orgId);
            result.ReturnId.Should().Be(returnId);
            result.AatfId.Should().Be(aatfId);
            result.WeeeSentOnId.Should().Be(weeeSentOnId);
        }
    }
}
