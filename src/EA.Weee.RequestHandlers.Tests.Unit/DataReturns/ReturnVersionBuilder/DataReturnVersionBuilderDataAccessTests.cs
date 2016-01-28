namespace EA.Weee.RequestHandlers.Tests.Unit.DataReturns.ReturnVersionBuilder
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
        public async Task GetOrAddAatfDeliveryLocation_RetrievesAatfDeliveryLocation_FromLocalCollection()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation>());
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            var aatfDeliveryLocation = new AatfDeliveryLocation("AAA", "BBB");
            A.CallTo(() => aatfDeliveryLocations.Local)
                .Returns(new ObservableCollection<AatfDeliveryLocation> { aatfDeliveryLocation });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Same(aatfDeliveryLocation, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
        }

        [Fact]
        public async Task GetOrAddAatfDeliveryLocation_RetrievesAatfDeliveryLocation_FromDatabaseWhenNotAvailableFromLocalCollection()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aatfDeliveryLocation = new AatfDeliveryLocation("AAA", "BBB");
            var aatfDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AatfDeliveryLocation> { aatfDeliveryLocation });
            A.CallTo(() => context.AatfDeliveryLocations)
                .Returns(aatfDeliveryLocations);

            A.CallTo(() => aatfDeliveryLocations.Local)
                .Returns(new ObservableCollection<AatfDeliveryLocation>());

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Same(aatfDeliveryLocation, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
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

            var aatfDeliveryLocationLocal = new AatfDeliveryLocation("zzz", "BBB");
            A.CallTo(() => aatfDeliveryLocations.Local)
                .Returns(new ObservableCollection<AatfDeliveryLocation> { aatfDeliveryLocationLocal });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.NotSame(aatfDeliveryLocationLocal, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
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

            var aatfDeliveryLocationLocal = new AatfDeliveryLocation("AAA", "zzz");
            A.CallTo(() => aatfDeliveryLocations.Local)
                .Returns(new ObservableCollection<AatfDeliveryLocation> { aatfDeliveryLocationLocal });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aatfDeliveryLocationDb, result);
            Assert.NotSame(aatfDeliveryLocationLocal, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
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

            A.CallTo(() => aatfDeliveryLocations.Local)
                .Returns(new ObservableCollection<AatfDeliveryLocation>());

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAatfDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.FacilityName);
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_RetrievesAeDeliveryLocation_FromLocalCollection()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation>());
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            var aeDeliveryLocation = new AeDeliveryLocation("AAA", "BBB");
            A.CallTo(() => aeDeliveryLocations.Local)
                .Returns(new ObservableCollection<AeDeliveryLocation> { aeDeliveryLocation });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Same(aeDeliveryLocation, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
        }

        [Fact]
        public async Task GetOrAddAeDeliveryLocation_RetrievesAeDeliveryLocation_FromDatabaseWhenNotAvailableFromLocalCollection()
        {
            // Arrange
            var dbContextHelper = new DbContextHelper();
            var context = A.Fake<WeeeContext>();

            var aeDeliveryLocation = new AeDeliveryLocation("AAA", "BBB");
            var aeDeliveryLocations = dbContextHelper.GetAsyncEnabledDbSet(new List<AeDeliveryLocation> { aeDeliveryLocation });
            A.CallTo(() => context.AeDeliveryLocations)
                .Returns(aeDeliveryLocations);

            A.CallTo(() => aeDeliveryLocations.Local)
                .Returns(new ObservableCollection<AeDeliveryLocation>());

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Same(aeDeliveryLocation, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
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

            var aeDeliveryLocationLocal = new AeDeliveryLocation("zzz", "BBB");
            A.CallTo(() => aeDeliveryLocations.Local)
                .Returns(new ObservableCollection<AeDeliveryLocation> { aeDeliveryLocationLocal });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.NotSame(aeDeliveryLocationLocal, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
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

            var aeDeliveryLocationLocal = new AeDeliveryLocation("AAA", "zzz");
            A.CallTo(() => aeDeliveryLocations.Local)
                .Returns(new ObservableCollection<AeDeliveryLocation> { aeDeliveryLocationLocal });

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(aeDeliveryLocationDb, result);
            Assert.NotSame(aeDeliveryLocationLocal, result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
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

            A.CallTo(() => aeDeliveryLocations.Local)
                .Returns(new ObservableCollection<AeDeliveryLocation>());

            var dataAccess = new DataReturnVersionBuilderDataAccess(A.Dummy<Scheme>(), A.Dummy<Quarter>(), context);

            // Act
            var result = await dataAccess.GetOrAddAeDeliveryLocation("AAA", "BBB");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("AAA", result.ApprovalNumber);
            Assert.Equal("BBB", result.OperatorName);
        }
    }
}
