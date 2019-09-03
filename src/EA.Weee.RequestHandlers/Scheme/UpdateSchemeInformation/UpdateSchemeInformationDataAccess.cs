﻿namespace EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation
{
    using Core.Shared;
    using DataAccess;
    using Domain;
    using EA.Weee.Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
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
                .Where(a => a.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
                .SingleAsync();
        }

        /// <summary>
        /// Returns all schemes for the Environment Agency which are non-rejected.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Scheme>> FetchNonRejectedEnvironmentAgencySchemesAsync()
        {
            return await context.Schemes
                .Where(s => s.SchemeStatus.Value != (int)Core.Shared.SchemeStatus.Rejected)
                .Where(s => s.CompetentAuthority.Abbreviation == UKCompetentAuthorityAbbreviationType.EA)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public void AddScheme(Scheme scheme)
        {
            context.Schemes.Add(scheme);
        }
    }
}
