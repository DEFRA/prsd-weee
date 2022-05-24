﻿namespace EA.Weee.RequestHandlers.Shared
{
    using Core.Shared;
    using Domain;
    using Domain.Charges;
    using Domain.Lookup;
    using Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommonDataAccess
    {
        /// <summary>
        /// Fetches the domain object representing the specified competent authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<UKCompetentAuthority> FetchCompetentAuthority(CompetentAuthority authority);

        /// <summary>
        /// Returns all member uploads for the specified authority which are submitted and not yet
        /// assigned to an invoice run.
        /// Results will be ordered by scheme name ascending and then compliance year descending.
        /// The scheme and UK competent authority domain objects will be pre-loaded with each member upload returned.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<IReadOnlyList<MemberUpload>> FetchSubmittedNonInvoicedMemberUploadsAsync(UKCompetentAuthority authority);

        /// <summary>
        /// Returns all member uploads for the specified authority which have been  assigned to an invoice run.
        /// Results will be ordered by scheme name ascending and then compliance year descending.
        /// The scheme and UK competent authority domain objects will be pre-loaded with each member upload returned.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        Task<IReadOnlyList<MemberUpload>> FetchInvoicedMemberUploadsAsync(UKCompetentAuthority authority);

        /// <summary>
        /// Returns th invoice run with the specified ID.
        /// The 1B1S file data domain object will be pre-loaded where it is available.
        /// </summary>
        /// <param name="invoiceRunId"></param>
        /// <returns></returns>
        Task<InvoiceRun> FetchInvoiceRunAsync(Guid invoiceRunId);

        Task<UKCompetentAuthority> FetchCompetentAuthority(string authority);

        Task<UKCompetentAuthority> FetchCompetentAuthorityById(Guid authorityId);

        Task<T> FetchLookup<T>(Guid id) where T : AreaBase;

        Task<UKCompetentAuthority> FetchCompetentAuthorityApprovedSchemes(CompetentAuthority authority);
    }
}
