namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using DataAccess.DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System;
    using System.Threading.Tasks;

    public class GetSchemePublicInfoBySchemeIdHandler : IRequestHandler<Requests.Scheme.GetSchemePublicInfoBySchemeId, SchemePublicInfo>
    {
        private readonly ISchemeDataAccess dataAccess;

        public GetSchemePublicInfoBySchemeIdHandler(ISchemeDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<SchemePublicInfo> HandleAsync(Requests.Scheme.GetSchemePublicInfoBySchemeId request)
        {
            var scheme = await dataAccess.GetSchemeOrDefault(request.SchemeId);

            if (scheme == null)
            {
                throw new ArgumentException($"No scheme was found with id \"{request.SchemeId}\".");
            }

            return new SchemePublicInfo()
            {
                SchemeId = scheme.Id,
                OrganisationId = scheme.OrganisationId,
                Name = scheme.SchemeName,
                ApprovalNo = scheme.ApprovalNumber,
                Status = scheme.SchemeStatus.ToCoreEnumeration<SchemeStatus>()
            };
        }
    }
}
