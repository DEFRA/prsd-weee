namespace EA.Weee.RequestHandlers.Scheme
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Admin;
    using DataAccess;

    public class GetSubmissionsHistoryResultsDataAccess : IGetSubmissionsHistoryResultsDataAccess
    {
        private readonly WeeeContext context;

        public GetSubmissionsHistoryResultsDataAccess(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<List<SubmissionsHistorySearchResult>> GetSubmissionsHistory(Guid orgId)
        {
            var scheme = (from s in context.Schemes
                          where s.OrganisationId == orgId
                          select s).SingleOrDefault();

            if (scheme == null)
            {
                return new List<SubmissionsHistorySearchResult>();
            }

            var schemeId = scheme.Id;

            var results = await(from mu in context.MemberUploads
                                 join user in context.Users on mu.UserId equals user.Id
                                 where mu.IsSubmitted & mu.SchemeId == schemeId
                                 select new SubmissionsHistorySearchResult
                                 {
                                     SchemeId = mu.SchemeId.Value,
                                     OrganisationId = mu.OrganisationId,
                                     MemberUploadId = mu.Id,
                                     SubmittedBy = user.FirstName + " " + user.Surname,
                                     Year = mu.ComplianceYear.Value,
                                     DateTime = mu.Date.Value,
                                     TotalCharges = mu.TotalCharges,
                                     NoOfWarnings = (from me in context.MemberUploadErrors
                                                     where me.MemberUploadId == mu.Id & (me.ErrorLevel.Value == Domain.ErrorLevel.Warning.Value)
                                                     select me).Count()
                                 }).OrderByDescending(s => s.DateTime).ToListAsync();
            return results;
        }
    }
}
