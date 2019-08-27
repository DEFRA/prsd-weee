namespace EA.Weee.RequestHandlers.Scheme.UpdateSchemeInformation
{
    using Domain;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Scheme = Domain.Scheme.Scheme;

    public interface IUpdateSchemeInformationDataAccess
    {
        Task<Scheme> FetchSchemeAsync(Guid schemeId);

        Task<bool> CheckSchemeApprovalNumberInUseAsync(string approvalNumber);

        Task<UKCompetentAuthority> FetchEnvironmentAgencyAsync();

        Task<List<Scheme>> FetchNonRejectedEnvironmentAgencySchemesAsync();

        Task SaveAsync();
    }
}
