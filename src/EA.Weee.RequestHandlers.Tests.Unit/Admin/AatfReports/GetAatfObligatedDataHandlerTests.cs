namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using Core.Shared;
    using DataAccess;
    using Domain.AatfReturn;
    using EA.Prsd.Core;
    using EA.Weee.Core.Admin;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Admin.GetAatfs;
    using EA.Weee.Requests.Admin.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Data;
    using System.Security;
    using System.Threading.Tasks;
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
            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));
            Aatf aatf = new Aatf("Company Aatf", A.Dummy<UKCompetentAuthority>(), "1234", Domain.AatfReturn.AatfStatus.Approved, A.Fake<Organisation>(), A.Dummy<AatfAddress>(), Domain.AatfReturn.AatfSize.Large, DateTime.Now, A.Fake<AatfContact>(), Domain.AatfReturn.FacilityType.Aatf, 2019, A.Fake<LocalArea>(), A.Fake<PanArea>());

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            A.CallTo(() => aatfDataAccess.GetAatfById(request.AatfId)).Returns(aatf);

            // Act
            CSVFileData data = await handler.HandleAsync(request);

            // Assert
            data.FileName.Should().Be("2019_Q1_1234_Full data_01022019_1101.csv");
        }

        [Fact]
        public async Task GetAatfAeReturnDataCSVHandler_Returns_MatchingFileContent()
        {
            var complianceYear = 2019;
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            var context = A.Fake<WeeeContext>();
            var aatfDataAccess = A.Fake<IGetAatfsDataAccess>();
            var csvWriterFactory = A.Fake<CsvWriterFactory>();

            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures)
                .Returns(storedProcedures);

            DataTable obligatedDataTable = CreateDummyDatatable();

            A.CallTo(() => storedProcedures
            .GetAatfObligatedCsvData(A.Dummy<Guid>(), A<int>._, A<int>._, A.Dummy<Guid>()))
            .Returns(obligatedDataTable);

            var handler = new GetAatfObligatedDataHandler(authorization, context, csvWriterFactory, aatfDataAccess);
            var request = new GetAatfObligatedData(complianceYear, 1, A.Dummy<Guid>(), A.Dummy<Guid>());

            // Act
            var data = await handler.HandleAsync(request);
            data.FileContent.Contains("2019,Q1,TestAatf1,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf2,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf3,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf4,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
            data.FileContent.Contains("2019,Q1,TestAatf5,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33,15,");
        }

        internal DataTable CreateDummyDatatable()
        {
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
            obligatedDataTable.Columns.Add("Reused as a whole appliance (t)");
            obligatedDataTable.Columns.Add("Total received from PCS (t)");

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
                row[9] = 15;
            }

            return obligatedDataTable;
        }
    }
}
