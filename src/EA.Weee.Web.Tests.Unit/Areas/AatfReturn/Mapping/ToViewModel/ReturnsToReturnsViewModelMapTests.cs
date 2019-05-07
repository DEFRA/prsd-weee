namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core.Mapper;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Web.Areas.AatfReturn.ViewModels;
    using Xunit;

    public class ReturnsToReturnsViewModelMapTests
    {
        private readonly IMapper mapper;
        private readonly ReturnsToReturnsViewModelMap returnsMap;

        public ReturnsToReturnsViewModelMapTests()
        {
            mapper = A.Fake<IMapper>();

            returnsMap = new ReturnsToReturnsViewModelMap(mapper);
        }

        [Fact]
        public void Map_GivenReturnsData_ReturnDataShouldBeMapped()
        {
            var returns = A.CollectionOfDummy<ReturnData>(2);

            returnsMap.Map(returns);

            A.CallTo(() => mapper.Map<ReturnViewModel>(returns.ElementAt(0))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => mapper.Map<ReturnViewModel>(returns.ElementAt(1))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Map_GivenMappedReturns_ModelShouldBeReturned()
        {
            var returns = A.CollectionOfFake<ReturnViewModel>(2).ToArray();
            var returnData = A.CollectionOfFake<ReturnData>(2);

            A.CallTo(() => mapper.Map<ReturnViewModel>(A<ReturnData>._)).ReturnsNextFromSequence(returns);

            var result = returnsMap.Map(returnData);

            result.Returns.Should().Contain(returns.ElementAt(0));
            result.Returns.Should().Contain(returns.ElementAt(1));
            result.Returns.Count().Should().Be(2);
        }
    }
}
