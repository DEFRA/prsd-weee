namespace EA.Weee.Web.Services
{
    using System;

    public interface IHttpContextService
    {
        string GetAccessToken();

        bool HasOrganisationClaim(Guid organisationId);
    }
}