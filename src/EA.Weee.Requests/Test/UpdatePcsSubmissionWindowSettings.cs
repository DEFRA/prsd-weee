namespace EA.Weee.Requests.Test
{
    using Prsd.Core.Mediator;

    public class UpdatePcsSubmissionWindowSettings : IRequest<bool>
    {
        public bool FixCurrentQuarterAndComplianceYear { get; set; }

        public int? CurrentComplianceYear { get; set; }

        public int? SelectedQuarter { get; set; }
    }
}
