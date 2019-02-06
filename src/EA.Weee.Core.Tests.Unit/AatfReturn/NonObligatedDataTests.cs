namespace EA.Weee.Core.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.AatfReturn;
    using FluentAssertions;
    using Xunit;

    public class NonObligatedDataTests
    {
        [Theory]
        [InlineData(1, null, true)]
        [InlineData(1, null, false)]
        [InlineData(1, 1, true)]
        [InlineData(1, 1, false)]
        public void NonObligatedData_PropertiesShouldBeSet(int categoryId, decimal? value, bool dcf)
        {
            var result = new NonObligatedData(categoryId, value, dcf);

            result.Tonnage.Should().Be(value);
            result.CategoryId.Should().Be(categoryId);
            result.Dcf.Should().Be(dcf);
        }
    }
}
