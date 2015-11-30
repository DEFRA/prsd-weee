namespace EA.Weee.Requests.Scheme.MemberUploadTesting
{
    using Core.Scheme.MemberUploadTesting;
    using Prsd.Core.Mediator;

    public class GeneratePcsXmlFile : IRequest<PcsXmlFile>
    {
        public ProducerListSettings Settings { get; private set; }

        public GeneratePcsXmlFile(ProducerListSettings settings)
        {
            Settings = settings;
        }
    }
}
