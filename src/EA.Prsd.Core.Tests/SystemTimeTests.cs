namespace EA.Prsd.Core.Tests
{
    using System;
    using Xunit;

    public class SystemTimeTests
    {
        [Fact]
        public void TryParse_ReturnsFalseForInvalidDay()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 4, 31, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForInvalidMonth()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 13, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForInvalidYear()
        {
            DateTime date;
            bool result = SystemTime.TryParse(10000, 1, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForNonLeapYearFeb29()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 2, 29, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsTrueForLeapYearFeb29()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2016, 2, 29, out date);

            Assert.True(result);
        }

        [Fact]
        public void TryParse_ReturnsTrueForValidDate()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2000, 1, 1, out date);

            Assert.True(result);
        }

        [Fact]
        public void TryParse_DateIsMinValueForInvalidDate()
        {
            DateTime date;
            SystemTime.TryParse(2015, 13, 1, out date);

            Assert.Equal(DateTime.MinValue, date);
        }

        [Fact]
        public void TryParse_DateIsParsedForValidDate()
        {
            DateTime date;
            SystemTime.TryParse(2015, 1, 1, out date);

            Assert.Equal(new DateTime(2015, 1, 1), date);
        }

        [Fact]
        public void TryParse_ReturnsFalseForZeroYear()
        {
            DateTime date;
            bool result = SystemTime.TryParse(0, 1, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForZeroMonth()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 0, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForZeroDay()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 1, 0, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForNegativeYear()
        {
            DateTime date;
            bool result = SystemTime.TryParse(-1, 1, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForNegativeMonth()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, -1, 1, out date);

            Assert.False(result);
        }

        [Fact]
        public void TryParse_ReturnsFalseForNegativeDay()
        {
            DateTime date;
            bool result = SystemTime.TryParse(2015, 1, -1, out date);

            Assert.False(result);
        }

        [Fact]
        public void CanFreezeToSpecifiedDate()
        {
            var freezeDate = new DateTime(2015, 8, 1, 12, 15, 30);
            SystemTime.Freeze(freezeDate);

            var now = SystemTime.UtcNow;

            Assert.Equal(freezeDate, now);

            SystemTime.Unfreeze();
        }

        [Fact]
        public void CanUnfreeze()
        {
            var freezeDate = new DateTime(2015, 7, 3, 14, 25, 50);
            SystemTime.Freeze(freezeDate);

            var now = SystemTime.UtcNow;

            SystemTime.Unfreeze();

            var nowAfterUnfreeze = SystemTime.Now;

            Assert.NotEqual(now, nowAfterUnfreeze);
        }

        [Fact]
        public void CanFreeze()
        {
            SystemTime.Freeze();

            var now = SystemTime.UtcNow;

            Assert.Equal(SystemTime.UtcNow, now);

            SystemTime.Unfreeze();
        }
    }
}