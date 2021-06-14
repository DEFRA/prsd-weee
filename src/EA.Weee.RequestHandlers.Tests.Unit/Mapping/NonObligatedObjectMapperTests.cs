namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using System;
    using System.Linq;
    using Xunit;

    public class NonObligatedObjectMapperTests
    {
        private readonly NonObligatedObjectMapper mapper = new NonObligatedObjectMapper();

        [Fact]
        public void Map_NonObligatedObjectMapper()
        {
            // Arrange
            var aatfReturn = new Return(new Organisation(), new Domain.DataReturns.Quarter(2019, Domain.DataReturns.QuarterType.Q4), "someone", FacilityType.Aatf);
            var categoryValues = new NonObligatedValue[]
            {
                new NonObligatedValue(1, 100, false, Guid.Empty),
                new NonObligatedValue(2, 200, false, Guid.Empty)
            };

            // Act
            var response = mapper.Map(categoryValues, aatfReturn);

            // Assert
            Assert.Equal(categoryValues.Length, response.Count());
            foreach (var expected in categoryValues)
            {
                var actual = response.FirstOrDefault(r => r.CategoryId == expected.CategoryId);
                Assert.NotNull(actual);
                Assert.Equal(expected.Tonnage, actual.Tonnage);
                Assert.Equal(expected.Dcf, actual.Dcf);
                Assert.Equal(aatfReturn, actual.Return);
            }
        }
    }
}
