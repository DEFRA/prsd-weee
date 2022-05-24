namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using AutoFixture;
    using EA.Prsd.Core;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Scheme;
    using EA.Weee.RequestHandlers.Admin.Obligations;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.RequestHandlers.Shared;
    using EA.Weee.Requests.Admin.Obligations;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetPcsObligationsCsvHandlerTests
    {
        private GetPcsObligationsCsvHandler handler;
        private readonly CompetentAuthority authority;
        private readonly GetPcsObligationsCsv request;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly WeeeContext context;
        private readonly CsvWriterFactory csvWriterFactory;
        private readonly ICommonDataAccess dataAccess;

        public GetPcsObligationsCsvHandlerTests()
        {
            fixture = new Fixture();
            authority = CompetentAuthority.England;
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            context = A.Fake<WeeeContext>();
            dataAccess = A.Fake<ICommonDataAccess>();
            csvWriterFactory = A.Fake<CsvWriterFactory>();

            request = new GetPcsObligationsCsv(authority);

            handler = new GetPcsObligationsCsvHandler(weeeAuthorization,
                context,
                csvWriterFactory,
                dataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetPcsObligationsCsvHandler(authorization,
                context,
                csvWriterFactory,
                dataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckInternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_ReturnsCsvFileData()
        {
            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().BeOfType<CSVFileData>();
        }

        [Fact]
        public async Task HandleAsync_CreatesCsvWriter()
        {
            //arrange
            var csvWriter = fixture.Create<CsvWriter<Scheme>>();

            A.CallTo(() => csvWriterFactory.Create<Scheme>()).Returns(csvWriter);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => csvWriterFactory.Create<Scheme>()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_CsvWriter_DefinesCorrectCollumns()
        {
            //arrange
            var csvWriter = fixture.Create<CsvWriter<Scheme>>();

            A.CallTo(() => csvWriterFactory.Create<Scheme>()).Returns(csvWriter);

            //act
            await handler.HandleAsync(request);

            //assert
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.SchemeIdentifierColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.SchemeNameColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat1ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat2ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat3ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat4ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat5ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat6ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat7ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat8ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat9ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat10ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat11ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat12ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat13ColumnName);
            csvWriter.ColumnTitles.Should().Contain(ObligationCsvConstants.Cat14ColumnName);
        }

        [Fact]
        public async Task HandleAsync_CsvFileData_HasCorrectlyFormattedFileName()
        {
            //arrange
            SystemTime.Freeze(new DateTime(2019, 2, 1, 11, 1, 2));
            var date = DateTime.Now;
            var csvWriter = fixture.Create<CsvWriter<Scheme>>();

            var competentAuthority = new UKCompetentAuthority(A.Dummy<Guid>(), "Test Auth", "AA", A.Dummy<Country>(), A.Dummy<string>(), null);
            A.CallTo(() => dataAccess.FetchCompetentAuthorityApprovedSchemes(request.Authority)).Returns(competentAuthority);

            A.CallTo(() => csvWriterFactory.Create<Scheme>()).Returns(csvWriter);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileName.Should().Be($"{competentAuthority.Abbreviation}_pcsobligationuploadtemplate{date.ToString(DateTimeConstants.FilenameTimestampFormat)}.csv)");
        }

        [Fact]
        public async Task HandleAsync_CallsCommonDataAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => dataAccess.FetchCompetentAuthorityApprovedSchemes(request.Authority)).MustHaveHappenedOnceExactly();
        }
    }
}
