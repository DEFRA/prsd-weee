namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGetSchemePublicInfoDataAccess
    {
        Task<Domain.Scheme.Scheme> FetchSchemeByOrganisationId(Guid organisationId);
    }
}
