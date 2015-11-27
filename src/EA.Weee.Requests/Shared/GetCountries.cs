namespace EA.Weee.Requests.Shared
{
    using System.Collections.Generic;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetCountries : IRequest<IList<CountryData>>
    {
        //flag if true would only return UK regions
        //flag if false would return all the countries setup in database
        public bool UKRegionsOnly;

        public GetCountries(bool regionsOfUKOnly)
        {
            UKRegionsOnly = regionsOfUKOnly;
        }
    }
}
