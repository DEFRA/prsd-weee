namespace EA.Weee.Requests.Organisations.Create.Base
{
    using System;
    using Prsd.Core.Mediator;

    public abstract class CreateOrganisationRequest : IRequest<Guid>
    {
        public string TradingName { get; set; }
    }
}
