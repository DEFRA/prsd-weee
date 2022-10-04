namespace EA.Weee.Requests.Scheme
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    public class GetOrganisationScheme : IRequest<List<EntityIdDisplayNameData>>
    {
        public bool IncludePBS { get; private set; }

        public GetOrganisationScheme(bool includePBS)
        {
            this.IncludePBS = includePBS;
        }
    }
}
