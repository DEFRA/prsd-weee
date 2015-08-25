namespace EA.Weee.RequestHandlers.Scheme
{
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
