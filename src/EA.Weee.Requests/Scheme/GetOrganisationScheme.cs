namespace EA.Weee.Requests.Scheme
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;

    public class GetOrganisationScheme : IRequest<List<OrganisationSchemeData>>
    {
        public bool IncludePBS { get; private set; }

        public GetOrganisationScheme(bool includePBS)
        {
            this.IncludePBS = includePBS;
        }
    }
}
