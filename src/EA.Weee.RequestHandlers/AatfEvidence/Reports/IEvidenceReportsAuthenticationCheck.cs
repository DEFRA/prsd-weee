namespace EA.Weee.RequestHandlers.AatfEvidence.Reports
{
    using System.Threading.Tasks;
    using Requests.AatfEvidence.Reports;

    public interface IEvidenceReportsAuthenticationCheck
    {
        Task EnsureIsAuthorised(GetEvidenceReportBaseRequest request);
    }
}