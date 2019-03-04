namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class NonObligatedDataToNonObligatedValueMapTests
    {
        private readonly NonObligatedDataToNonObligatedValueMap map;

        public NonObligatedDataToNonObligatedValueMapTests()
        {
            map = new NonObligatedDataToNonObligatedValueMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
