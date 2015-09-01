namespace EA.Weee.Domain.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    
    public class ExtensionMethodsTests
    {
        [Fact]
        public void EqualElements_NullCollections_ReturnsTrue()
        {
            List<int> a = null;
            List<int> b = null;

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_OneNullCollection_ReturnsFalse()
        {
            List<int> a = new List<int>();
            List<int> b = null;

            Assert.False(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameInstances_ReturnsTrue()
        {
            List<int> a = new List<int>();

            Assert.True(a.ElementsEqual(a));
        }

        [Fact]
        public void EqualElements_DifferentCount_ReturnsFalse()
        {
            List<int> a = new List<int> { 1 };
            List<int> b = new List<int> { 1, 2 };

            Assert.False(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameElementsSameOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 5 };

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_SameElementsDifferentOrder_ReturnsTrue()
        {
            List<int> a = new List<int> { 2, 1, 5, 3, 4 };
            List<int> b = new List<int> { 5, 3, 1, 4, 2 };

            Assert.True(a.ElementsEqual(b));
        }

        [Fact]
        public void EqualElements_DifferentElements_ReturnsFalse()
        {
            List<int> a = new List<int> { 1, 2, 3, 4, 5 };
            List<int> b = new List<int> { 1, 2, 3, 4, 10 };

            Assert.False(a.ElementsEqual(b));
        }
    }
}
