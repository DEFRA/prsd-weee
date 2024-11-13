namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Core.Constants;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class GetProducerEeeDataCsvDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly IStoredProcedures storedProcedures;
        private readonly GetProducerEeeDataCsvDataAccess dataAccess;

        public GetProducerEeeDataCsvDataAccessTests()
        {
            // Arrange
            context = A.Fake<WeeeContext>();
            storedProcedures = A.Fake<IStoredProcedures>();
            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);
            dataAccess = new GetProducerEeeDataCsvDataAccess(context);
        }

        [Fact]
        public async Task GetItemsAsync_WhenCalledWithRegularScheme_ShouldCallStoredProcedureWithCorrectParameters()
        {
            // Arrange
            const int complianceYear = 2024;
            var id = Guid.NewGuid();
            const string obligationType = "B2C";
            var expectedResult = new List<ProducerEeeCsvData>
            {
                new ProducerEeeCsvData()
            };

            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                id,
                obligationType,
                false,
                false))
                .Returns(expectedResult);

            // Act
            var result = await dataAccess.GetItemsAsync(complianceYear, id, obligationType);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                id,
                obligationType,
                false,
                false))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemsAsync_WhenCalledWithDirectRegistrant_ShouldCallStoredProcedureWithNullSchemeIdAndFilterFlag()
        {
            // Arrange
            const int complianceYear = 2024;
            var directRegistrantId = DirectRegistrantFixedIdConstant.DirectRegistrantFixedId;
            const string obligationType = "B2C";
            var expectedResult = new List<ProducerEeeCsvData>
            {
                new ProducerEeeCsvData()
            };

            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                null,
                obligationType,
                true,
                false))
                .Returns(expectedResult);

            // Act
            var result = await dataAccess.GetItemsAsync(complianceYear, directRegistrantId, obligationType);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                null,
                obligationType,
                true, 
                false))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetItemsAsync_WhenCalledWithNullSchemeId_ShouldCallStoredProcedureWithNullSchemeIdAndNoFilter()
        {
            // Arrange
            const int complianceYear = 2024;
            Guid? id = null;
            const string obligationType = "B2C";
            var expectedResult = new List<ProducerEeeCsvData>
            {
                new ProducerEeeCsvData()
            };

            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                null,
                obligationType,
                false,
                false))
                .Returns(expectedResult);

            // Act
            var result = await dataAccess.GetItemsAsync(complianceYear, id, obligationType);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                null,
                obligationType,
                false,
                false))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("B2B")]
        [InlineData("B2C")]
        public async Task GetItemsAsync_ShouldAcceptDifferentObligationTypes(string obligationType)
        {
            // Arrange
            const int complianceYear = 2024;
            var id = Guid.NewGuid();
            var expectedResult = new List<ProducerEeeCsvData>
            {
                new ProducerEeeCsvData()
            };

            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                id,
                obligationType,
                false,
                false))
                .Returns(expectedResult);

            // Act
            var result = await dataAccess.GetItemsAsync(complianceYear, id, obligationType);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            A.CallTo(() => storedProcedures.SpgProducerEeeCsvData(
                complianceYear,
                id,
                obligationType,
                false,
                false))
                .MustHaveHappenedOnceExactly();
        }
    }
}