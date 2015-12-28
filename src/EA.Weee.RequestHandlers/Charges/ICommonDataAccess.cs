namespace EA.Weee.RequestHandlers.Charges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Core.Shared;
    using Domain;
    using Domain.Scheme;

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
    }
}
