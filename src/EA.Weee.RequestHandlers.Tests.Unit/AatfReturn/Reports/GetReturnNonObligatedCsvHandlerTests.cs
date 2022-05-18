namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Shared;
    using DataAccess;
    using DataAccess.DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.AatfReturn;
    using Domain.DataReturns;
    using Domain.Organisation;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.AatfReturn.Reports;
    using RequestHandlers.AatfReturn.Specification;
    using Requests.AatfReturn.Reports;
    using Weee.Tests.Core;
    using Xunit;
    using ReturnReportOn = Domain.AatfReturn.ReturnReportOn;

    public class GetReturnNonObligatedCsvHandlerTests
    {
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly GetReturnNonObligatedCsvHandler handler;
        private readonly IGenericDataAccess dataAccess;
        private readonly IStoredProcedures storedProcedures;
        private readonly Fixture fixture;
        
        public GetReturnNonObligatedCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            dataAccess = A.Fake<IGenericDataAccess>();
            storedProcedures = A.Fake<IStoredProcedures>();
            fixture = new Fixture();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            handler = new GetReturnNonObligatedCsvHandler(authorization, context, dataAccess, csvWriterFactory);
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_SecurityExceptionExpected()
        {
            var handler = new GetReturnNonObligatedCsvHandler(new AuthorizationBuilder().DenyOrganisationAccess().Build(), context, dataAccess, csvWriterFactory);

            var returnId = SetupReturn();

            var result = await Xunit.Record.ExceptionAsync(() => handler.HandleAsync(new GetReturnNonObligatedCsv(returnId)));

            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_StoredProcedureShouldBeCalled()
        {
            var returnId = SetupReturn();

            await handler.HandleAsync(new GetReturnNonObligatedCsv(returnId));

            A.CallTo(() => storedProcedures.GetReturnNonObligatedCsvData(returnId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenReturn_ReturnReportsOnShouldBeRetrieved()
        {
            var returnId = SetupReturn();

            await handler.HandleAsync(new GetReturnNonObligatedCsv(returnId));

            A.CallTo(() => dataAccess.GetManyByExpression(A<ReturnReportOnByReturnIdSpecification>.That.Matches(r => r.ReturnId == @returnId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenStoredProcedureReturnData_FileContentShouldMatch()
        {
            var returnId = SetupReturn();
            var csvData1 = fixture.Create<NonObligatedWeeeReceivedCsvData>();
            var csvData2 = fixture.Create<NonObligatedWeeeReceivedCsvData>();

            A.CallTo(() => storedProcedures.GetReturnNonObligatedCsvData(returnId)).Returns(new List<NonObligatedWeeeReceivedCsvData> { csvData1, csvData2 });
            A.CallTo(() => dataAccess.GetManyByExpression(A<ReturnReportOnByReturnIdSpecification>.That.Matches(r => r.ReturnId == returnId)))
                .Returns(new List<ReturnReportOn>() { new ReturnReportOn(returnId, (int)ReportOnQuestionEnum.NonObligatedDcf) });

            var result = await handler.HandleAsync(new GetReturnNonObligatedCsv(returnId));

            result.FileContent.Should()
                .Contain(
                    "Compliance year,Quarter,Submitted by,Submitted date (GMT),Name of operator,Category,Total non-obligated WEEE received (t),Non-obligated WEEE kept / retained by DCFs (t)");
            result.FileContent.Should().Contain($"{csvData1.Year},{csvData1.Quarter},{csvData1.SubmittedBy},{csvData1.SubmittedDate},{csvData1.OrganisationName},{csvData1.Category},{csvData1.TotalNonObligatedWeeeReceived},{csvData1.TotalNonObligatedWeeeReceivedFromDcf}");
            result.FileContent.Should().Contain($"{csvData2.Year},{csvData2.Quarter},{csvData2.SubmittedBy},{csvData2.SubmittedDate},{csvData2.OrganisationName},{csvData2.Category},{csvData2.TotalNonObligatedWeeeReceived},{csvData2.TotalNonObligatedWeeeReceivedFromDcf}");
        }

        [Fact]
        public async Task HandleAsync_GivenReturnHasNotSelectedDcfAndReturnData_FileContentShouldNotHaveDcfColumn()
        {
            var returnId = SetupReturn();
            var csvData1 = fixture.Create<NonObligatedWeeeReceivedCsvData>();
            var csvData2 = fixture.Create<NonObligatedWeeeReceivedCsvData>();

            A.CallTo(() => storedProcedures.GetReturnNonObligatedCsvData(returnId)).Returns(new List<NonObligatedWeeeReceivedCsvData> { csvData1, csvData2 });
            A.CallTo(() => dataAccess.GetManyByExpression(A<ReturnReportOnByReturnIdSpecification>.That.Matches(r => r.ReturnId == returnId)))
                .Returns(new List<ReturnReportOn>());

            var result = await handler.HandleAsync(new GetReturnNonObligatedCsv(returnId));

            result.FileContent.Should()
                .Contain(
                    "Compliance year,Quarter,Submitted by,Submitted date (GMT),Name of operator,Category,Total non-obligated WEEE received (t)");
            result.FileContent.Should().Contain($"{csvData1.Year},{csvData1.Quarter},{csvData1.SubmittedBy},{csvData1.SubmittedDate},{csvData1.OrganisationName},{csvData1.Category},{csvData1.TotalNonObligatedWeeeReceived}");
            result.FileContent.Should().Contain($"{csvData2.Year},{csvData2.Quarter},{csvData2.SubmittedBy},{csvData2.SubmittedDate},{csvData2.OrganisationName},{csvData2.Category},{csvData2.TotalNonObligatedWeeeReceived}");
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_FileNamedShouldBeCorrectFileContent()
        {
            var returnId = SetupReturn();

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));
            var result = await handler.HandleAsync(new GetReturnNonObligatedCsv(returnId));

            result.FileName.Should().Be("2019_Q1_Name_Non-obligated return data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        private Guid SetupReturn()
        {
            var returnId = Guid.NewGuid();
            var @return = A.Fake<Return>();
            var organisation = Organisation.CreatePartnership("Name");
            A.CallTo(() => @return.Quarter).Returns(new Quarter(2019, QuarterType.Q1));
            A.CallTo(() => @return.Id).Returns(returnId);
            A.CallTo(() => @return.Organisation).Returns(organisation);
            A.CallTo(() => dataAccess.GetById<Return>(returnId)).Returns(@return);
            return returnId;
        }
    }
}
