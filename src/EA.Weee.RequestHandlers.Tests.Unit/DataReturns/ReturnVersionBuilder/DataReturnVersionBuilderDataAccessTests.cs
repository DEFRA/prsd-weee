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
        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_PopulatesCacheWithDatabaseValues()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation("AAA", "BBB");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.Equal(1, dataAccess.GetCachedAatfDeliveryLocations().Count);
            Assert.Contains(aatfDeliveryLocationDb, dataAccess.GetCachedAatfDeliveryLocations().Values);
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_WithMatchingApprovalNumberAndFacilityName_DoesNotAddToCacheAndDatabase()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation("AAA", "BBB");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.Equal(1, dataAccess.GetCachedAatfDeliveryLocations().Count);
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

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
            Assert.Contains(result, dataAccess.GetCachedAatfDeliveryLocations().Values);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_NoMatchingFacilityName_ReturnsNewAatfDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocationDb = new AatfDeliveryLocation("AAA", "xxx");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocationDb });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
            Assert.Contains(result, dataAccess.GetCachedAatfDeliveryLocations().Values);
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

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
            Assert.Contains(result, dataAccess.GetCachedAatfDeliveryLocations().Values);
            A.CallTo(() => aatfDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_PopulatesCacheWithDatabaseValues()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation("AAA", "BBB");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.Equal(1, dataAccess.GetCachedAeDeliveryLocations().Count);
            Assert.Contains(aeDeliveryLocationDb, dataAccess.GetCachedAeDeliveryLocations().Values);
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_WithMatchingApprovalNumberAndOperatorName_DoesNotAddToCacheAndDatabase()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation("AAA", "BBB");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.Equal(1, dataAccess.GetCachedAeDeliveryLocations().Count);
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

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
            Assert.Contains(result, dataAccess.GetCachedAeDeliveryLocations().Values);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_NoMatchingOperatorName_ReturnsNewAeDeliveryLocation()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocationDb = new AeDeliveryLocation("AAA", "xxx");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocationDb });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
            Assert.Contains(result, dataAccess.GetCachedAeDeliveryLocations().Values);
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

            var dataAccess = new DataReturnVersionBuilderDataAccessExtension(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
            Assert.Contains(result, dataAccess.GetCachedAeDeliveryLocations().Values);
            A.CallTo(() => aeDeliveryLocations.Add(result))
                .MustHaveHappened();
        }

        private class DataReturnVersionBuilderDataAccessExtension : DataReturnVersionBuilderDataAccess
        {
            public DataReturnVersionBuilderDataAccessExtension(Scheme scheme, Quarter quarter, WeeeContext context)
                : base(scheme, quarter, context)
            {
            }

            public Dictionary<string, AatfDeliveryLocation> GetCachedAatfDeliveryLocations()
            {
                return CachedAatfDeliveryLocations;
            }

            public Dictionary<string, AeDeliveryLocation> GetCachedAeDeliveryLocations()
            {
                return CachedAeDeliveryLocations;
            }
        }
    }
}
