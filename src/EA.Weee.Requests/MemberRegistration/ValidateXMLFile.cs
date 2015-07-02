namespace EA.Weee.Requests.MemberRegistration
{
    using Prsd.Core.Mediator;
    using System;

    public class ValidateXmlFile : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public string Data { get; private set; }

        public ValidateXmlFile(Guid organisationId, string data)
        {
            OrganisationId = organisationId;
            Data = data;
        }   
    }
}
