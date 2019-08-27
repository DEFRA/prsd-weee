namespace EA.Weee.Requests.Organisations.Create.Base
{
    using Prsd.Core.Mediator;
    using System;

    public abstract class CreateOrganisationRequest : IRequest<Guid>
    {
        public string TradingName { get; set; }
    }
}
