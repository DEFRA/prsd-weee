namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class ProcessXmlFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public string Data { get; private set; }

        public ProcessXmlFile(Guid organisationId, string data)
        {
            OrganisationId = organisationId;
            Data = data;
        }
    }
}