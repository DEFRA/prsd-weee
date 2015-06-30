namespace EA.Weee.Requests.MemberRegistration
{
    using System;
    using EA.Prsd.Core.Mediator;

    public class ValidateXmlFile : IRequest<Guid>
    {
        public string Data { get; private set; }

        public ValidateXmlFile(string data)
        {
            Data = data;
        }
    }
}