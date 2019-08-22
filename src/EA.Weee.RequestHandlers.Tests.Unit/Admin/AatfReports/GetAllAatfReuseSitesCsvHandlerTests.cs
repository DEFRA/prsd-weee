namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using Core.Shared;
    using DataAccess;
    using EA.Prsd.Core;
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Shared;
    using FakeItEasy;
    using FluentAssertions;
    using Requests.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAllAatfReuseSitesCsvHandlerTests
    {
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly GetAllAatfReuseSitesCsvHandler handler;
        private readonly ICommonDataAccess commanDataAccess;

        public GetAllAatfReuseSitesCsvHandlerTests()
        {
            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            commanDataAccess = A.Fake<ICommonDataAccess>();

            handler = new GetAllAatfReuseSitesCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
        }

        [Fact]
        public async Task GetAllAatfReuseSitesCsvHandler_NotInternalUser_ThrowsSecurityException()
        {
            const int complianceYear = 2016;

            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAllAatfReuseSitesCsvHandler(authorization, context, csvWriterFactory, commanDataAccess);
            var request = new GetAllAatfReuseSitesCsv(complianceYear, null, null);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task GetAllAatfReuseSitesCsvHandler_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            var request = new GetAllAatfReuseSitesCsv(complianceYear, null, null);

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task GetAllAatfReuseSitesCsvHandler_ComplianceYear_ReturnsFileContent()
        {
            const int complianceYear = 2016;

            var request = new GetAllAatfReuseSitesCsv(complianceYear, null, null);

            var data = await handler.HandleAsync(request);

            Assert.NotEmpty(data.FileContent);
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetAllAatfReuseSitesCsv(2019, null, null);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_AATF using reuse sites_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_MatchingFileContent()
        {
            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            var csvData1 = new AatfReuseSitesData()
            {
                ComplianceYear = 2019,
                Quarter = "Q1",
                SubmittedBy = "Test User",
                SubmittedDate = new DateTime(2019, 2, 1, 11, 1, 2),
                OrgName = "Org 1",
                Abbreviation = "EA",
                PanName = "North",
                LaName = "Surrey",
                SiteName = "Test Site",
                SiteAddress = "1 address, address2, town, county, UK-England"
            };
            var csvData2 = new AatfReuseSitesData()
            {
                ComplianceYear = 2019,
                Quarter = "Q1",
                SubmittedBy = "Test User",
                SubmittedDate = new DateTime(2019, 2, 1, 11, 1, 2),
                OrgName = "Org 1",
                Abbreviation = "EA",
                PanName = "North",
                LaName = "Surrey",
                SiteName = "Test Site 2",
                SiteAddress = "1 address, address2, town, county, UK-England"
            };

            var request = new GetAllAatfReuseSitesCsv(2019, null, null);

            A.CallTo(() => storedProcedures.GetAllAatfReuseSitesCsvData(request.ComplianceYear, request.AuthorityId, request.PanArea)).Returns(new List<AatfReuseSitesData> { csvData1, csvData2 });

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().Contain($"{csvData1.Abbreviation},{csvData1.PanName},{csvData1.LaName},{csvData1.ComplianceYear},{csvData1.Quarter},{csvData1.SubmittedBy},{csvData1.SubmittedDate},{csvData1.OrgName},{csvData1.Name},{csvData1.ApprovalNumber},{csvData1.SiteName},\"{csvData1.SiteAddress}\"");
            data.FileContent.Should().Contain($"{csvData2.Abbreviation},{csvData2.PanName},{csvData2.LaName},{csvData2.ComplianceYear},{csvData2.Quarter},{csvData2.SubmittedBy},{csvData2.SubmittedDate},{csvData2.OrgName},{csvData2.Name},{csvData2.ApprovalNumber},{csvData2.SiteName},\"{csvData2.SiteAddress}\"");
        }
    }
}
