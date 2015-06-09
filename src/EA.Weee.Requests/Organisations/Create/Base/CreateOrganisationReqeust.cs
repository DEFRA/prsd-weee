namespace EA.Weee.Requests.Organisations.Create.Base
{
    using System;
    using Prsd.Core.Mediator;

    public abstract class CreateOrganisationReqeust : IRequest<Guid>
    {
        public string TradingName { get; set; }
    }
}
