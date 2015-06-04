namespace EA.Weee.Requests.Organisations
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class FindMatchingOrganisations : IRequest<IList<OrganisationSearchData>>
    {
        public string CompanyName { get; private set; }

        public FindMatchingOrganisations(string companyName)
        {
            if (companyName == null)
            {
                throw new ArgumentNullException();
            }

            CompanyName = companyName;
        }
    }
}