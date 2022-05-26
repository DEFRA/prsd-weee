namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using FakeItEasy;
    using RequestHandlers.Admin.Obligations;
    using Weee.Requests.Admin.Obligations;
    using Xunit;
    using FileInfo = Core.Shared.FileInfo;

    public class SubmitSchemeObligationsHandlerTests
    {
        private SubmitSchemeObligationHandler handler;
        private readonly IFileHelper fileHelper;
        private readonly IObligationCsvReader obligationCsvReader;
        private readonly Fixture fixture;
        private readonly SubmitSchemeObligation request;

        public SubmitSchemeObligationsHandlerTests()
        {
            fileHelper = A.Fake<IFileHelper>();
            A.Fake<IWeeeCsvReader>();
            obligationCsvReader = A.Fake<IObligationCsvReader>();
            fixture = new Fixture();

            var fileInfo = new FileInfo(fixture.Create<string>(), fixture.Create<byte[]>());
            request = new SubmitSchemeObligation(fileInfo);

            handler = new SubmitSchemeObligationHandler(fileHelper, obligationCsvReader);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ValidateHeader()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => obligationCsvReader.ValidateHeader(request.FileInfo.Data)).MustHaveHappenedOnceExactly();
        }
    }
}
