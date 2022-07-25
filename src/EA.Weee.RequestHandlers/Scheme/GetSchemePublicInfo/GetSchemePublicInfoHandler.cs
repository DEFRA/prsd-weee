﻿namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System.Threading.Tasks;

    public class GetSchemePublicInfoHandler : IRequestHandler<Requests.Scheme.GetSchemePublicInfo, SchemePublicInfo>
    {
        private readonly IGetSchemePublicInfoDataAccess dataAccess;

        public GetSchemePublicInfoHandler(IGetSchemePublicInfoDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<SchemePublicInfo> HandleAsync(Requests.Scheme.GetSchemePublicInfo message)
        {
            Domain.Scheme.Scheme scheme = await dataAccess.FetchSchemeByOrganisationId(message.OrganisationId);

            return new SchemePublicInfo()
            {
                SchemeId = scheme.Id,
                OrganisationId = scheme.OrganisationId,
                Name = scheme.SchemeName,
                ApprovalNo = scheme.ApprovalNumber,
                StatusName = scheme.SchemeStatus.DisplayName,
                Status = scheme.SchemeStatus.ToCoreEnumeration<SchemeStatus>()
            };
        }
    }
}
