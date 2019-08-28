namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using EA.Weee.Core.Admin;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.Security;
    using FakeItEasy;
    using FluentAssertions;
    using Prsd.Core;
    using RequestHandlers.Admin.AatfReports;
    using RequestHandlers.Shared;
    using Requests.Admin.AatfReports;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfAeDetailsCsvHandlerTests
    {
        private readonly GetAatfAeDetailsCsvHandler handler;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly Fixture fixture;
        private readonly IStoredProcedures storedProcedures;
        private readonly ICommonDataAccess commonDataAccess;

        public GetAatfAeDetailsCsvHandlerTests()
        {
            context = A.Fake<WeeeContext>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();
            fixture = new Fixture();
            storedProcedures = A.Fake<IStoredProcedures>();
            commonDataAccess = A.Fake<ICommonDataAccess>();

            A.CallTo(() => context.StoredProcedures).Returns(storedProcedures);

            handler = new GetAatfAeDetailsCsvHandler(new AuthorizationBuilder().AllowInternalAreaAccess().Build(),
                context,
                csvWriterFactory,
                commonDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NotInternalUser_ThrowsSecurityException()
        {
            IWeeeAuthorization authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            GetAatfAeReturnDataCsvHandler handler = new GetAatfAeReturnDataCsvHandler(authorization, context, csvWriterFactory, commonDataAccess);
            GetAatfAeReturnDataCsv request = new GetAatfAeReturnDataCsv(fixture.Create<int>(), fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<ReportReturnStatus>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<string>(), fixture.Create<bool>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(complianceYear, fixture.Create<FacilityType>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_VariousParameters_ReturnsFileContent()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>());

            CSVFileData data = await handler.HandleAsync(request);

            data.FileContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task HandleAsync_GivenStoredProcedureReturnItems_MatchingFileContent()
        {
            int complianceYear = fixture.Create<int>();
            FacilityType facilityType = fixture.Create<FacilityType>();
            Guid authority = fixture.Create<Guid>();
            Guid area = fixture.Create<Guid>();
            Guid pat = fixture.Create<Guid>();

            AatfAeDetailsData csvData1 = CreateCsvData();

            AatfAeDetailsData csvData2 = CreateCsvData();

            AatfAeDetailsData csvData3 = CreateCsvData();

            A.CallTo(() => storedProcedures.GetAatfAeDetailsCsvData(complianceYear, (int)facilityType, authority, area, pat))
            .Returns(new List<AatfAeDetailsData> { csvData1, csvData2, csvData3 });

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(complianceYear, facilityType, authority, pat, area);

            CSVFileData data = await handler.HandleAsync(request);

            string facilityTypeString = facilityType.ToString().ToUpper();

            data.FileContent.Should().Contain($"Compliance year,Appropriate authority,WROS Pan Area Team,EA Area,Name of {facilityTypeString},{facilityTypeString} address,{facilityTypeString} postcode,{facilityTypeString} approval number,Date of approval,{facilityTypeString} size,{facilityTypeString} status,Contact name,Contact position,Contact address,Contact postcode,Contact email,Contact phone number,Organisation name,Organisation address,Organisation postcode");
            data.FileContent.Should().Contain($"{csvData1.ComplianceYear},{csvData1.AppropriateAuthorityAbbr},{csvData1.PanAreaTeam},{csvData1.EaArea},{csvData1.Name},{csvData1.Address},{csvData1.PostCode},{csvData1.ApprovalNumber},{csvData1.ApprovalDate},{csvData1.Size},{csvData1.Status},{csvData1.ContactName},{csvData1.ContactPosition},{csvData1.ContactAddress},{csvData1.ContactPostcode},{csvData1.ContactEmail},{csvData1.ContactPhone},{csvData1.OrganisationName},{csvData1.OrganisationAddress},{csvData1.OrganisationPostcode}");
            data.FileContent.Should().Contain($"{csvData2.ComplianceYear},{csvData2.AppropriateAuthorityAbbr},{csvData2.PanAreaTeam},{csvData2.EaArea},{csvData2.Name},{csvData2.Address},{csvData2.PostCode},{csvData2.ApprovalNumber},{csvData2.ApprovalDate},{csvData2.Size},{csvData2.Status},{csvData2.ContactName},{csvData2.ContactPosition},{csvData2.ContactAddress},{csvData2.ContactPostcode},{csvData2.ContactEmail},{csvData2.ContactPhone},{csvData2.OrganisationName},{csvData2.OrganisationAddress},{csvData2.OrganisationPostcode}");
            data.FileContent.Should().Contain($"{csvData3.ComplianceYear},{csvData3.AppropriateAuthorityAbbr},{csvData3.PanAreaTeam},{csvData3.EaArea},{csvData3.Name},{csvData3.Address},{csvData3.PostCode},{csvData3.ApprovalNumber},{csvData3.ApprovalDate},{csvData3.Size},{csvData3.Status},{csvData3.ContactName},{csvData3.ContactPosition},{csvData3.ContactAddress},{csvData3.ContactPostcode},{csvData3.ContactEmail},{csvData3.ContactPhone},{csvData3.OrganisationName},{csvData3.OrganisationAddress},{csvData3.OrganisationPostcode}");
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParameters_FileNameShouldBeCorrect()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, null);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{request.FacilityType.ToString().ToUpper()}_details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndAuthority_FileNameShouldBeCorrect()
        {
            UKCompetentAuthority ca = fixture.Create<UKCompetentAuthority>();

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), ca.Id, null, null);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{ca.Abbreviation}_{request.FacilityType.ToString().ToUpper()}_details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndPanArea_FileNameShouldBeCorrect()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), null, fixture.Create<Guid>(), null);

            PanArea panArea = fixture.Create<PanArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{panArea.Name}_{request.FacilityType.ToString().ToUpper()}_details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndLocalArea_FileNameShouldNotContainLocalArea()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), null, null, fixture.Create<Guid>());

            LocalArea localArea = fixture.Create<LocalArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{localArea.Name}_{request.FacilityType.ToString().ToUpper()}_details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndAllOptionalParameters_FileNameShouldBeCorrect()
        {
            UKCompetentAuthority ca = fixture.Create<UKCompetentAuthority>();
            LocalArea localArea = fixture.Create<LocalArea>();
            PanArea panArea = fixture.Create<PanArea>();

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<FacilityType>(), ca.Id, panArea.Id, localArea.Id);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{ca.Abbreviation}_{panArea.Name}_{localArea.Name}_{request.FacilityType.ToString().ToUpper()}_details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        private AatfAeDetailsData CreateCsvData()
        {
            return new AatfAeDetailsData
            {
                ComplianceYear = fixture.Create<int>(),
                AppropriateAuthorityAbbr = fixture.Create<string>(),
                PanAreaTeam = fixture.Create<string>(),
                EaArea = fixture.Create<string>(),
                Name = fixture.Create<string>(),
                Address = fixture.Create<string>(),
                PostCode = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                ApprovalDate = fixture.Create<DateTime>(),
                Size = fixture.Create<string>(),
                Status = fixture.Create<string>(),
                ContactName = fixture.Create<string>(),
                ContactPosition = fixture.Create<string>(),
                ContactAddress = fixture.Create<string>(),
                ContactPostcode = fixture.Create<string>(),
                ContactEmail = fixture.Create<string>(),
                ContactPhone = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                OrganisationAddress = fixture.Create<string>(),
                OrganisationPostcode = fixture.Create<string>()
            };
        }
    }
}
