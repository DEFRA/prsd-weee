namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using Prsd.Core.Helpers;
    using RequestHandlers.Admin.AatfReports;
    using RequestHandlers.Shared;
    using Requests.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfAeReturnDataCsvHandlerTests
    {
        private readonly GetAatfAeReturnDataCsvHandler handler;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly Fixture fixture;
        private readonly IStoredProcedures storedProcedures;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAatfAeReturnDataCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            fixture = new Fixture();
            storedProcedures = A.Fake<IStoredProcedures>();
            commonDataAccess = A.Fake<ICommonDataAccess>();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            handler = new GetAatfAeReturnDataCsvHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                context,
                csvWriterFactory,
                commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NotInternalUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            var handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory, commonDataAccess);
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<ReportReturnStatus>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>(), fixture.Create<bool>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            var request = new GetAatfAeReturnDataCsv(complianceYear, fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<ReportReturnStatus>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>(), fixture.Create<bool>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_VariousParameters_ReturnsFileContent()
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<ReportReturnStatus>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>(), fixture.Create<bool>());

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task HandleAsync_GivenStoredProcedureReturnItems_MatchingFileContent()
        {
            var complianceYear = fixture.Create<int>();
            var quarter = fixture.Create<int>();
            var facilityType = fixture.Create<FacilityType>();
            var returnStatus = fixture.Create<ReportReturnStatus>();
            var authority = fixture.Create<Guid>();
            var area = fixture.Create<Guid>();
            var pat = fixture.Create<Guid>();
            var resubmission = fixture.Create<bool>();
            var aatf = fixture.Create<string>();

            var csvData1 = new AatfAeReturnData
            {
                Name = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                ReturnStatus = fixture.Create<string>(),
                CreatedDate = fixture.Create<DateTime>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                CompetentAuthorityAbbr = fixture.Create<string>(),
                ReSubmission = fixture.Create<string>()
            };

            var csvData2 = new AatfAeReturnData
            {
                Name = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                ReturnStatus = fixture.Create<string>(),
                CreatedDate = fixture.Create<DateTime>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                CompetentAuthorityAbbr = fixture.Create<string>(),
                ReSubmission = fixture.Create<string>()
            };

            var csvData3 = new AatfAeReturnData
            {
                Name = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                ReturnStatus = fixture.Create<string>(),
                CreatedDate = fixture.Create<DateTime>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                CompetentAuthorityAbbr = fixture.Create<string>(),
                ReSubmission = fixture.Create<string>()
            };

            A.CallTo(() => storedProcedures.GetAatfAeReturnDataCsvData(complianceYear, quarter, (int)facilityType, (int)returnStatus, authority, area, pat, resubmission))
            .Returns(new List<AatfAeReturnData> { csvData1, csvData2, csvData3 });

            var request = new GetAatfAeReturnDataCsv(complianceYear, quarter, facilityType, returnStatus, authority, pat, area, aatf, resubmission);

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().Contain("Name of AATF / AE,Approval number,Organisation name,Submission status,Date created (GMT),Date submitted (GMT),Submitted by,Appropriate authority,First submission / resubmission,");
            data.FileContent.Should().Contain($"{csvData1.Name},{csvData1.ApprovalNumber},{csvData1.OrganisationName},{csvData1.ReturnStatus},{csvData1.CreatedDate},{csvData1.SubmittedDate},{csvData1.SubmittedBy},{csvData1.CompetentAuthorityAbbr},{csvData1.ReSubmission}");
            data.FileContent.Should().Contain($"{csvData2.Name},{csvData2.ApprovalNumber},{csvData2.OrganisationName},{csvData2.ReturnStatus},{csvData2.CreatedDate},{csvData2.SubmittedDate},{csvData2.SubmittedBy},{csvData2.CompetentAuthorityAbbr},{csvData2.ReSubmission}");
            data.FileContent.Should().Contain($"{csvData3.Name},{csvData3.ApprovalNumber},{csvData3.OrganisationName},{csvData3.ReturnStatus},{csvData3.CreatedDate},{csvData3.SubmittedDate},{csvData3.SubmittedBy},{csvData3.CompetentAuthorityAbbr},{csvData3.ReSubmission}");
        }

        [Fact]
        public async Task GetAatfAeReturnDataCSVHandler_Sets_URL()
        {
            var csvData1 = new AatfAeReturnData
            {
                Name = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                ReturnStatus = fixture.Create<string>(),
                CreatedDate = fixture.Create<DateTime>(),
                SubmittedBy = fixture.Create<string>(),
                SubmittedDate = fixture.Create<DateTime>(),
                CompetentAuthorityAbbr = fixture.Create<string>(),
                ReSubmission = fixture.Create<string>()
            };

            A.CallTo(() => storedProcedures
            .GetAatfAeReturnDataCsvData(A<int>._, A<int>._, A<int>._, A<int>._, A<Guid>._, A<Guid>._, A<Guid>._, A<bool>._)).Returns(new List<AatfAeReturnData> { csvData1 });

            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<ReportReturnStatus>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), "https://localhost:44300/admin/aatf/details/", fixture.Create<bool>());

            var url1 = $@"""=HYPERLINK(""""{request.AatfDataUrl}{csvData1.AatfId}#data"""", """"View AATF / AE data"""")";

            var data = await handler.HandleAsync(request);

            data.FileContent.Should().Contain($"{csvData1.Name},{csvData1.ApprovalNumber},{csvData1.OrganisationName},{csvData1.ReturnStatus},{csvData1.CreatedDate},{csvData1.SubmittedDate},{csvData1.SubmittedBy},{csvData1.CompetentAuthorityAbbr},{csvData1.ReSubmission},{url1}");
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParameters_FileNameShouldBeCorrect()
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, null, null, fixture.Create<string>(), false);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_Exclude resubmissions_{request.FacilityType.ToString().ToUpper()}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false)]
        [InlineData("Include resubmissions", true)]
        public async Task HandleAsync_GivenMandatoryParametersAndIncludeResubmissions_FileNameShouldBeCorrect(string expectedText, bool includeResubmissions)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, null, null, fixture.Create<string>(), includeResubmissions);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.Submitted)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.Submitted)]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.NotStarted)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.NotStarted)]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.Started)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.Started)]
        public async Task HandleAsync_GivenMandatoryParametersAndSubmissionStatus_FileNameShouldBeCorrect(string expectedText, bool includeResubmissions, ReportReturnStatus status)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), status, null, null, null, fixture.Create<string>(), includeResubmissions);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_{EnumHelper.GetDisplayName(status)}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false)]
        [InlineData("Include resubmissions", true)]
        public async Task HandleAsync_GivenMandatoryParametersAndAuthority_FileNameShouldBeCorrect(string expectedText, bool includeResubmissions)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), null, fixture.Create<Guid>(), null, null, fixture.Create<string>(), includeResubmissions);

            var ca = fixture.Create<EA.Weee.Domain.UKCompetentAuthority>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_{ca.Abbreviation}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false)]
        [InlineData("Include resubmissions", true)]
        public async Task HandleAsync_GivenMandatoryParametersAndPanArea_FileNameShouldBeCorrect(string expectedText, bool includeResubmissions)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, fixture.Create<Guid>(), null, fixture.Create<string>(), includeResubmissions);

            var panArea = fixture.Create<PanArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_{panArea.Name}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false)]
        [InlineData("Include resubmissions", true)]
        public async Task HandleAsync_GivenMandatoryParametersAndLocalArea_FileNameShouldNotContainLocalArea(string expectedText, bool includeResubmissions)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, null, fixture.Create<Guid>(), fixture.Create<string>(), includeResubmissions);

            var localArea = fixture.Create<LocalArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Theory]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.Submitted)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.Submitted)]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.NotStarted)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.NotStarted)]
        [InlineData("Exclude resubmissions", false, ReportReturnStatus.Started)]
        [InlineData("Include resubmissions", true, ReportReturnStatus.Started)]
        public async Task HandleAsync_GivenMandatoryParametersAndAllOptionalParameters_FileNameShouldBeCorrect(string expectedText, bool includeResubmissions, ReportReturnStatus status)
        {
            var request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), status, fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>(), includeResubmissions);

            var ca = fixture.Create<EA.Weee.Domain.UKCompetentAuthority>();
            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            var localArea = fixture.Create<LocalArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            var panArea = fixture.Create<PanArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            var date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            var data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_Q{request.Quarter}_{expectedText}_{request.FacilityType.ToString().ToUpper()}_{EnumHelper.GetDisplayName(status)}_{ca.Abbreviation}_{panArea.Name}_Summary of AATF-AE returns to date_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }
    }
}
