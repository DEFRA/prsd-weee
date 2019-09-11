namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports
{
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Shared;
    using DataAccess;
    using DataAccess.StoredProcedure;
    using Domain.Lookup;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Admin.AatfReports;
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

            GetAatfAeDetailsCsvHandler handler = new GetAatfAeDetailsCsvHandler(authorization, context, csvWriterFactory, commonDataAccess);
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<bool>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<SecurityException>(action);
        }

        [Fact]
        public async Task HandleAsync_NoComplianceYear_ThrowsArgumentException()
        {
            const int complianceYear = 0;

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(complianceYear, fixture.Create<ReportFacilityType>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<bool>());

            Func<Task> action = async () => await handler.HandleAsync(request);

            await Assert.ThrowsAsync<ArgumentException>(action);
        }

        [Fact]
        public async Task HandleAsync_VariousParameters_ReturnsFileContent()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<Guid>(), fixture.Create<bool>());

            CSVFileData data = await handler.HandleAsync(request);

            data.FileContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task HandleAsync_GivenStoredProcedureReturnItems_MatchingFileContent()
        {
            int complianceYear = fixture.Create<int>();
            ReportFacilityType facilityType = fixture.Create<ReportFacilityType>();
            Guid authority = fixture.Create<Guid>();
            Guid area = fixture.Create<Guid>();
            Guid pat = fixture.Create<Guid>();

            AatfAeDetailsData csvData1 = CreateCsvData();

            AatfAeDetailsData csvData2 = CreateCsvData();

            AatfAeDetailsData csvData3 = CreateCsvData();

            A.CallTo(() => storedProcedures.GetAatfAeDetailsCsvData(complianceYear, (int)facilityType, authority, area, pat))
            .Returns(new List<AatfAeDetailsData> { csvData1, csvData2, csvData3 });

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(complianceYear, facilityType, authority, pat, area, false);

            CSVFileData data = await handler.HandleAsync(request);

            string facilityTypeString = facilityType.ToString().ToUpper();

            data.FileContent.Should().Contain($"Compliance year,Appropriate authority,WROS Pan Area Team,EA Area,Name of {facilityTypeString},{facilityTypeString} address1,{facilityTypeString} address2,{facilityTypeString} town or city,{facilityTypeString} county or region,{facilityTypeString} country,{facilityTypeString} postcode,{facilityTypeString} approval number,Date of approval,{facilityTypeString} size,{facilityTypeString} status,Contact name,Contact position,Contact address1,Contact address2,Contact town or city,Contact county or region,Contact country,Contact postcode,Contact email,Contact phone number,Organisation name,Organisation address1,Organisation address2,Organisation town or city,Organisation county or region,Organisation country,Organisation postcode");
            data.FileContent.Should().Contain($"{csvData1.ComplianceYear},{csvData1.AppropriateAuthorityAbbr},{csvData1.PanAreaTeam},{csvData1.EaArea},{csvData1.Name},{csvData1.Address1},{csvData1.Address2},{csvData1.TownCity},{csvData1.CountyRegion},{csvData1.Country},{csvData1.PostCode},{csvData1.ApprovalNumber},{csvData1.ApprovalDateString},{csvData1.Size},{csvData1.Status},{csvData1.FirstName},{csvData1.ContactPosition},{csvData1.ContactAddress1},{csvData1.ContactAddress2},{csvData1.ContactTownCity},{csvData1.ContactCountyRegion},{csvData1.ContactCountry},{csvData1.ContactPostcode},{csvData1.ContactEmail},{csvData1.ContactPhone},{csvData1.OrganisationName},{csvData1.OrganisationAddress1},{csvData1.OrganisationAddress2},{csvData1.OrganisationTownCity},{csvData1.OrganisationCountyRegion},{csvData1.OrganisationCountry},{csvData1.OrganisationPostcode}");
            data.FileContent.Should().Contain($"{csvData2.ComplianceYear},{csvData2.AppropriateAuthorityAbbr},{csvData2.PanAreaTeam},{csvData2.EaArea},{csvData2.Name},{csvData2.Address1},{csvData2.Address2},{csvData2.TownCity},{csvData2.CountyRegion},{csvData2.Country},{csvData2.PostCode},{csvData2.ApprovalNumber},{csvData2.ApprovalDateString},{csvData2.Size},{csvData2.Status},{csvData2.FirstName},{csvData2.ContactPosition},{csvData2.ContactAddress1},{csvData2.ContactAddress2},{csvData2.ContactTownCity},{csvData2.ContactCountyRegion},{csvData2.ContactCountry},{csvData2.ContactPostcode},{csvData2.ContactEmail},{csvData2.ContactPhone},{csvData2.OrganisationName},{csvData2.OrganisationAddress1},{csvData2.OrganisationAddress2},{csvData2.OrganisationTownCity},{csvData2.OrganisationCountyRegion},{csvData2.OrganisationCountry},{csvData2.OrganisationPostcode}");
            data.FileContent.Should().Contain($"{csvData3.ComplianceYear},{csvData3.AppropriateAuthorityAbbr},{csvData3.PanAreaTeam},{csvData3.EaArea},{csvData3.Name},{csvData3.Address1},{csvData3.Address2},{csvData3.TownCity},{csvData3.CountyRegion},{csvData3.Country},{csvData3.PostCode},{csvData3.ApprovalNumber},{csvData3.ApprovalDateString},{csvData3.Size},{csvData3.Status},{csvData3.FirstName},{csvData3.ContactPosition},{csvData3.ContactAddress1},{csvData3.ContactAddress2},{csvData3.ContactTownCity},{csvData3.ContactCountyRegion},{csvData3.ContactCountry},{csvData3.ContactPostcode},{csvData3.ContactEmail},{csvData3.ContactPhone},{csvData3.OrganisationName},{csvData3.OrganisationAddress1},{csvData3.OrganisationAddress2},{csvData3.OrganisationTownCity},{csvData3.OrganisationCountyRegion},{csvData3.OrganisationCountry},{csvData3.OrganisationPostcode}");
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParameters_FileNameShouldBeCorrect()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), null, null, null, false);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{request.FacilityType.ToString().ToUpper()} details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndAuthority_FileNameShouldBeCorrect()
        {
            UKCompetentAuthority ca = fixture.Create<UKCompetentAuthority>();

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), ca.Id, null, null, false);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{ca.Abbreviation}_{request.FacilityType.ToString().ToUpper()} details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndPanArea_FileNameShouldBeCorrect()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), null, fixture.Create<Guid>(), null, false);

            PanArea panArea = fixture.Create<PanArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{panArea.Name}_{request.FacilityType.ToString().ToUpper()} details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndLocalArea_FileNameShouldNotContainLocalArea()
        {
            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), null, null, fixture.Create<Guid>(), false);

            LocalArea localArea = fixture.Create<LocalArea>();
            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{localArea.Name}_{request.FacilityType.ToString().ToUpper()} details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersAndAllOptionalParameters_FileNameShouldBeCorrect()
        {
            UKCompetentAuthority ca = fixture.Create<UKCompetentAuthority>();
            LocalArea localArea = fixture.Create<LocalArea>();
            PanArea panArea = fixture.Create<PanArea>();

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), ca.Id, panArea.Id, localArea.Id, false);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            A.CallTo(() => commonDataAccess.FetchLookup<LocalArea>(request.LocalArea.Value)).Returns(localArea);

            A.CallTo(() => commonDataAccess.FetchLookup<PanArea>(request.PanArea.Value)).Returns(panArea);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{ca.Abbreviation}_{panArea.Name}_{localArea.Name}_{request.FacilityType.ToString().ToUpper()} details_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        [Fact]
        public async Task HandleAsyncForPublicRegister_GivenStoredProcedureReturnItems_MatchingFileContent()
        {
            int complianceYear = fixture.Create<int>();
            ReportFacilityType facilityType = fixture.Create<ReportFacilityType>();
            Guid authority = fixture.Create<Guid>();

            AatfAeDetailsData csvData1 = CreateCsvData();

            AatfAeDetailsData csvData2 = CreateCsvData();

            AatfAeDetailsData csvData3 = CreateCsvData();

            A.CallTo(() => storedProcedures.GetAatfAeDetailsCsvData(complianceYear, (int)facilityType, authority, null, null))
            .Returns(new List<AatfAeDetailsData> { csvData1, csvData2, csvData3 });

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(complianceYear, facilityType, authority, null, null, true);

            CSVFileData data = await handler.HandleAsync(request);

            string facilityTypeString = facilityType.ToString().ToUpper();

            data.FileContent.Should().Contain($"Compliance year,Appropriate authority,Name of {facilityTypeString},{facilityTypeString} address,{facilityTypeString} postcode,{facilityTypeString} country,EA Area for the {facilityTypeString},{facilityTypeString} approval number,Date of approval,{facilityTypeString} size,{facilityTypeString} status,Name of operator,Business trading name of operator,Operator address,Operator postcode,Operator country");
            data.FileContent.Should().Contain($"{csvData1.ComplianceYear},{csvData1.AppropriateAuthorityAbbr},{csvData1.Name},{csvData1.AatfAddress},{csvData1.PostCode},{csvData1.Country},{csvData1.EaArea},{csvData1.ApprovalNumber},{csvData1.ApprovalDateString},{csvData1.Size},{csvData1.Status},{csvData1.OperatorName},{csvData1.OperatorTradingName},{csvData1.OperatorAddress},{csvData1.OrganisationPostcode},{csvData1.OrganisationCountry}");
            data.FileContent.Should().Contain($"{csvData2.ComplianceYear},{csvData2.AppropriateAuthorityAbbr},{csvData2.Name},{csvData2.AatfAddress},{csvData2.PostCode},{csvData2.Country},{csvData2.EaArea},{csvData2.ApprovalNumber},{csvData2.ApprovalDateString},{csvData2.Size},{csvData2.Status},{csvData2.OperatorName},{csvData2.OperatorTradingName},{csvData2.OperatorAddress},{csvData2.OrganisationPostcode},{csvData2.OrganisationCountry}");
            data.FileContent.Should().Contain($"{csvData3.ComplianceYear},{csvData3.AppropriateAuthorityAbbr},{csvData3.Name},{csvData3.AatfAddress},{csvData3.PostCode},{csvData3.Country},{csvData3.EaArea},{csvData3.ApprovalNumber},{csvData3.ApprovalDateString},{csvData3.Size},{csvData3.Status},{csvData3.OperatorName},{csvData3.OperatorTradingName},{csvData3.OperatorAddress},{csvData3.OrganisationPostcode},{csvData3.OrganisationCountry}");
        }

        [Fact]
        public async Task HandleAsync_GivenMandatoryParametersPublicRegister_FileNameShouldBeCorrect()
        {
            UKCompetentAuthority ca = fixture.Create<UKCompetentAuthority>();

            GetAatfAeDetailsCsv request = new GetAatfAeDetailsCsv(fixture.Create<int>(), fixture.Create<ReportFacilityType>(), ca.Id, null, null, true);

            A.CallTo(() => commonDataAccess.FetchCompetentAuthorityById(request.AuthorityId.Value)).Returns(ca);

            DateTime date = new DateTime(2019, 05, 18, 11, 12, 0);

            SystemTime.Freeze(date);

            CSVFileData data = await handler.HandleAsync(request);

            data.FileName.Should().Be($"{request.ComplianceYear}_{ca.Abbreviation}_{request.FacilityType.ToString().ToUpper()} public register_{date:ddMMyyyy_HHmm}.csv");

            SystemTime.Unfreeze();
        }

        private AatfAeDetailsData CreateCsvData()
        {
            return new AatfAeDetailsData
            {
                ComplianceYear = fixture.Create<string>(),
                AppropriateAuthorityAbbr = fixture.Create<string>(),
                PanAreaTeam = fixture.Create<string>(),
                EaArea = fixture.Create<string>(),
                Name = fixture.Create<string>(),
                Address1 = fixture.Create<string>(),
                Address2 = fixture.Create<string>(),
                TownCity = fixture.Create<string>(),
                CountyRegion = fixture.Create<string>(),
                Country = fixture.Create<string>(),
                PostCode = fixture.Create<string>(),
                ApprovalNumber = fixture.Create<string>(),
                ApprovalDate = fixture.Create<DateTime>(),
                Size = fixture.Create<string>(),
                Status = fixture.Create<string>(),
                FirstName = fixture.Create<string>(),
                LastName = fixture.Create<string>(),
                ContactPosition = fixture.Create<string>(),
                ContactAddress1 = fixture.Create<string>(),
                ContactAddress2 = fixture.Create<string>(),
                ContactTownCity = fixture.Create<string>(),
                ContactCountyRegion = fixture.Create<string>(),
                ContactCountry = fixture.Create<string>(),
                ContactPostcode = fixture.Create<string>(),
                ContactEmail = fixture.Create<string>(),
                ContactPhone = fixture.Create<string>(),
                OrganisationName = fixture.Create<string>(),
                OrganisationAddress1 = fixture.Create<string>(),
                OrganisationAddress2 = fixture.Create<string>(),
                OrganisationTownCity = fixture.Create<string>(),
                OrganisationCountyRegion = fixture.Create<string>(),
                OrganisationCountry = fixture.Create<string>(),
                OrganisationPostcode = fixture.Create<string>(),
                AatfAddress = fixture.Create<string>(),
                OperatorAddress = fixture.Create<string>(),
                OperatorName = fixture.Create<string>(),
                OperatorTradingName = fixture.Create<string>()
            };
        }
    }
}
