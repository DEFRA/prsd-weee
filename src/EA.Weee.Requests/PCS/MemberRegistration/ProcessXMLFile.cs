namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class ProcessXMLFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public string Data { get; private set; }

        public ProcessXMLFile(Guid organisationId, string data)
        {
            OrganisationId = organisationId;
            Data = data;
        }
    }
}