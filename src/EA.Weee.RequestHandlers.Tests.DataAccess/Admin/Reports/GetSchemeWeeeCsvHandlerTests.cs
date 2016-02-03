namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using System;
    using System.Diagnostics;
    using Core.Shared;
    using FakeItEasy;
    using Prsd.Core.Domain;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin.Reports;
    using Security;
    using Weee.DataAccess;
    using Weee.Tests.Core.Model;
    using Xunit;

    public class GetSchemeWeeeCsvHandlerTests
    {
        [Theory(Skip = "This should be used only to test the processing time of this handler when a full dataset is present")]
        [InlineData(2017, ObligationType.B2B)]
        [InlineData(2017, ObligationType.B2C)]
        [InlineData(2016, ObligationType.B2B)]
        [InlineData(2016, ObligationType.B2C)]
        public async void GivenVariousDifferingParameters_RunningTimeIsUnderTenSeconds(int complianceYear, ObligationType obligationType)
        {
            long processingTime;

            using (var databaseWrapper = new DatabaseWrapper())
            {
                var stopWatch = Stopwatch.StartNew();
                    await
                        Handler(databaseWrapper.WeeeContext)
                            .HandleAsync(new GetSchemeWeeeCsv(complianceYear, obligationType));
                stopWatch.Stop();
                processingTime = stopWatch.ElapsedMilliseconds;
            }

            Assert.InRange(processingTime, 0, 10000);
        }

        private GetSchemeWeeeCsvHandler Handler(WeeeContext context)
        {
            var userId = context.GetCurrentUser();

            var userContext = A.Fake<IUserContext>();
            A.CallTo(() => userContext.UserId)
                .Returns(Guid.Parse(userId));

            var authorization = A.Fake<IWeeeAuthorization>();

            return new GetSchemeWeeeCsvHandler(context, authorization,
                new CsvWriterFactory(new NoFormulaeExcelSanitizer()));
        }
    }
}
