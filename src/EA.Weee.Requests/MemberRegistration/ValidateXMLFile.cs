namespace EA.Weee.Requests.MemberRegistration
{
    using Prsd.Core.Mediator;
    using System;

    public class ValidateXMLFile : IRequest<Guid>
    {
        public ValidateXMLFile(Guid organisationId, string data)
        {
        }   
    }
}
