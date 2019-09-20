namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.Admin.AatfReports;
    using System;
    using System.Data;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain;
    using Domain.Lookup;
    using Prsd.Core;
    using Weee.Tests.Core;
    using Xunit;
    public class GetAllAatfSentOnDataCsvHandlerTests
    {
        private const int ComplianceYear = 2019;
        private readonly WeeeContext context;
        private readonly GetAllAatfSentOnDataCsvHandler handler;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IStoredProcedures storedProcedures;
        private readonly Fixture fixture;

        public GetAllAatfSentOnDataCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            storedProcedures = A.Fake<IStoredProcedures>();
            fixture = new Fixture();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            handler = new GetAllAatfSentOnDataCsvHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(), context, commonDataAccess);
        }
        
        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            var handler = new GetAllAatfSentOnDataCsvHandler(authorization, context, commonDataAccess);
            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, string.Empty, null, null);

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

            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, null, null);

            // Act
            Func<Task> action = async () => await handler.HandleAsync(request);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Theory]
        [InlineData(2019, "", null, null)]
        [InlineData(2019, "B2C", null, null)]
        public async Task GetAllAatfSentOnDataCsvHandler_VariousParameters_ReturnsFileContent(int complianceYear,
           string obligationType, Guid? authority, Guid? panArea)
        {
            var sentOnDataSet = CreateDummyDataSet();

            A.CallTo(() => storedProcedures
                .GetAllAatfSentOnDataCsv(complianceYear, obligationType, authority, panArea))
                .Returns(sentOnDataSet);

            var request = new GetAllAatfSentOnDataCsv(complianceYear, obligationType, authority, panArea);

            // Act
            var data = await handler.HandleAsync(request);

            // Assert
            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_MandatoryParameters_ReturnsFileName()
        {
            SystemTime.Freeze(new DateTime(2019, 8, 27, 11, 30, 1));

            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, string.Empty, null, null);

            // Act
            var data = await handler.HandleAsync(request);

            // Assert
            data.FileName.Should().Be("2019_AATF WEEE sent on for treatment_27082019_1130.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_GivenObligationTypeParameter_FileNameIsCorrect()
        {
            SystemTime.Freeze(new DateTime(2019, 8, 27, 11, 30, 1));

            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, "B2B", null, null);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_B2B_AATF WEEE sent on for treatment_27082019_1130.csv");
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_GivenAuthorityParameter_FileNameIsCorrect()
        {
            SystemTime.Freeze(new DateTime(2019, 8, 27, 11, 30, 1));

            var authority = fixture.Create<UKCompetentAuthority>();

            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, null, authority.Id, null);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(authority.Id)).Returns(authority);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"2019_{authority.Abbreviation}_AATF WEEE sent on for treatment_27082019_1130.csv");
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_GivenAreaParameter_FileNameIsCorrect()
        {
            SystemTime.Freeze(new DateTime(2019, 8, 27, 11, 30, 1));

            var panArea = fixture.Create<PanArea>();

            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, null, null, panArea.Id);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(panArea.Id)).Returns(panArea);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"2019_{panArea.Name}_AATF WEEE sent on for treatment_27082019_1130.csv");
        }

        [Fact]
        public async Task GetAllAatfSentOnDataCsvHandler_GivenAllNonMandatoryParameter_FileNameIsCorrect()
        {
            SystemTime.Freeze(new DateTime(2019, 8, 27, 11, 30, 1));

            var panArea = fixture.Create<PanArea>();
            var authority = fixture.Create<UKCompetentAuthority>();

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(panArea.Id)).Returns(panArea);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(authority.Id)).Returns(authority);

            var request = new GetAllAatfSentOnDataCsv(ComplianceYear, "B2C", authority.Id, panArea.Id);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"2019_{authority.Abbreviation}_{panArea.Name}_B2C_AATF WEEE sent on for treatment_27082019_1130.csv");
        }

        [Fact]
        public async Task GetAatfAeReturnDataCSVHandler_Returns_MatchingFileContent()
        {
            const int complianceYear = 2019;

            var sentOnDataSet = CreateDummyDataSet();

            A.CallTo(() => storedProcedures.GetAllAatfSentOnDataCsv(A<int>._, A<string>._, A<Guid>._, A<Guid>._)).Returns(sentOnDataSet);

            var request = new GetAllAatfSentOnDataCsv(complianceYear, string.Empty, A.Dummy<Guid>(), A.Dummy<Guid>());

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().Contain("Year,Quarter,AATF Name,AATF approval number,Submitted By,Date Submitted,Category,Obligation,Total Sent to another AATF / ATF (t)");
            data.FileContent.Should().Contain("2019,Q1,TestAatf0,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33");
            data.FileContent.Should().Contain("2019,Q1,TestAatf1,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33");
            data.FileContent.Should().Contain("2019,Q1,TestAatf2,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33");
            data.FileContent.Should().Contain("2019,Q1,TestAatf3,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33");
            data.FileContent.Should().Contain("2019,Q1,TestAatf4,WEE/AC0005ZT/ATF,T User,24/04/2019,1. Large Household Appliances,B2C,33");
        }

        internal DataSet CreateDummyDataSet()
        {
            var sentOnDataSet = new DataSet();
            var obligatedDataTable = new DataTable();
            obligatedDataTable.Columns.Add("Year");
            obligatedDataTable.Columns.Add("Quarter");
            obligatedDataTable.Columns.Add("AATF Name");
            obligatedDataTable.Columns.Add("AATF approval number");
            obligatedDataTable.Columns.Add("Submitted By");
            obligatedDataTable.Columns.Add("Date Submitted");
            obligatedDataTable.Columns.Add("Category");
            obligatedDataTable.Columns.Add("Obligation");
            obligatedDataTable.Columns.Add("Total Sent to another AATF / ATF (t)");

            for (var i = 0; i < 5; i++)
            {
                var row = obligatedDataTable.NewRow();
                row[0] = 2019;
                row[1] = "Q1";
                row[2] = "TestAatf" + i;
                row[3] = "WEE/AC0005ZT/ATF";
                row[4] = "T User";
                row[5] = "24/04/2019";
                row[6] = "1. Large Household Appliances";
                row[7] = "B2C";
                row[8] = 33;
                obligatedDataTable.Rows.Add(row);
            }

            sentOnDataSet.Tables.Add(obligatedDataTable);

            var addressDataTable = new DataTable();
            addressDataTable.Columns.Add("SiteOperatorId");
            addressDataTable.Columns.Add("SiteOperatorData");

            sentOnDataSet.Tables.Add(addressDataTable);

            return sentOnDataSet;
        }
    }
}
