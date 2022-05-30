namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using DataAccess.DataAccess;
    using Domain;
    using Domain.Error;
    using Domain.Obligation;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
    using RequestHandlers.Shared;
    using Weee.Requests.Admin.Obligations;
    using Weee.Security;
    using Weee.Tests.Core;
    using Xunit;
    using FileInfo = Core.Shared.FileInfo;

    public class SubmitSchemeObligationsHandlerTests
    {
        private SubmitSchemeObligationHandler handler;
        private readonly IObligationCsvReader obligationCsvReader;
        private readonly IObligationUploadValidator obligationUploadValidator;
        private readonly IWeeeAuthorization authorization;
        private readonly ICommonDataAccess commonDataAccess;
        private readonly IObligationDataAccess obligationDataAccess;
        private readonly Fixture fixture;
        private readonly SubmitSchemeObligation request;

        public SubmitSchemeObligationsHandlerTests()
        {
            obligationCsvReader = A.Fake<IObligationCsvReader>();
            obligationUploadValidator = A.Fake<IObligationUploadValidator>();
            fixture = new Fixture();
            authorization = A.Fake<IWeeeAuthorization>();
            commonDataAccess = A.Fake<ICommonDataAccess>();
            obligationDataAccess = A.Fake<IObligationDataAccess>();

            var fileInfo = new FileInfo(fixture.Create<string>(), fixture.Create<byte[]>());
            request = new SubmitSchemeObligation(fileInfo, fixture.Create<CompetentAuthority>());

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization, commonDataAccess, obligationDataAccess);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization, commonDataAccess, obligationDataAccess);

            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NotAnAdminUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyAnyRole().Build();

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization, commonDataAccess, obligationDataAccess);

            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_InternalAccess_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_UserInAdminRole_ShouldBeChecked()
        {
            //act
            await handler.HandleAsync(request);

            //arrange
            A.CallTo(() => authorization.EnsureUserInRole(Roles.InternalAdmin)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_CompetentAuthorityShouldBeReceived()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(request.Authority)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndCompetentAuthorityNotFound_ArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._))
                .Returns((UKCompetentAuthority)null);

            //act
            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_CsvShouldBeRead()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationCsvReader.Read(request.FileInfo.Data)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_CsvDataShouldBeValidated()
        {
            //arrange
            var obligationUploadData = fixture.CreateMany<ObligationCsvUpload>().ToList();
            A.CallTo(() => obligationCsvReader.Read(A<byte[]>._)).Returns(obligationUploadData);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationUploadValidator.Validate(A<List<ObligationCsvUpload>>.That.Matches(o => o.SequenceEqual(obligationUploadData)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithNoErrors_ObligationDataAccessShouldBeCalled()
        {
            //arrange
            var obligationErrors = new List<ObligationUploadError>();
            var authority = fixture.Create<UKCompetentAuthority>();
            A.CallTo(() => obligationUploadValidator.Validate(A<List<ObligationCsvUpload>>._))
                .Returns(obligationErrors);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._)).Returns(authority);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.AddObligationUpload(A<UKCompetentAuthority>.That.Matches(a => a.Equals(authority)),
                A<string>.That.Matches(s => s.Equals(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data))),
                A<string>.That.Matches(s => s.Equals(request.FileInfo.FileName)), A<List<ObligationUploadError>>.That.Matches(o => o.SequenceEqual(obligationErrors)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithErrors_ObligationDataAccessShouldBeCalled()
        {
            //arrange
            var obligationErrors = fixture.CreateMany<ObligationUploadError>().ToList();
            var authority = fixture.Create<UKCompetentAuthority>();
            
            A.CallTo(() => obligationUploadValidator.Validate(A<List<ObligationCsvUpload>>._))
                .Returns(obligationErrors);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._)).Returns(authority);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.AddObligationUpload(A<UKCompetentAuthority>.That.Matches(a => a.Equals(authority)),
                A<string>.That.Matches(s => s.Equals(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data))),
                A<string>.That.Matches(s => s.Equals(request.FileInfo.FileName)), A<List<ObligationUploadError>>.That.Matches(o => o.SequenceEqual(obligationErrors)))).MustHaveHappenedOnceExactly();
        }

        public static IEnumerable<object[]> CsvExceptions =>
            new List<object[]>
            {
                new object[] { new CsvValidationException() },
                new object[] { new CsvReaderException() },
            };

        [Theory]
        [MemberData(nameof(CsvExceptions))]
        public async Task HandleAsync_GivenRequestWithCsvReaderExceptionError_ObligationDataAccessShouldBeCalled(Exception exception)
        {
            //arrange
            var authority = fixture.Create<UKCompetentAuthority>();
            A.CallTo(() => obligationCsvReader.Read(A<byte[]>._)).Throws(exception);
            A.CallTo(() => commonDataAccess.FetchCompetentAuthority(A<CompetentAuthority>._)).Returns(authority);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationDataAccess.AddObligationUpload(A<UKCompetentAuthority>.That.Matches(a => a.Equals(authority)),
                A<string>.That.Matches(s => s.Equals(System.Text.Encoding.UTF8.GetString(request.FileInfo.Data))),
                A<string>.That.Matches(s => s.Equals(request.FileInfo.FileName)), A<List<ObligationUploadError>>._)).MustHaveHappenedOnceExactly();

            A.CallTo(() => obligationDataAccess.AddObligationUpload(A<UKCompetentAuthority>._,
                A<string>._,
                A<string>._,
                A<List<ObligationUploadError>>.That.Matches(o => o.Exists(oe => oe.ErrorType == ObligationUploadErrorType.File && oe.Description.Equals("The error may be a problem with the file structure, which prevents our system from validating your file. You should rectify this error before we can continue our validation process."))))).MustHaveHappenedOnceExactly();
        }
    }
}
