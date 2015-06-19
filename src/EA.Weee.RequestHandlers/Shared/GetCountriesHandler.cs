namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Shared;

    internal class GetCountriesHandler : IRequestHandler<GetCountries, IList<CountryData>>
    {
        private readonly WeeeContext context;

        public GetCountriesHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<IList<CountryData>> HandleAsync(GetCountries query)
        {
            var result = await context.Countries.ToArrayAsync();
            var countryData = result.Select(c => new CountryData
            {
                Name = c.Name,
                Id = c.Id
            }).OrderBy(c => c.Name).ToArray();
            return countryData;
        }
    }
}