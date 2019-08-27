namespace EA.Weee.Email.EventHandlers.SchemeDataReturnSubmission
{
    using Domain.DataReturns;
    using System.Threading.Tasks;
    using Scheme = Domain.Scheme.Scheme;

    public interface ISchemeDataReturnSubmissionEventHandlerDataAccess
    {
        Task<int> GetNumberOfDataReturnSubmissionsAsync(Scheme scheme, int complianceYear, QuarterType quarter);
    }
}
