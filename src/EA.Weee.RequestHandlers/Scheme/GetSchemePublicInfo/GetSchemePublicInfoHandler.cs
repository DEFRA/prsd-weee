namespace EA.Weee.RequestHandlers.Scheme.GetSchemePublicInfo
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Core.Shared;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class GetSchemePublicInfoHandler : IRequestHandler<Requests.Scheme.GetSchemePublicInfo, SchemePublicInfo>
    {
        private readonly IGetSchemePublicInfoDataAccess schemeDataAccess;
        private readonly IOrganisationDataAccess organisationDataAccess;

        public GetSchemePublicInfoHandler(IGetSchemePublicInfoDataAccess schemeDataAccess, 
            IOrganisationDataAccess organisationDataAccess)
        {
            this.schemeDataAccess = schemeDataAccess;
            this.organisationDataAccess = organisationDataAccess;
        }

        public async Task<SchemePublicInfo> HandleAsync(Requests.Scheme.GetSchemePublicInfo message)
        {
            var organisation = await organisationDataAccess.GetById(message.OrganisationId);

            if (organisation.IsBalancingScheme)
            {
                return new SchemePublicInfo()
                {
                    OrganisationId = organisation.Id,
                    Name = organisation.OrganisationName,
                    IsBalancingScheme = true
                };
            }

            var scheme = await schemeDataAccess.FetchSchemeByOrganisationId(message.OrganisationId);

            return new SchemePublicInfo()
            {
                SchemeId = scheme.Id,
                OrganisationId = scheme.OrganisationId,
                Name = scheme.SchemeName,
                ApprovalNo = scheme.ApprovalNumber,
                StatusName = scheme.SchemeStatus.DisplayName,
                Status = scheme.SchemeStatus.ToCoreEnumeration<SchemeStatus>(),
                IsBalancingScheme = false
            };
        }
    }
}
