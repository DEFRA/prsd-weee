namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using Prsd.Core.Mediator;
    using System;

    public class ProcessXMLFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public byte[] Data { get; private set; }

        public string FileName { get; private set; }

        public ProcessXMLFile(Guid organisationId, byte[] data, string fileName)
        {
            OrganisationId = organisationId;
            Data = data;
            FileName = fileName;
        }
    }
}