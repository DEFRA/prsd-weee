namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using System.Linq;
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
        private readonly SubmitSchemeObligationHandler handler;
        private readonly IObligationCsvReader obligationCsvReader;
        private readonly IObligationUploadValidator obligationUploadValidator;
        private readonly Fixture fixture;
        private readonly SubmitSchemeObligation request;

        public SubmitSchemeObligationsHandlerTests()
        {
            obligationCsvReader = A.Fake<IObligationCsvReader>();
            obligationUploadValidator = A.Fake<IObligationUploadValidator>();
            fixture = new Fixture();

            var fileInfo = new FileInfo(fixture.Create<string>(), fixture.Create<byte[]>());
            request = new SubmitSchemeObligation(fileInfo);

            handler = new SubmitSchemeObligationHandler(obligationCsvReader, obligationUploadValidator);
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
            A.CallTo(() => obligationUploadValidator.Validate(obligationUploadData)).MustHaveHappenedOnceExactly();
        }
    }
}
