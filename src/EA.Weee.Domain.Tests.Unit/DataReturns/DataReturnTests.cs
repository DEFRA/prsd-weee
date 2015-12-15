namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using Domain.Scheme;
    using FakeItEasy;
    using Xunit;

    public class DataReturnTests
    {
        [Fact]
        public void DataReturn_SchemeNotDefined_ThrowsArugmentNullException()
        {
            // Arrange            
            Quarter quarter = new Quarter(2016, QuarterType.Q1);
            
            // Assert
            Assert.Throws<ArgumentNullException>(() => new DataReturn(null, quarter));
        }

        [Fact]
        public void DataReturn_qQuarterNotDefined_ThrowsArugmentNullException()
        {
            // Arrange
            Scheme scheme = new Scheme(Guid.NewGuid());
           
            // Assert
            Assert.Throws<ArgumentNullException>(() => new DataReturn(scheme, null));
        }

        [Fact]
        public void SetCurrentDataReturnVersion_ThrowsArugmentNullException()
        {
            // Arrange
            Scheme scheme = new Scheme(Guid.NewGuid());
            Quarter quarter = new Quarter(2016, QuarterType.Q1);
            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion version = new DataReturnVersion(new DataReturn(scheme, quarter));

            // Act
            Action action = () => dataReturn.SetCurrentVersion(null);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void SetCurrentDataReturnVersion_ThrowInvalidOperationException()
        {
            // Arrange
            Scheme scheme = new Scheme(Guid.NewGuid());
            Quarter quarter = new Quarter(2016, QuarterType.Q1);
            DataReturn dataReturn = new DataReturn(scheme, quarter);
            DataReturnVersion version = new DataReturnVersion(new DataReturn(scheme, quarter));

            // Act
            Action action = () => dataReturn.SetCurrentVersion(version);

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
