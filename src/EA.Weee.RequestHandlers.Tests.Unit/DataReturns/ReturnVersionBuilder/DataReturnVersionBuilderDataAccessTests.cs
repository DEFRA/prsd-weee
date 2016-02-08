namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ReturnVersionBuilder
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using FakeItEasy;
    using RequestHandlers.DataReturns.ReturnVersionBuilder;
    using Weee.Tests.Core;
    using Xunit;
    using Scheme = Domain.Scheme.Scheme;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Variable name aeDeliveryLocation is valid.")]
    public class DataReturnVersionBuilderDataAccessTests
    {
        [Theory]
        [InlineData("AAA", "BBB")]
        [InlineData("AAA", "")]
        [InlineData("AAA", null)]
        public async Task GetOrAddAatfDeliveryLocation_PopulatesCacheWithDatabaseValues(string approvalNumber, string facilityName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation(approvalNumber, facilityName);
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            await dataAccess.GetOrAddAatfDeliveryLocation(approvalNumber, facilityName);

            // Assert
            Assert.Equal(1, dataAccess.CachedAatfDeliveryLocations.Count);
            Assert.Contains(aatfDeliveryLocationDb, dataAccess.CachedAatfDeliveryLocations.Values);
        }

        [Theory]
        [InlineData("AAA", "BBB")]
        [InlineData("AAA", "")]
        [InlineData("AAA", null)]
        public async Task GetOrAddAatfDeliveryLocation_WithMatchingApprovalNumberAndFacilityName_DoesNotAddToCacheAndDatabase(string approvalNumber, string facilityName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation(approvalNumber, facilityName);
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation(approvalNumber, facilityName);

            // Assert
            Assert.Equal(1, dataAccess.CachedAatfDeliveryLocations.Count);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_NoMatchingApprovalNumber_ReturnsNewAatfDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation("xxx", "BBB");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
            Assert.Contains(result, dataAccess.CachedAatfDeliveryLocations.Values);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Theory]
        [InlineData("BBB", "BBB")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public async Task GetOrAddAatfDeliveryLocation_NoMatchingFacilityName_ReturnsNewAatfDeliveryLocation(string suppliedFacilityName, string resultingFacilityName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation("AAA", "xxx");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", suppliedFacilityName);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal(resultingFacilityName, result.FacilityName);
            Assert.Contains(result, dataAccess.CachedAatfDeliveryLocations.Values);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_NoMatchingApprovalNumberAndFacilityName_ReturnsNewAatfDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation>());
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
            Assert.Contains(result, dataAccess.CachedAatfDeliveryLocations.Values);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_IgnoresCaseOfApprovalNumberAndFacilityName()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation>());
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result1 = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");
            var result2 = await dataAccess.GetOrAddAatfDeliveryLocation("aaa", "BBB");
            var result3 = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "bbb");
            var result4 = await dataAccess.GetOrAddAatfDeliveryLocation("aaa", "bbb");

            // Assert
            Assert.Contains(result1, dataAccess.CachedAatfDeliveryLocations.Values);
            Assert.Same(result1, result2);
            Assert.Same(result1, result3);
            Assert.Same(result1, result4);
            A.CallTo(() => aatfDeliveryLocations.Add(A<AatfDeliveryLocation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Theory]
        [InlineData("AAA", "BBB")]
        [InlineData("AAA", "")]
        [InlineData("AAA", null)]
        public async Task GetOrAddAeDeliveryLocation_PopulatesCacheWithDatabaseValues(string approvalNumber, string operatorName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation(approvalNumber, operatorName);
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            await dataAccess.GetOrAddAeDeliveryLocation(approvalNumber, operatorName);

            // Assert
            Assert.Equal(1, dataAccess.CachedAeDeliveryLocations.Count);
            Assert.Contains(aeDeliveryLocationDb, dataAccess.CachedAeDeliveryLocations.Values);
        }

        [Theory]
        [InlineData("AAA", "BBB")]
        [InlineData("AAA", "")]
        [InlineData("AAA", null)]
        public async Task GetOrAddAeDeliveryLocation_WithMatchingApprovalNumberAndOperatorName_DoesNotAddToCacheAndDatabase(string approvalNumber, string operatorName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation(approvalNumber, operatorName);
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation(approvalNumber, operatorName);

            // Assert
            Assert.Equal(1, dataAccess.CachedAeDeliveryLocations.Count);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_NoMatchingApprovalNumber_ReturnsNewAeDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation("xxx", "BBB");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
            Assert.Contains(result, dataAccess.CachedAeDeliveryLocations.Values);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Theory]
        [InlineData("BBB", "BBB")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public async Task GetOrAddAeDeliveryLocation_NoMatchingOperatorName_ReturnsNewAeDeliveryLocation(string suppliedOperatorName, string resultingOperatorName)
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation("AAA", "xxx");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", suppliedOperatorName);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal(resultingOperatorName, result.OperatorName);
            Assert.Contains(result, dataAccess.CachedAeDeliveryLocations.Values);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_NoMatchingApprovalNumberAndOperatorName_ReturnsNewAeDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation>());
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
            Assert.Contains(result, dataAccess.CachedAeDeliveryLocations.Values);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_IgnoresCaseOfApprovalNumberAndOperatorName()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation>());
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result1 = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");
            var result2 = await dataAccess.GetOrAddAeDeliveryLocation("aaa", "BBB");
            var result3 = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "bbb");
            var result4 = await dataAccess.GetOrAddAeDeliveryLocation("aaa", "bbb");

            // Assert
            Assert.Contains(result1, dataAccess.CachedAeDeliveryLocations.Values);
            Assert.Same(result1, result2);
            Assert.Same(result1, result3);
            Assert.Same(result1, result4);
            A.CallTo(() => aeDeliveryLocations.Add(A<AeDeliveryLocation>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
