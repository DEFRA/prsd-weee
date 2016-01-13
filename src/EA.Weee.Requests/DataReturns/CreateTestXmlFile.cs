namespace EA.Weee.Requests.DataReturns
{
    using Core.DataReturns;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    public class CreateTestXmlFile : IRequest<FileInfo>
    {
        public TestFileSettings Settings { get; private set; }

        public CreateTestXmlFile(TestFileSettings settings)
        {
            Settings = settings;
        }
    }
}
