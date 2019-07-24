namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.Core.Admin;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.Aatf;
    using FakeItEasy;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfObligatedDataHandlerTests
    {
        [Fact]
        public async Task GetAatfObligatedDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var aatfDataAccess = A.Fake<IGetAatfsDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAatfObligatedDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var aatfDataAccess = A.Fake<IGetAatfsDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Theory]
        [InlineData(2019)]
        [InlineData(2020)]
        [InlineData(2021)]
        public async Task GetAatfObligatedDataCsvHandlerr_PassVariousComplianceYears_ReturnsFileContent(int complianceYear)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var aatfDataAccess = A.Fake<IGetAatfsDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAatfObligatedDataCsvHandler_ReturnsFileName()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var aatfDataAccess = A.Fake<IGetAatfsDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            int complianceYear = 2019;

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.Contains("2019Q1", data.FileName);
        }
    }
}
