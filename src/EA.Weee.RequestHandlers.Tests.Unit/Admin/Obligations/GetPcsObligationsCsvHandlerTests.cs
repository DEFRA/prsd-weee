namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using AutoFixture;
    using AutoFixture.Kernel;
    using EA.Prsd.Core;
    using EA.Weee.Core.Admin;
    using EA.Weee.Core.Constants;
    using EA.Weee.Core.Shared;
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Weee.Security;
    using Xunit;

    public class GetPcsObligationsCsvHandlerTests
    {
        private GetPcsObligationsCsvHandler handler;
        private readonly GetPcsObligationsCsv request;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly ICsvWriter<Scheme> csvWriter;
        private readonly ICommonDataAccess dataAccess;
        private readonly Fixture fixture;

        public GetPcsObligationsCsvHandlerTests()
        {
            fixture = new Fixture();
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IExcelSanitizer),
                    typeof(NoFormulaeExcelSanitizer)));

            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            dataAccess = A.Fake<ICommonDataAccess>();
            csvWriter = A.Fake<ICsvWriter<Scheme>>();

            request = new GetPcsObligationsCsv(fixture.Create<CompetentAuthority>());

            handler = new GetPcsObligationsCsvHandler(weeeAuthorization,
                csvWriter,
                dataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetPcsObligationsCsvHandler(authorization,
                csvWriter,
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
        public async Task HandleAsync_NotAnAdminUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyAnyRole().Build();

            handler = new GetPcsObligationsCsvHandler(authorization,
                csvWriter,
                dataAccess);

            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_UserInAdminRole_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => weeeAuthorization.EnsureUserInRole(Roles.InternalAdmin)).MustHaveHappenedOnceExactly();
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
        public async Task HandleAsync_CsvWriter_DefinesCorrectColumns()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.SchemeIdentifierColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened()
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.SchemeNameColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened()).Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat1ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat2ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat3ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat4ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat5ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat6ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat7ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat8ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat9ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat10ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat11ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat12ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat13ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened())
                .Then(A.CallTo(() => csvWriter.DefineColumn(ObligationCsvConstants.Cat14ColumnName, A<Func<Scheme, object>>._, false)).MustHaveHappened());
        }

        [Fact]
        public async Task HandleAsync_GivenSchemes_CsvWriterShouldBeCalled()
        {
            //arrange
            var competentAuthority = A.Fake<UKCompetentAuthority>();
            var scheme1 = A.Fake<Scheme>();
            A.CallTo(() => scheme1.SchemeName).Returns("Z");
            var scheme2 = A.Fake<Scheme>();
            A.CallTo(() => scheme2.SchemeName).Returns("A");
            var scheme3 = A.Fake<Scheme>();
            A.CallTo(() => scheme3.SchemeName).Returns("B");
            var schemes = new List<Scheme>() { scheme1, scheme2, scheme3 };
            A.CallTo(() => competentAuthority.Schemes).Returns(schemes);

            A.CallTo(() => dataAccess.FetchCompetentAuthorityApprovedSchemes(A<CompetentAuthority>._)).Returns(competentAuthority);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            A.CallTo(() => csvWriter.Write(A<IEnumerable<Scheme>>.That.Matches(s => s.SequenceEqual(schemes.OrderBy(s1 => s1.SchemeName))))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenSchemesCsv_FileContentShouldBeReturned()
        {
            //arrange
            var content = fixture.Create<string>();
            A.CallTo(() => csvWriter.Write(A<IEnumerable<Scheme>>._)).Returns(content);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileContent.Should().Be(content);
        }

        [Fact]
        public async Task HandleAsync_CsvFileData_HasCorrectlyFormattedFileName()
        {
            //arrange
            var date = new DateTime(2019, 2, 1, 11, 1, 2);
            SystemTime.Freeze(date);

            var competentAuthority = new UKCompetentAuthority(A.Dummy<Guid>(), "Test Auth", "AA", A.Dummy<Country>(), A.Dummy<string>(), null);
            A.CallTo(() => dataAccess.FetchCompetentAuthorityApprovedSchemes(request.Authority)).Returns(competentAuthority);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.FileName.Should().Be($"{competentAuthority.Abbreviation}_pcsobligationuploadtemplate{date.ToString(DateTimeConstants.FilenameTimestampFormat)}.csv");
            SystemTime.Unfreeze();
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
