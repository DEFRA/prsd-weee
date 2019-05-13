namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ReturnsToReturnsViewModelMapTests
    {
        private readonly IMap<ReturnData, ReturnsItemViewModel> returnItemViewModelMap;
        private readonly IReturnsOrdering ordering;

        private readonly ReturnsToReturnsViewModelMap returnsMap;

        public ReturnsToReturnsViewModelMapTests()
        {
            returnItemViewModelMap = A.Fake<IMap<ReturnData, ReturnsItemViewModel>>();
            ordering = A.Fake<IReturnsOrdering>();

            returnsMap = new ReturnsToReturnsViewModelMap(ordering, returnItemViewModelMap);
        }

        [Fact]
        public void Map_GivenMappedReturns_ModelShouldBeReturned()
        {
            var returnsItems = A.CollectionOfFake<ReturnsItemViewModel>(2).ToArray();

            var returnData = new List<ReturnData>()
            {
                new ReturnData()
                {
                    Quarter = new Quarter(2019, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1))
                },
                new ReturnData()
                {
                    Quarter = new Quarter(2020, QuarterType.Q1), QuarterWindow = new QuarterWindow(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1))
                }
            };

            A.CallTo(() => ordering.Order(returnData)).Returns(returnData);
            A.CallTo(() => returnItemViewModelMap.Map(A<ReturnData>._)).ReturnsNextFromSequence(returnsItems);
            
            var result = returnsMap.Map(returnData);

            result.Returns.Should().Contain(returnsItems.ElementAt(0));
            result.Returns.Should().Contain(returnsItems.ElementAt(1));
            result.Returns.Count().Should().Be(2);
        }
    }
}
