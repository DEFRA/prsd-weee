namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;

    public class GetSchemeByIdDataAccess : IGetSchemeByIdDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemeByIdDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Scheme.Scheme> GetSchemeOrDefault(Guid schemeId)
        {
            return await context.Schemes.FindAsync(schemeId);
        }
    }
}
