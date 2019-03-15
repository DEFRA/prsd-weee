namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using System;
    using System.Threading.Tasks;

    public interface IGetSchemePublicInfoDataAccess
    {
        Task<Domain.Scheme.Scheme> FetchSchemeByOrganisationId(Guid organisationId);

        Task<Domain.Scheme.Scheme> FetchSchemeBySchemeId(Guid schemeId);
    }
}
