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
        private readonly IMapper mapper;
        private readonly IReturnsOrdering ordering;

        private readonly ReturnsToReturnsViewModelMap returnsMap;

        public ReturnsToReturnsViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();
            ordering = A.Fake<IReturnsOrdering>();

            returnsMap = new ReturnsToReturnsViewModelMap(mapper, ordering);
        }

        [Fact]
        public void Map_GivenReturnsData_ReturnDataShouldBeMapped()
        {
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

            returnsMap.Map(returnData);

            A.CallTo(() => mapper.Map<ReturnViewModel>(returnData.ElementAt(0))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => mapper.Map<ReturnViewModel>(returnData.ElementAt(1))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenMappedReturns_ModelShouldBeReturned()
        {
            var returns = A.CollectionOfFake<ReturnsItemViewModel>(2).ToArray();

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
            A.CallTo(() => mapper.Map<ReturnsItemViewModel>(A<ReturnData>._)).ReturnsNextFromSequence(returns);

            var result = returnsMap.Map(returnData);

            result.Returns.Should().Contain(returns.ElementAt(0));
            result.Returns.Should().Contain(returns.ElementAt(1));
            result.Returns.Count().Should().Be(2);
        }
    }
}
