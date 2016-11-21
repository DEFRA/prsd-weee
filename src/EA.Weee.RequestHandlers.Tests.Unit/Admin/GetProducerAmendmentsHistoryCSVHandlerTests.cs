namespace EA.Weee.RequestHandlers.Tests.Unit.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using FakeItEasy;
    using RequestHandlers.Admin;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin;
    using Weee.Tests.Core;
    using Xunit;
    
    public class GetProducerAmendmentsHistoryCSVHandlerTests
    {
        [Fact]
        public async Task GetProducerAmendmentsHistoryCSVHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var prn = "WEE/MM0001AA";

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetProducerAmendmentsHistoryCSVHandler_PRNIsNullOrEmpty_ThrowsArgumentException(string prn)
        {
            // Arrange
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetProducerAmendmentsHistoryCSVHandler_ReturnsFileContent()
        {
            // Arrange
            var prn = "WEE/MM0001DD";

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV(prn);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetProducerAmendmentsHistoryCSVHandler_WithBrandNamesLongerThanMaxLength_ThrowsException()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var storedProcedures = A.Fake<IStoredProcedures>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            var csvData1 = new ProducerAmendmentsHistoryCSVData
            {
                ProducerName = "Producer1",
                DateRegistered = new DateTime(2000, 3, 2),
                BrandNames = new string('A', GetMembersDetailsCsvHandler.MaxBrandNamesLength + 1)
            };

            var csvData2 = new ProducerAmendmentsHistoryCSVData
            {
                ProducerName = "Producer1",
                DateRegistered = new DateTime(2001, 7, 12),
                BrandNames = "BrandName2"
            };

            var csvData3 = new ProducerAmendmentsHistoryCSVData
            {
                ProducerName = "Producer1",
                DateRegistered = new DateTime(2002, 8, 5),
                BrandNames = new string('A', GetMembersDetailsCsvHandler.MaxBrandNamesLength + 1)
            };

            A.CallTo(() => storedProcedures
            .SpgProducerAmendmentsCSVDataByPRN(A<string>._))
            .Returns(new List<ProducerAmendmentsHistoryCSVData> { csvData1, csvData2, csvData3 });

            var handler = new GetProducerAmendmentsHistoryCSVHandler(authorization, context, csvWriterFactory);
            var request = new GetProducerAmendmentsHistoryCSV("PRN");

            // Act
            var exception = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            // Assert
            Assert.NotNull(exception);
            Assert.Contains("02/03/2000", exception.Message);
            Assert.Contains("05/08/2002", exception.Message);
            Assert.Contains("brand names", exception.Message);
        }
    }
}
