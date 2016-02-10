namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Threading.Tasks;
    using Core.Scheme;
    using Core.Security;
    using DataAccess.DataAccess;
    using Domain.Scheme;
    using EA.Weee.RequestHandlers.Security;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Scheme;

    internal class GetSchemeByIdHandler : IRequestHandler<GetSchemeById, SchemeData>
        {
            private readonly ISchemeDataAccess dataAccess;
            private readonly IWeeeAuthorization authorization;
            private readonly IMapper mapper;

            public GetSchemeByIdHandler(ISchemeDataAccess dataAccess,
                IMapper mapper,
                IWeeeAuthorization authorization)
            {
                this.dataAccess = dataAccess;
                this.mapper = mapper;
                this.authorization = authorization;
            }

            public async Task<SchemeData> HandleAsync(GetSchemeById request)
            {
                authorization.EnsureCanAccessInternalArea();

                var scheme = await dataAccess.GetSchemeOrDefault(request.SchemeId);

                if (scheme == null)
                {
                    string message = string.Format("No scheme was found with id \"{0}\".", request.SchemeId);
                    throw new ArgumentException(message);
                }

                var schemeData = mapper.Map<Scheme, SchemeData>(scheme);

                var complianceYearsWithSubmittedMemberUploads =
                    await dataAccess.GetComplianceYearsWithSubmittedMemberUploads(request.SchemeId);

                var complianceYearsWithSubmittedDataReturns =
                    await dataAccess.GetComplianceYearsWithSubmittedDataReturns(request.SchemeId);

                var schemeDownloadsByYears = mapper.Map<Core.Scheme.SchemeDataAvailability>(
                    Domain.Scheme.SchemeDataAvailability.Create(
                        complianceYearsWithSubmittedMemberUploads,
                        complianceYearsWithSubmittedDataReturns));

                schemeData.SchemeDataAvailability = schemeDownloadsByYears;
                schemeData.CanEdit = authorization.CheckUserInRole(Roles.InternalAdmin);
                return schemeData;
            }
        }      
}
