namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Data;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.Admin.AatfReports;
    using Weee.Tests.Core;
    using Xunit;
    public class GetAllAatfSentOnDataCsvHandlerTests
    {
        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            // Arrange
            var complianceYear = 2019;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, string.Empty, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            // Arrange
            var complianceYear = 0;

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, string.Empty, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Theory]
        [InlineData(2019, "", "", null, null)]
        [InlineData(2019, "B2C", "", null, null)]
        [InlineData(2019, "", "A", null, null)]
        public async Task GetAllAatfSentOnDataCsvHandler_VariousParameters_ReturnsFileContent(int complianceYear, 
           string obligationType, string aatfName, Guid? authority, Guid? panArea)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            DataSet sentOnDataSet = CreateDummyDataSet();

            A.CallTo(() => storedProcedures
            .GetAllAatfSentOnDataCsv(complianceYear, aatfName, obligationType, authority, panArea))
            .Returns(sentOnDataSet);

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, obligationType, aatfName, authority, panArea);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_MandatoryParameters_ReturnsFileName()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            int complianceYear = 2019;

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, string.Empty, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            Assert.Contains("2019", data.FileName);
        }

        [Theory]
        [InlineData(2019, "", "")]
        [InlineData(2019, "B2C", "")]
        [InlineData(2019, "", "A")]
        public async Task GetAllAatfSentOnDataCsvHandler_StringParameters_ReturnsFileName(int complianceYear,
           string obligationType, string aatfName)
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, obligationType, aatfName, null, null);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            StringBuilder filename = new StringBuilder();
            filename.Append(complianceYear.ToString());
           
            if (aatfName != string.Empty)
            {
                filename.Append("_");
                filename.Append(aatfName);
            }
            if (obligationType != string.Empty)
            {
                filename.Append("_");
                filename.Append(obligationType);
            }
            Assert.Contains(filename.ToString(), data.FileName);
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_ReturnsEmptyFileContent()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();
            int complianceYear = 2020;

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, string.Empty, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            data.FileContent.Should().Be(string.Empty);
        }

        [Fact]
        public async Task GetAatfAeReturnDataCSVHandler_Returns_MatchingFileContent()
        {
            var complianceYear = 2019;
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var commanDataAccess = A.Fake<ICommonDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);
            
            DataSet sentOnDataSet = CreateDummyDataSet();
            
            A.CallTo(() => storedProcedures
            .GetAllAatfSentOnDataCsv(A<int>._, string.Empty, string.Empty, A.Dummy<Guid>(), A.Dummy<Guid>()))
            .Returns(sentOnDataSet);

            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, string.Empty, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            var data = await handler.HandleAsync(request);
            data.FileContent.Contains("2019,Q1,TestAatf1,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf2,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf3,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf4,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf5,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
        }

        internal DataSet CreateDummyDataSet()
        {
            DataSet sentOnDataSet = new DataSet();
            DataTable obligatedDataTable = new DataTable();
            obligatedDataTable.Columns.Add("Year");
            obligatedDataTable.Columns.Add("Quarter");
            obligatedDataTable.Columns.Add("AATF Name");
            obligatedDataTable.Columns.Add("AATF approval number");
            obligatedDataTable.Columns.Add("Submitted By");
            obligatedDataTable.Columns.Add("Date Submitted");
            obligatedDataTable.Columns.Add("Category");
            obligatedDataTable.Columns.Add("Obligation");
            obligatedDataTable.Columns.Add("Total Sent to another AATF / ATF (t)");

            for (int i = 0; i < 5; i++)
            {
                DataRow row = obligatedDataTable.NewRow();
                row[0] = 2019;
                row[1] = "Q1";
                row[2] = "TestAatf" + i;
                row[3] = "WEE/AC0005ZT/ATF";
                row[4] = "T User";
                row[5] = "24/04/2019";
                row[6] = "1. Large Household Appliances";
                row[7] = "B2C";
                row[8] = 33;
            }

            sentOnDataSet.Tables.Add(obligatedDataTable);

            DataTable addressDataTable = new DataTable();
            addressDataTable.Columns.Add("SiteOperatorId");
            addressDataTable.Columns.Add("SiteOperatorData");

            sentOnDataSet.Tables.Add(addressDataTable);
            return sentOnDataSet;
        }
    }
}
