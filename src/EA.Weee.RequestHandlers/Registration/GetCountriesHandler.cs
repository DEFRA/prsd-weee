namespace EA.Weee.RequestHandlers.Registration
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Registration;

    internal class GetCountriesHandler : IRequestHandler<GetCountries, CountryData[]>
    {
        private readonly IwsContext context;

        public GetCountriesHandler(IwsContext context)
        {
            this.context = context;
        }

        public async Task<CountryData[]> HandleAsync(GetCountries query)
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