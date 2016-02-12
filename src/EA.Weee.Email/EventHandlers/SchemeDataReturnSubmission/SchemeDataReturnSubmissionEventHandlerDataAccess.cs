namespace EA.Weee.Email.EventHandlers.SchemeDataReturnSubmission
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Domain.DataReturns;
    using Domain.Scheme;

    public class SchemeDataReturnSubmissionEventHandlerDataAccess : ISchemeDataReturnSubmissionEventHandlerDataAccess
    {
        private readonly WeeeContext weeeContext;

        public SchemeDataReturnSubmissionEventHandlerDataAccess(WeeeContext weeeContext)
        {
            this.weeeContext = weeeContext;
        }

        public Task<int> GetNumberOfDataReturnSubmissionsAsync(Scheme scheme, int complianceYear, QuarterType quarter)
        {
            return weeeContext.DataReturnVersions
                .Where(d => d.SubmittedDate.HasValue)
                .Where(d => d.DataReturn.Scheme.Id == scheme.Id)
                .Where(d => d.DataReturn.Quarter.Year == complianceYear)
                .Where(d => d.DataReturn.Quarter.Q == quarter)
                .CountAsync();
        }
    }
}
