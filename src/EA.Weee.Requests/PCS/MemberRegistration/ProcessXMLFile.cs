namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class ProcessXMLFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public byte[] Data { get; private set; }

        public ProcessXMLFile(Guid organisationId, byte[] data)
        {
            OrganisationId = organisationId;
            Data = data;
        }
    }
}