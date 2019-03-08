namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Common;
    using Xunit;

    public class ReturnToNonObligatedViewModelMapTests
    {
        private readonly ReturnToNonObligatedValuesViewModelMap mapper;
        private readonly IWeeeCache cache;
        private readonly IMap<NonObligatedDataToNonObligatedValueMapTransfer, IList<NonObligatedCategoryValue>> categoryMap;
        private readonly ICategoryValueTotalCalculator calculator;

        public ReturnToNonObligatedViewModelMapTests()
        {
            cache = A.Fake<IWeeeCache>();
            categoryMap = A.Fake<IMap<NonObligatedDataToNonObligatedValueMapTransfer, IList<NonObligatedCategoryValue>>>();
            calculator = A.Fake<ICategoryValueTotalCalculator>();

            mapper = new ReturnToNonObligatedValuesViewModelMap(cache, categoryMap, calculator);
        }

        [Fact]
        public void Map_GivenNullSource_ShouldThrowArgumentNullException()
        {
            Action call = () => mapper.Map(null);

            call.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenEntityIds_IdPropertiesShouldBeSet()
        {
            var transfer = new ReturnToNonObligatedValuesViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid() };

            var result = mapper.Map(transfer);

            result.OrganisationId.Should().Be(transfer.OrganisationId);
            result.ReturnId.Should().Be(transfer.ReturnId);
        }

        [Fact]
        public void Map_GivenObligatedAndCategoryValues_ObligatedMapperShouldBeCalled()
        {
            var orgId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            var nonObligatedDataId = Guid.NewGuid();
            var transfer = new ReturnToNonObligatedValuesViewModelMapTransfer() { OrganisationId = orgId, ReturnId = returnId };

            var returnData = new ReturnData();
            var nonObDataList = new List<NonObligatedData>();
            nonObDataList.Add(new NonObligatedData(1, 10, false, nonObligatedDataId));
            returnData.NonObligatedData = nonObDataList;
            transfer.ReturnData = returnData;

            var obligatedValues = new NonObligatedCategoryValues();
            var returnList = new List<NonObligatedCategoryValue>();

            A.CallTo(() => categoryMap.Map(A<NonObligatedDataToNonObligatedValueMapTransfer>.That.Matches(c => c.NonObligatedDataValues.IsSameOrEqualTo(nonObDataList)))).Returns(returnList);

            var result = mapper.Map(transfer);

            result.CategoryValues.Should().BeEquivalentTo(returnList);
        }
    }
}
