namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Admin;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain;
    using Domain.Lookup;
    using EA.Weee.RequestHandlers.Admin.AatfReports;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin.Aatf;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.Shared;
    using Requests.Admin.AatfReports;
    using Weee.Tests.Core;
    using Xunit;

    public class GetNonObligatedWeeeReceivedAtAatfsDataCsvHandlerTests
    {
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler handler;
        private readonly Fixture fixture;

        public GetNonObligatedWeeeReceivedAtAatfsDataCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            fixture = new Fixture();

            var authorization = new AuthorizationBuilder().AllowInternalAreaAccess().Build();

            handler = new GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler(authorization, context, csvWriterFactory, commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NotInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetNonObligatedWeeeReceivedDataAtAatfsCsvHandler(authorization, context, csvWriterFactory, commonDataAccess);
            var request = fixture.Create<GetUkNonObligatedWeeeReceivedAtAatfsDataCsv>();

            Func<Task> action = async () => await handler.HandleAsync(request);

            await action.Should().ThrowAsync<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(0, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ReturnsFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<string>());

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_MatchingFileContent()
        {
            var storedProcedures = A.Fake<IStoredProcedures>();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            var csvData1 = new NonObligatedWeeeReceivedAtAatfsData()
            {
                Authority = fixture.Create<string>(),
                PatArea = fixture.Create<string>(),
                Area = fixture.Create<string>(),
                Year = fixture.Create<int>(),
                Quarter = fixture.Create<string>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                OrganisationName = fixture.Create<string>(),
                Category = fixture.Create<string>(),
                TotalNonObligatedWeeeReceived = fixture.Create<decimal>(),
                TotalNonObligatedWeeeReceivedFromDcf = fixture.Create<decimal>()
            };

            var csvData2 = new NonObligatedWeeeReceivedAtAatfsData()
            {
                Authority = fixture.Create<string>(),
                PatArea = fixture.Create<string>(),
                Area = fixture.Create<string>(),
                Year = fixture.Create<int>(),
                Quarter = fixture.Create<string>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                OrganisationName = fixture.Create<string>(),
                Category = fixture.Create<string>(),
                TotalNonObligatedWeeeReceived = fixture.Create<decimal>(),
                TotalNonObligatedWeeeReceivedFromDcf = fixture.Create<decimal>()
            };

            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>());

            A.CallTo(() => storedProcedures.GetNonObligatedWeeeReceivedAtAatfs(request.ComplianceYear, request.AuthorityId, request.PatAreaId, request.AatfName)).Returns(new List<NonObligatedWeeeReceivedAtAatfsData> { csvData1, csvData2 });

            var data = await handler.HandleAsync(request);

            data.FileContent.Should()
                .Contain(
                    "Authority,Pat Area,Area,Year,Quarter,Submitted by,Date submitted,Organisation name,Category,Total non-obligated WEEE received (t),Non-obligated WEEE kept / retained by DCFs (t)");
            data.FileContent.Should().Contain($"{csvData1.Authority},{csvData1.PatArea},{csvData1.Area},{csvData1.Year},{csvData1.Quarter},{csvData1.SubmittedBy},{csvData1.SubmittedDate},{csvData1.OrganisationName},{csvData1.Category},{csvData1.TotalNonObligatedWeeeReceived},{csvData1.TotalNonObligatedWeeeReceivedFromDcf}");
            data.FileContent.Should().Contain($"{csvData2.Authority},{csvData2.PatArea},{csvData2.Area},{csvData2.Year},{csvData2.Quarter},{csvData2.SubmittedBy},{csvData2.SubmittedDate},{csvData2.OrganisationName},{csvData2.Category},{csvData2.TotalNonObligatedWeeeReceived},{csvData2.TotalNonObligatedWeeeReceivedFromDcf}");
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, null, null, null);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));
            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndAuthorityRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, fixture.Create<Guid>(), null, null);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var ca = new UKCompetentAuthority(fixture.Create<Guid>(), null, "CA", new Country(fixture.Create<Guid>(), null), null, null);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.AuthorityId.Value)).Returns(ca);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_CA_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndAuthorityAndPatAreaRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, fixture.Create<Guid>(), fixture.Create<Guid>(), null);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var ca = new UKCompetentAuthority(fixture.Create<Guid>(), null, "CA", new Country(fixture.Create<Guid>(), null), null, null);
            var patArea = fixture.Build<PanArea>().With(p => p.Name, "PAT").Create();

            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.AuthorityId.Value)).Returns(ca);
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PatAreaId.Value)).Returns(patArea);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_CA_PAT_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndAuthorityAndPatAreaAndAatfRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, fixture.Create<Guid>(), fixture.Create<Guid>(), "AATF NAME AATF NAME AATF NAME AATF NAME");

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var ca = new UKCompetentAuthority(fixture.Create<Guid>(), null, "CA", new Country(fixture.Create<Guid>(), null), null, null);
            var patArea = fixture.Build<PanArea>().With(p => p.Name, "PAT").Create();

            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.AuthorityId.Value)).Returns(ca);
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PatAreaId.Value)).Returns(patArea);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_CA_PAT_AATF NAME AATF NAME AATF NAME AATF NAME_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndAuthorityAndAatfRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, fixture.Create<Guid>(), null, "AATF NAME AATF NAME AATF NAME AATF NAME");

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var ca = new UKCompetentAuthority(fixture.Create<Guid>(), null, "CA", new Country(fixture.Create<Guid>(), null), null, null);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.AuthorityId.Value)).Returns(ca);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_CA_AATF NAME AATF NAME AATF NAME AATF NAME_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndPatAreaAndAatfRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, null, fixture.Create<Guid>(), "AATF NAME AATF NAME AATF NAME AATF NAME");

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var patArea = fixture.Build<PanArea>().With(p => p.Name, "PAT").Create();

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PatAreaId.Value)).Returns(patArea);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_PAT_AATF NAME AATF NAME AATF NAME AATF NAME_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAndPatArea_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, null, fixture.Create<Guid>(), null);

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var patArea = fixture.Build<PanArea>().With(p => p.Name, "PAT").Create();
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PatAreaId.Value)).Returns(patArea);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_PAT_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenComplianceYearAatfRequest_FileNamedShouldBeCorrectFileContent()
        {
            var request = new GetUkNonObligatedWeeeReceivedAtAatfsDataCsv(2019, null, null, "AATF NAME AATF NAME AATF NAME AATF NAME");

            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be("2019_AATF NAME AATF NAME AATF NAME AATF NAME_AATF non-obligated WEEE data_01022019_1101.csv");

            SystemTime.Unfreeze();
        }
    }
}
