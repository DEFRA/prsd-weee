namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin;
    using Requests.Admin.Reports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetUkNonObligatedWeeeReceivedDataCsvHandlerTests
    {
        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandlerTests_NotInternalUser_ThrowsSecurityException()
        {
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandlerTests_NoComplianceYear_ThrowsArgumentException()
        {
            int complianceYear = 0;

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetUkNonObligatedWeeeReceivedDataCsvHandlerTests_ComplianceYear_ReturnsFileContent()
        {
            int complianceYear = 2016;

            IWeeeAuthorization authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            WeeeContext context = A.Fake<WeeeContext>();
            CsvWriterFactory csvWriterFactory = A.Fake<CsvWriterFactory>();

            GetUkNonObligatedWeeeReceivedDataCsvHandler handler = new GetUkNonObligatedWeeeReceivedDataCsvHandler(authorization, context, csvWriterFactory);
            GetUkNonObligatedWeeeReceivedDataCsv request = new GetUkNonObligatedWeeeReceivedDataCsv(complianceYear);

            CSVFileData data = await handler.HandleAsync(request);

            Assert.NotEmpty(data.FileContent);
        }
    }
}
