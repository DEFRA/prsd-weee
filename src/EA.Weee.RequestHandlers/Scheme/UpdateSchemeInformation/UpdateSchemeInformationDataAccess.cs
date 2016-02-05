namespace EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using DataAccess;
    using Domain;
    using Scheme = Domain.Scheme.Scheme;

    public class UpdateSchemeInformationDataAccess : IUpdateSchemeInformationDataAccess
    {
        private readonly WeeeContext context;

        public UpdateSchemeInformationDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<Scheme> FetchSchemeAsync(Guid schemeId)
        {
            Scheme scheme = await context.Schemes
                .Where(s => s.Id == schemeId)
                .SingleOrDefaultAsync();

            if (scheme == null)
            {
                string errorMessage = string.Format(
                    "A scheme with ID \"{0}\" could not be found.",
                    schemeId);

                throw new ArgumentException(errorMessage);
            }

            return scheme;
        }

        public async Task<bool> CheckSchemeApprovalNumberInUseAsync(string approvalNumber)
        {
            return await context.Schemes
                .Where(s => s.ApprovalNumber == approvalNumber)
                .AnyAsync();
        }

        public async Task<UKCompetentAuthority> FetchEnvironmentAgencyAsync()
        {
            return await context.UKCompetentAuthorities
                .Where(a => a.Abbreviation == "EA")
                .SingleAsync();
        }

        /// <summary>
        /// Returns all schemes for the Environment Agency which are non-rejected.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Scheme>> FetchNonRejectedEnvironmentAgencySchemesAsync()
        {
            return await context.Schemes
                .Where(s => s.SchemeStatus.Value != (int)SchemeStatus.Rejected)
                .Where(s => s.CompetentAuthority.Abbreviation == "EA")
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
