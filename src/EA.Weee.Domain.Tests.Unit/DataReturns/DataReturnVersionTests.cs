namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.Producer;
    using EA.Weee.Domain.DataReturns;
    using FakeItEasy;
    using Lookup;
    using Xunit;

    public class DataReturnVersionTests
    {
        [Fact]
        public void AddEeeOutputAmount_AddNull_ThrowsArgumentNullException()
        {
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());

            Assert.Throws<ArgumentNullException>(() => returnVersion.AddEeeOutputAmount(null));
        }

        [Fact]
        public void AddEeeOutputAmount_AddsToEeeOutputAmounts()
        {
            //Arrange
            var returnVersion = new DataReturnVersion(A.Fake<DataReturn>());

            var eeeOutputAmount = new EeeOutputAmount(A<ObligationType>._, A<Category>._, A<decimal>._, A.Fake<RegisteredProducer>(), A.Fake<DataReturnVersion>());

            //Act
            returnVersion.AddEeeOutputAmount(eeeOutputAmount);

            Assert.Contains(eeeOutputAmount, returnVersion.EeeOutputAmounts);
        }
    }
}
