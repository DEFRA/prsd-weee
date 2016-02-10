namespace EA.Weee.RequestHandlers.Tests.DataAccess.Admin.Reports
{
    using System;
    using System.Diagnostics;
    using Core.Shared;
    using RequestHandlers.Admin.Reports;
    using Requests.Admin.Reports;
    using Weee.Tests.Core;
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
            using (var databaseWrapper = new DatabaseWrapper())
            {
                // Arrange
                Stopwatch stopWatch = new Stopwatch();

                GetSchemeWeeeCsvHandler handler = new GetSchemeWeeeCsvHandler(
                    databaseWrapper.WeeeContext.StoredProcedures,
                    AuthorizationBuilder.CreateUserWithAllRights(),
                    new CsvWriterFactory(new NoFormulaeExcelSanitizer()));

                // Act
                GetSchemeWeeeCsv request = new GetSchemeWeeeCsv(complianceYear, null, obligationType);

                stopWatch.Start();
                await handler.HandleAsync(request);
                stopWatch.Stop();

                // Assert
                Assert.InRange(stopWatch.ElapsedMilliseconds, 0, 10000);
            }
        }
    }
}
