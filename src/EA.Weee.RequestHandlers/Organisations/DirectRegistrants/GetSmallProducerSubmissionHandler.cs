namespace EA.Weee.RequestHandlers.Organisations.DirectRegistrants
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.Domain.Producer;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Organisations.DirectRegistrant;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal class GetSmallProducerSubmissionHandler : IRequestHandler<GetSmallProducerSubmission, SmallProducerSubmissionData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IMapper mapper;

        public GetSmallProducerSubmissionHandler(IWeeeAuthorization authorization, 
            IGenericDataAccess genericDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.mapper = mapper;
        }

        public async Task<SmallProducerSubmissionData> HandleAsync(GetSmallProducerSubmission request)
        { 
            authorization.EnsureCanAccessExternalArea();

            var directRegistrant = await genericDataAccess.GetById<DirectRegistrant>(request.DirectRegistrantId);

            authorization.EnsureOrganisationAccess(directRegistrant.OrganisationId);

            var organisation = mapper.Map<Organisation, OrganisationData>(directRegistrant.Organisation);

            var currentYearSubmission = directRegistrant.DirectProducerSubmissions.FirstOrDefault(r => r.ComplianceYear == SystemTime.UtcNow.Year);

            if (currentYearSubmission != null)
            {
                return new SmallProducerSubmissionData()
                {
                    OrganisationData = organisation,
                    CurrentSubmission = new SmallProducerSubmissionHistoryData()
                    {
                        OrganisationDetailsComplete = currentYearSubmission.CurrentSubmission.BusinessAddressId.HasValue
                    }
                };
            }

            return null;
        }
    }
}
