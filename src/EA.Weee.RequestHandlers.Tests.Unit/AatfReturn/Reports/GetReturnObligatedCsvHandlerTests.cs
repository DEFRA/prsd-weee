namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Reports
{
    using EA.Prsd.Core;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
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
    }
}
