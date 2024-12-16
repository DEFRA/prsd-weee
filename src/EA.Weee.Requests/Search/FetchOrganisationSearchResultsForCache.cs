namespace EA.Weee.Requests.Search
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Search;
    using System;
    using System.Collections.Generic;

    public class FetchOrganisationSearchResultsForCache : IRequest<IList<OrganisationSearchResult>>
    {
        public DateTime SmallProducerEnabledFrom { get; private set; }

        public FetchOrganisationSearchResultsForCache(DateTime smallProducerEnabledFrom)    
        {
            SmallProducerEnabledFrom = smallProducerEnabledFrom;
        }
    }
}
