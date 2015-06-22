namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Shared;

    internal class GetCountriesHandler : IRequestHandler<GetCountries, IList<CountryData>>
    {
        private readonly WeeeContext context;
        private readonly IMap<Country, CountryData> mapper;

        public GetCountriesHandler(WeeeContext context, IMap<Country, CountryData> mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IList<CountryData>> HandleAsync(GetCountries query)
        {
            var countries = await context.Countries.ToArrayAsync();
            return countries.Select(mapper.Map).ToArray();
        }
    }
}