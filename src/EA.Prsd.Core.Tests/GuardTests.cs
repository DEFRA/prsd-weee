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
            Action action = () => Guard.ArgumentNotNullOrEmpty(() => theString, theString);

            Assert.Throws<ArgumentException>("theString", action);
        }

        [Fact]
        public void NullStringThrowsArgumentNullException()
        {
            string theString = null;
            Action action = () => Guard.ArgumentNotNullOrEmpty(() => theString, theString);

            Assert.Throws<ArgumentNullException>("theString", action);
        }

        [Fact]
        public void NullObjectThrowsArgumentNullException()
        {
            object theObject = null;
            Action action = () => Guard.ArgumentNotNull(() => theObject, theObject);

            Assert.Throws<ArgumentNullException>("theObject", action);
        }

        [Fact]
        public void ZeroIntThrowsArgumentOutOfRangeException()
        {
            int theInt = 0;
            Action action = () => Guard.ArgumentNotZeroOrNegative(() => theInt, theInt);

            Assert.Throws<ArgumentOutOfRangeException>("theInt", action);
        }

        [Fact]
        public void NegativeIntThrowsArgumentOutOfRangeException()
        {
            int theInt = -1;
            Action action = () => Guard.ArgumentNotZeroOrNegative(() => theInt, theInt);

            Assert.Throws<ArgumentOutOfRangeException>("theInt", action);
        }

        [Fact]
        public void ZeroDecimalThrowsArgumentOutOfRangeException()
        {
            decimal theDecimal = 0.0M;
            Action action = () => Guard.ArgumentNotZeroOrNegative(() => theDecimal, theDecimal);

            Assert.Throws<ArgumentOutOfRangeException>("theDecimal", action);
        }

        [Fact]
        public void NegativeDecimalThrowsArgumentOutOfRangeException()
        {
            decimal theDecimal = -1.0M;
            Action action = () => Guard.ArgumentNotZeroOrNegative(() => theDecimal, theDecimal);

            Assert.Throws<ArgumentOutOfRangeException>("theDecimal", action);
        }

        [Fact]
        public void DefaultObjectThrowsArgumentException()
        {
            object theObject = default(object);
            Action action = () => Guard.ArgumentNotDefaultValue(() => theObject, theObject);

            Assert.Throws<ArgumentException>("theObject", action);
        }

        [Fact]
        public void DefaultIntThrowsArgumentException()
        {
            int theInt = default(int);
            Action action = () => Guard.ArgumentNotDefaultValue(() => theInt, theInt);

            Assert.Throws<ArgumentException>("theInt", action);
        }
    }
}