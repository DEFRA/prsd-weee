namespace EA.Weee.Email.EventHandlers.SchemeDataReturnSubmission
{
    using System.Threading.Tasks;
    using Domain.DataReturns;
    using Scheme = Domain.Scheme.Scheme;

    public interface ISchemeDataReturnSubmissionEventHandlerDataAccess
    {
        Task<int> GetNumberOfDataReturnSubmissionsAsync(Scheme scheme, int complianceYear, QuarterType quarter);
    }
}
