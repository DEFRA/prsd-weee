namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using Core.DataReturns;
    using FluentAssertions;
    using Web.Areas.AatfReturn.Mappings.ToViewModel;
    using Xunit;

    public class ReturnToSubmittedReturnViewModelMapTests
    {
        private readonly ReturnToSubmittedReturnViewModelMap map;

        public ReturnToSubmittedReturnViewModelMapTests()
        {
            map = new ReturnToSubmittedReturnViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action action = () => map.Map(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Maop_GivenValidSource_PropertiesShouldBeMapped()
        {
            var id = Guid.NewGuid();

            var returnData = new ReturnData() { Id = id, Quarter = new Quarter(2019, QuarterType.Q1) };

            var result = map.Map(returnData);

            int i = 10;
        }
    }
}
