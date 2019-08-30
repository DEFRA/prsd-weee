namespace EA.Prsd.Core.Tests.Domain
{
    using System;
    using System.Collections.Generic;
    using Core.Domain;
    using Xunit;

    public class EnumerationTests
    {
        private class ExampleEnumeration : Enumeration
        {
            public static readonly ExampleEnumeration A = new ExampleEnumeration(1, "A");
            public static readonly ExampleEnumeration B = new ExampleEnumeration(2, "B");
            public static readonly ExampleEnumeration C = new ExampleEnumeration(3, "C");

            protected ExampleEnumeration()
            {
            }

            private ExampleEnumeration(int value, string displayName)
                : base(value, displayName)
            {
            }
        }

        [Fact]
        public void AbsoluteDifferenceShouldReturnCorrectDifference()
        {
            int zero = Enumeration.AbsoluteDifference(ExampleEnumeration.A, ExampleEnumeration.A);
            int one  = Enumeration.AbsoluteDifference(ExampleEnumeration.B, ExampleEnumeration.A);
            int two  = Enumeration.AbsoluteDifference(ExampleEnumeration.A, ExampleEnumeration.C);

            Assert.Equal(0, zero);
            Assert.Equal(1, one);
            Assert.Equal(2, two);
        }

        [Fact]
        public void GetAllShouldReturnAllAndOnlyEnumerationValues()
        {
            IEnumerable<ExampleEnumeration> getAllResult = Enumeration.GetAll<ExampleEnumeration>();
            Assert.Equal(new[] { ExampleEnumeration.A, ExampleEnumeration.B, ExampleEnumeration.C }, getAllResult);
        }

        [Fact]
        public void ValidValueShouldReturnCorrectEnumerationValue()
        {
            ExampleEnumeration shouldBeA = Enumeration.FromValue<ExampleEnumeration>(1);
            ExampleEnumeration shouldBeB = Enumeration.FromValue<ExampleEnumeration>(2);
            ExampleEnumeration shouldBeC = Enumeration.FromValue<ExampleEnumeration>(3);

            Assert.Equal(ExampleEnumeration.A, shouldBeA);
            Assert.Equal(ExampleEnumeration.B, shouldBeB);
            Assert.Equal(ExampleEnumeration.C, shouldBeC);
        }

        [Fact]
        public void InvalidValueShouldThrowApplicationException()
        {
            Action shouldThrowApplicationException = () => Enumeration.FromValue<ExampleEnumeration>(-1);
            Assert.Throws<ApplicationException>(shouldThrowApplicationException);
        }

        [Fact]
        public void ValidDisplayNameShouldReturnCorrectEnumerationValue()
        {
            ExampleEnumeration shouldBeA = Enumeration.FromDisplayName<ExampleEnumeration>("A");
            ExampleEnumeration shouldBeB = Enumeration.FromDisplayName<ExampleEnumeration>("B");
            ExampleEnumeration shouldBeC = Enumeration.FromDisplayName<ExampleEnumeration>("C");

            Assert.Equal(ExampleEnumeration.A, shouldBeA);
            Assert.Equal(ExampleEnumeration.B, shouldBeB);
            Assert.Equal(ExampleEnumeration.C, shouldBeC);
        }

        [Fact]
        public void InvalidDisplayNameShouldThrowApplicationException()
        {
            Action shouldThrowApplicationException = () => Enumeration.FromDisplayName<ExampleEnumeration>("Some invalid display text");
            Assert.Throws<ApplicationException>(shouldThrowApplicationException);
        }
    }
}