namespace EA.Prsd.Core.Tests
{
    using System;
    using Xunit;

    public class GuardTests
    {
        [Fact]
        public void EmptyStringThrowsArgumentException()
        {
            string theString = string.Empty;
            void action() => Guard.ArgumentNotNullOrEmpty(() => theString, theString);

            Assert.Throws<ArgumentException>("theString", action);
        }

        [Fact]
        public void NullStringThrowsArgumentNullException()
        {
            string theString = null;
            void action() => Guard.ArgumentNotNullOrEmpty(() => theString, theString);

            Assert.Throws<ArgumentNullException>("theString", action);
        }

        [Fact]
        public void NullObjectThrowsArgumentNullException()
        {
            object theObject = null;
            void action() => Guard.ArgumentNotNull(() => theObject, theObject);

            Assert.Throws<ArgumentNullException>("theObject", action);
        }

        [Fact]
        public void ZeroIntThrowsArgumentOutOfRangeException()
        {
            int theInt = 0;
            void action() => Guard.ArgumentNotZeroOrNegative(() => theInt, theInt);

            Assert.Throws<ArgumentOutOfRangeException>("theInt", action);
        }

        [Fact]
        public void NegativeIntThrowsArgumentOutOfRangeException()
        {
            int theInt = -1;
            void action() => Guard.ArgumentNotZeroOrNegative(() => theInt, theInt);

            Assert.Throws<ArgumentOutOfRangeException>("theInt", action);
        }

        [Fact]
        public void ZeroDecimalThrowsArgumentOutOfRangeException()
        {
            decimal theDecimal = 0.0M;
            void action() => Guard.ArgumentNotZeroOrNegative(() => theDecimal, theDecimal);

            Assert.Throws<ArgumentOutOfRangeException>("theDecimal", action);
        }

        [Fact]
        public void NegativeDecimalThrowsArgumentOutOfRangeException()
        {
            decimal theDecimal = -1.0M;
            void action() => Guard.ArgumentNotZeroOrNegative(() => theDecimal, theDecimal);

            Assert.Throws<ArgumentOutOfRangeException>("theDecimal", action);
        }

        [Fact]
        public void DefaultObjectThrowsArgumentException()
        {
            object theObject = default(object);
            void action() => Guard.ArgumentNotDefaultValue(() => theObject, theObject);

            Assert.Throws<ArgumentException>("theObject", action);
        }

        [Fact]
        public void DefaultIntThrowsArgumentException()
        {
            int theInt = default(int);
            void action() => Guard.ArgumentNotDefaultValue(() => theInt, theInt);

            Assert.Throws<ArgumentException>("theInt", action);
        }
    }
}