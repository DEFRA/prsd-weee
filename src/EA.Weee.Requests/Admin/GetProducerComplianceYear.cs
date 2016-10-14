namespace EA.Weee.Requests.Admin
{
    using Prsd.Core.Mediator;
    using System.Collections.Generic;

    public class GetProducerComplianceYear : IRequest<List<int>>
    {
        public string RegistrationNumber { get; set; }
    }
}
