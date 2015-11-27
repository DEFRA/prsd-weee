namespace EA.Weee.RequestHandlers.Shared
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
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
            var regionsOfUKOnly = query.UKRegionsOnly;
            var countries = await context.Countries.ToArrayAsync();
            if (regionsOfUKOnly)
            {
                var ukcompetentauthories = await context.UKCompetentAuthorities.ToArrayAsync();

                var ukregions = countries.Join(ukcompetentauthories, c => c.Id, u => u.Country.Id,
                    (c, u) => new CountryData { Id = c.Id, Name = c.Name });
                return ukregions.OrderBy(m => m.Name).ToList();
            }
            else
            {
                return countries.Select(mapper.Map).OrderBy(m => m.Name).ToArray();    
            }
        }
    }
}