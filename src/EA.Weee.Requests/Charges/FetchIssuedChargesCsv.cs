namespace EA.Weee.Requests.Charges
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Shared;

    /// <summary>
    /// Requests a CSV file containing details of charges issued for the specified
    /// authority in the specified year, optionally filtered to a specified scheme.
    /// </summary>
    public class FetchIssuedChargesCsv : IRequest<FileInfo>
    {
        public CompetentAuthority Authority { get; private set; }

        public int ComplianceYear { get; private set; }

        public string SchemeName { get; private set; }
        
        public FetchIssuedChargesCsv(CompetentAuthority authority, int complianceYear, string schemeName)
        {
            Authority = authority;
            ComplianceYear = complianceYear;
            SchemeName = schemeName;
        }
    }
}
