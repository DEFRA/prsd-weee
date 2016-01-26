namespace EA.Weee.Domain.Tests.Unit.DataReturns
{
    using System;
    using Domain.DataReturns;
    using FakeItEasy;
    using Xunit;

    public class DataReturnVersionAssociativeEntityTests
    {
        [Fact]
        public void AddDataReturnVersion_AddsToDataReturnVersions()
        {
            var dataReturnVersion = new DataReturnVersion(A.Fake<DataReturn>());

            var associativeEntity = new DataReturnVersionAssociativeEntity();
            associativeEntity.AddDataReturnVersion(dataReturnVersion);

            Assert.Contains(dataReturnVersion, associativeEntity.DataReturnVersions);
        }

        [Fact]
        public void AddDataReturnVersion_WithNullParameter_ThrowsArgumentNullException()
        {
            var associativeEntity = new DataReturnVersionAssociativeEntity();

            Assert.Throws<ArgumentNullException>(() => associativeEntity.AddDataReturnVersion(null));
        }

        [Fact]
        public void AddDataReturnVersion_ForDifferentDataReturn_ThrowsInvalidOperationException()
        {
            var dataReturnVersion1 = new DataReturnVersion(A.Fake<DataReturn>());
            var dataReturnVersion2 = new DataReturnVersion(A.Fake<DataReturn>());

            var associativeEntity = new DataReturnVersionAssociativeEntity();
            associativeEntity.AddDataReturnVersion(dataReturnVersion1);

            Assert.Throws<InvalidOperationException>(() => associativeEntity.AddDataReturnVersion(dataReturnVersion2));
        }
    }
}
