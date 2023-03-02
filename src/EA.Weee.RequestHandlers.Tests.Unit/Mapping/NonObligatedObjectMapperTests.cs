namespace EA.Weee.RequestHandlers.Tests.Unit.Mapping
{
    using System;
    using System.Linq;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using Xunit;

    public class NonObligatedObjectMapperTests
    {
        private readonly Guid returnId = Guid.NewGuid();
        private readonly NonObligatedWeeeMap mapper = new NonObligatedWeeeMap();
        private readonly Return aatfReturn = new Return(new Organisation(), new Domain.DataReturns.Quarter(2019, Domain.DataReturns.QuarterType.Q4), "someone", FacilityType.Aatf);
        private readonly NonObligatedValue[] values = new NonObligatedValue[]
                {
                    new NonObligatedValue(1, 100, false, Guid.NewGuid()),
                    new NonObligatedValue(2, 200, false, Guid.NewGuid())
                };

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Map_AddNonObligated(bool isDcf)
        {
            // Arrange
            var request = new AddNonObligated()
            {
                CategoryValues = values,
                ReturnId = returnId,
                Dcf = isDcf,
                OrganisationId = Guid.NewGuid()
            };

            // Act
            var response = mapper.Map(request, aatfReturn);

            // Assert
            Assert.Equal(request.CategoryValues.Count(), response.Count());
            foreach (var expected in request.CategoryValues)
            {
                var actual = response.FirstOrDefault(r => r.CategoryId == expected.CategoryId);
                Assert.NotNull(actual);
                Assert.Equal(expected.Tonnage, actual.Tonnage);
                Assert.Equal(isDcf, actual.Dcf);
                Assert.Equal(aatfReturn, actual.Return);
            }
        }

        [Fact]
        public void Map_EditNonObligated()
        {
            // Arrange
            var request = new EditNonObligated()
            {
                CategoryValues = values,
                ReturnId = returnId
            };

            // Act
            var response = mapper.Map(request, aatfReturn);

            // Assert
            Assert.Equal(values.Count(), response.Count());
            foreach (var expected in request.CategoryValues)
            {
                var actual = response.FirstOrDefault(r => r.Item1 == expected.Id);
                Assert.NotNull(actual);
                Assert.Equal(expected.Tonnage, actual.Item2);
            }
        }
    }
}
