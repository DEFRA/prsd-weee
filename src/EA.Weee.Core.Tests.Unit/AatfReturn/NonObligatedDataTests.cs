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
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { 1, null, true },
                new object[] { 1, null, false },
                new object[] { 1, 2.1M, true },
                new object[] { 1, 2.1M, false },
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void NonObligatedData_PropertiesShouldBeSet(int categoryId, decimal? value, bool dcf)
        {
            var result = new NonObligatedData(categoryId, value, dcf);

            result.Tonnage.Should().Be(value);
            result.CategoryId.Should().Be(categoryId);
            result.Dcf.Should().Be(dcf);
        }
    }
}
