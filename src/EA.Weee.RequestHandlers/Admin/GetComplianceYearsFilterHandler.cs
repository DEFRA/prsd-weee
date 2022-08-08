namespace EA.Weee.RequestHandlers.Admin
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.Requests.Admin;
    using Prsd.Core.Mediator;
    using Security;

    public class GetComplianceYearsFilterHandler : IRequestHandler<GetComplianceYearsFilter, IEnumerable<int>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IEvidenceDataAccess noteDataAccess;

        public GetComplianceYearsFilterHandler(IWeeeAuthorization authorization,
        IEvidenceDataAccess noteDataAccess)
        {
            this.authorization = authorization;
            this.noteDataAccess = noteDataAccess;
        }

        public async Task<IEnumerable<int>> HandleAsync(GetComplianceYearsFilter request)
        {
            authorization.EnsureCanAccessInternalArea();

            var statuses = request.AllowedStatuses.Select(a => a.ToDomainEnumeration<NoteStatus>()).ToList();

            var complianceYearList = await noteDataAccess.GetComplianceYearsForNotes(statuses.Select(s => s.Value).ToList());

            return complianceYearList;
        }
    }
}
