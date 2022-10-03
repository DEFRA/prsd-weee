namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Scheme;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Requests.Admin;
    using Prsd.Core.Mediator;
    using Security;

    public class GetAllAatfsForComplianceYearHandler : IRequestHandler<GetAllAatfsForComplianceYearRequest, List<OrganisationSchemeData>>
    { 
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;

        public GetAllAatfsForComplianceYearHandler(IWeeeAuthorization authorization,
            IEvidenceDataAccess evidenceDataAccess, IMapper mapper)
        {
            this.authorization = authorization;
            this.evidenceDataAccess = evidenceDataAccess;
            this.mapper = mapper;
        }

        public async Task<List<OrganisationSchemeData>> HandleAsync(GetAllAatfsForComplianceYearRequest message)
        {
            authorization.EnsureCanAccessInternalArea();

            var listAatfs = await evidenceDataAccess.GetAatfForAllNotesForComplianceYear(message.ComplianceYear);

            if (listAatfs == null)
            {
                throw new ArgumentException($"Aatfs not found for a compliance year {message.ComplianceYear}");
            }

            var result = mapper.Map<List<Domain.AatfReturn.Aatf>, List<OrganisationSchemeData>>(listAatfs);

            return result.OrderBy(n => n.DisplayName).ToListDynamic();
        }
    }
}
