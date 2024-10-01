namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using Core.DirectRegistrant;
    using Domain.Organisation;
    using Domain.Producer;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using Security;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataDataAccess;

        public GetSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, IMapper mapper, ISystemDataDataAccess systemDataDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
            this.systemDataDataAccess = systemDataDataAccess;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var organisation = mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation);

            var systemTime = await systemDataDataAccess.GetSystemDateTime();
            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(r => r.ComplianceYear == systemTime.Year);
            
            if (currentYearSubmission != null)
            {
                return new SmallProducerSubmissionData()
                {
                    DirectRegistrantId = request.DirectRegistrantId,
                    OrganisationData = organisation,
                    HasAuthorisedRepresentitive = directRegistrant.AuthorisedRepresentativeId.HasValue,
                    CurrentSubmission = mapper.Map<SmallProducerSubmissionHistoryData>(new DirectProducerSubmissionSource(directRegistrant, currentYearSubmission))
                };
            }
            return null;
        }
    }
}
