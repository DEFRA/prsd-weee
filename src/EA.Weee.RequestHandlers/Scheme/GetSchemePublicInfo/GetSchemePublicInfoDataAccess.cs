namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using EA.Weee.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetSchemePublicInfoDataAccess : IGetSchemePublicInfoDataAccess
    {
        private readonly WeeeContext context;

        public GetSchemePublicInfoDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Domain.Scheme.Scheme> FetchSchemeByOrganisationId(Guid organisationId)
        {
            var results = await context.Schemes.Where(s => s.OrganisationId == organisationId).ToListAsync();

            if (results.Count == 0)
            {
                string errorMessage = string.Format("No scheme was found in the database with an ID of \"{0}\".", organisationId);
                throw new Exception(errorMessage);
            }

            if (results.Count > 1)
            {
                string errorMessage = string.Format("More than one scheme was found in the database with an ID of \"{0}\".", organisationId);
                throw new Exception(errorMessage);
            }

            return results[0];
        }
    }
}
