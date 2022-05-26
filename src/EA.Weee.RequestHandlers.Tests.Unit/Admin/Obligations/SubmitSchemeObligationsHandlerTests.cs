namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Obligations
{
    using Core.Shared;
    using Core.Shared.CsvReading;
    using FakeItEasy;
    using RequestHandlers.Admin.Obligations;
    using Xunit;

    public class SubmitSchemeObligationsHandlerTests
    {
        private readonly SubmitSchemeObligationHandler handler;
        private readonly IWeeeCsvReader csvReader;
        private readonly IFileHelper fileHelper;

        public SubmitSchemeObligationsHandlerTests()
        {
            csvReader = A.Fake<IWeeeCsvReader>();
            fileHelper = A.Fake<IFileHelper>();

            handler = new SubmitSchemeObligationHandler(csvReader, fileHelper);
        }

        [Fact]
        public void Test()
        {

        }
    }
}
