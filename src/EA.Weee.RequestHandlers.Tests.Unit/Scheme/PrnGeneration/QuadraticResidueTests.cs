namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.PrnGeneration
{
    using Core.Helpers.PrnGeneration;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class QuadraticResidueTests
    {
        [Fact]
        public void QuadraticResidue_PassedManyValuesInValidRange_DoesNotRepeatItself()
        {
            const int StartValue = 0;
            const int Prime = 9967;

            var results = new HashSet<int>();

            for (int ii = StartValue; ii < Prime; ii++)
            {
                Assert.False(results.Contains(ii), string.Format("Value {0} appeared more than once, but isn't supposed to!", ii));
                results.Add(ii);
            }
        }

        [Theory]
        [InlineData(0, 11, 0)]
        [InlineData(1, 11, 1)]
        [InlineData(2, 11, 4)]
        [InlineData(3, 11, 9)]
        [InlineData(4, 11, 5)]
        [InlineData(5, 11, 3)]
        [InlineData(6, 11, 8)]
        [InlineData(7, 11, 6)]
        [InlineData(8, 11, 2)]
        [InlineData(9, 11, 7)]
        [InlineData(10, 11, 10)]
        public void QuadraticResidue_PassedValueValue_ReturnsExpectedValues(int x, int prime, int expectedResult)
        {
            var quadraticResidueHelper = new QuadraticResidueHelper();

            var result = quadraticResidueHelper.QuadraticResidue(x, prime);

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void QuadraticResidue_PassedNegativeX_ThrowsArgumentOutOfRangeException()
        {
            const int X = -1;
            const int Prime = 11;

            var quadraticResidueHelper = new QuadraticResidueHelper();

            Assert.Throws<ArgumentOutOfRangeException>(() => quadraticResidueHelper.QuadraticResidue(X, Prime));
        }

        [Fact]
        public void QuadraticResidue_PassedXEqualToPrime_ThrowsArgumentOutOfRangeException()
        {
            const int X = 11;
            const int Prime = 11;

            var quadraticResidueHelper = new QuadraticResidueHelper();

            Assert.Throws<ArgumentOutOfRangeException>(() => quadraticResidueHelper.QuadraticResidue(X, Prime));
        }

        [Fact]
        public void QuadraticResidue_PassedXLargerThanPrime_ThrowsArgumentOutOfRangeException()
        {
            const int X = 12;
            const int Prime = 11;

            var quadraticResidueHelper = new QuadraticResidueHelper();

            Assert.Throws<ArgumentOutOfRangeException>(() => quadraticResidueHelper.QuadraticResidue(X, Prime));
        }

        [Fact]
        public void QuadraticResidue_PassedPrimeThatDoesNotFulfilConstraints_ThrowsArgumentOutOfRangeException()
        {
            const int X = 1;
            const int Prime = 12; // (constraint is: (prime - 3) % 4 == 0, so 12 fails)

            var quadraticResidueHelper = new QuadraticResidueHelper();

            Assert.Throws<ArgumentOutOfRangeException>(() => quadraticResidueHelper.QuadraticResidue(X, Prime));
        }
    }
}
