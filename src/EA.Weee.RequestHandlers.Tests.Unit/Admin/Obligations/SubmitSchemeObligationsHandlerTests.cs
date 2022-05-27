namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared.CsvReading;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Admin.Obligations;
    using RequestHandlers.Security;
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
        private readonly Fixture fixture;
        private readonly SubmitSchemeObligation request;

        public SubmitSchemeObligationsHandlerTests()
        {
            obligationCsvReader = A.Fake<IObligationCsvReader>();
            obligationUploadValidator = A.Fake<IObligationUploadValidator>();
            fixture = new Fixture();
            authorization = A.Fake<IWeeeAuthorization>();
            var fileInfo = new FileInfo(fixture.Create<string>(), fixture.Create<byte[]>());
            request = new SubmitSchemeObligation(fileInfo);

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization);
        }

        [Fact]
        public async Task HandleAsync_NoInternalAccess_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization);

            var exception = await Record.ExceptionAsync(async () => await handler.HandleAsync(request));

            exception.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_NotAnAdminUser_ThrowsSecurityException()
        {
            var authorization = new AuthorizationBuilder().DenyAnyRole().Build();

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator, authorization);

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
    }
}
