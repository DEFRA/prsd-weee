namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Reports
{
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Reports;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.AatfReturn.Reports;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class GetReturnObligatedCsvHandlerTests
    {
        private readonly IWeeeAuthorization authorization;
        private readonly WeeeContext weeContext;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IReturnDataAccess returnDataAccess;
        private GetReturnObligatedCsvHandler handler;

        public GetReturnObligatedCsvHandlerTests()
        {
            this.authorization = A.Fake<IWeeeAuthorization>();
            this.weeContext = A.Fake<WeeeContext>();
            this.csvWriterFactory = A.Fake<CsvWriterFactory>();
            this.commonDataAccess = A.Fake<ICommonDataAccess>();
            this.returnDataAccess = A.Fake<IReturnDataAccess>();

            this.handler = new GetReturnObligatedCsvHandler(authorization, weeContext, csvWriterFactory, commonDataAccess, returnDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ReturnDataAccessIsCalled()
        {
            var request = new GetReturnObligatedCsv(Guid.NewGuid());

            await handler.HandleAsync(request);

            A.CallTo(() => returnDataAccess.GetById(request.ReturnId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_FileNameShouldBeCorrect()
        {
            var request = new GetReturnObligatedCsv(Guid.NewGuid());

            var @return = new Return(Organisation.CreatePartnership("trading"), new Quarter(2019, QuarterType.Q1), "created", FacilityType.Aatf);

            A.CallTo(() => returnDataAccess.GetById(request.ReturnId)).Returns(@return);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be(@return.Quarter.Year + "_" + @return.Quarter.Q + "_" + @return.Organisation.OrganisationName + "_Obligated return data_01022019_1101.csv");
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_MatchingFileContent()
        {
            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => weeContext.StoredProcedures).Returns(storedProcedures);

            DataTable obligatedDataTable = CreateDummyDatatable();

            var request = new GetReturnObligatedCsv(Guid.NewGuid());

            A.CallTo(() => storedProcedures.GetReturnObligatedCsvData(request.ReturnId)).Returns(obligatedDataTable);

            var data = await handler.HandleAsync(request);

            data.FileContent.Contains("2019,Q1,TestAatf1,WEE/AC0005ZT/ATF,T User,24/04/2019,B2C,1. Large Household Appliances,33,15,2,88");
            data.FileContent.Should().NotContain("ReturnId");
            data.FileContent.Should().NotContain("AatfKey");
        }

        internal DataTable CreateDummyDatatable()
        {
            DataTable obligatedDataTable = new DataTable();
            obligatedDataTable.Columns.Add("ComplianceYear");
            obligatedDataTable.Columns.Add("Quarter");
            obligatedDataTable.Columns.Add("AATFName");
            obligatedDataTable.Columns.Add("AATFApprovalNumber");
            obligatedDataTable.Columns.Add("SubmittedBy");
            obligatedDataTable.Columns.Add("SubmittedDate");
            obligatedDataTable.Columns.Add("Obligation");
            obligatedDataTable.Columns.Add("CategoryName");
            obligatedDataTable.Columns.Add("TotalSent");
            obligatedDataTable.Columns.Add("TotalReused");
            obligatedDataTable.Columns.Add("TotalReceived");
            obligatedDataTable.Columns.Add("Obligated WEEE sent to DSDS (t)");
            obligatedDataTable.Columns.Add("ReturnId");
            obligatedDataTable.Columns.Add("AatfKey");

            for (int i = 0; i < 5; i++)
            {
                DataRow row = obligatedDataTable.NewRow();
                row[0] = 2019;
                row[1] = "Q1";
                row[2] = "TestAatf" + i;
                row[3] = "WEE/AC0005ZT/ATF";
                row[4] = "T User";
                row[5] = "24/04/2019";
                row[6] = "B2C";
                row[7] = "1. Large Household Appliances";
                row[8] = 33;
                row[9] = 15;
                row[10] = 2;
                row[11] = 88;
            }

            return obligatedDataTable;
        }
    }
}
