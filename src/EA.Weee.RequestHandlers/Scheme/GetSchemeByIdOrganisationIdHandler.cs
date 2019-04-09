namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Threading.Tasks;
    using Core.Scheme;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;
    using Weee.Security;

    internal class GetSchemeByIdOrganisationIdHandler : IRequestHandler<GetSchemeByOrganisationId, SchemeData>
        {
            private readonly ISchemeDataAccess dataAccess;
            private readonly IWeeeAuthorization authorization;
            private readonly IMapper mapper;

            public GetSchemeByIdOrganisationIdHandler(ISchemeDataAccess dataAccess,
                IMapper mapper,
                IWeeeAuthorization authorization)
            {
                this.dataAccess = dataAccess;
                this.mapper = mapper;
                this.authorization = authorization;
            }

            public async Task<SchemeData> HandleAsync(GetSchemeByOrganisationId request)
            {
                authorization.EnsureInternalOrOrganisationAccess(request.OrganisationId);

                var scheme = await dataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId);

                if (scheme == null)
                {
                    var message = $"No scheme was found with organisation id \"{request.OrganisationId}\".";
                    throw new ArgumentException(message);
                }

                var schemeData = mapper.Map<Scheme, SchemeData>(scheme);

                //CHECK
                //var complianceYearsWithSubmittedMemberUploads =
                //    await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(request.SchemeId);

                //var complianceYearsWithSubmittedDataReturns =
                //    await dataAccess.GetComplianceYearsWithSubmittedDataReturns(request.SchemeId);

                //var schemeDownloadsByYears = mapper.Map<Core.Scheme.SchemeDataAvailability>(
                //    Domain.Scheme.SchemeDataAvailability.Create(
                //        complianceYearsWithSubmittedMemberUploads,
                //        complianceYearsWithSubmittedDataReturns));

                //schemeData.SchemeDataAvailability = schemeDownloadsByYears;
                //schemeData.CanEdit = authorization.CheckUserInRole(Roles.InternalAdmin);
                return schemeData;
            }
        }      
}
